using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableEntity : MonoBehaviour
{
    public Vector2Int MyGridPos { get; protected set; }

    protected List<HexTile> oldPath = new List<HexTile>();

    [SerializeField] protected float moveTime = 0.6f;

    protected bool isMoving = false;

    public virtual void OnPathFound(List<HexTile> path, bool succeded) { }

    protected IEnumerator MoveAlongPath()
    {
        isMoving = true;
        while (oldPath.Count > 0)
        {
            var targetTile = oldPath[0];
            yield return StartCoroutine(MoveToTile(targetTile));
            targetTile.ChangeTileColor(Color.white);
            oldPath.RemoveAt(0);
        }
        isMoving = false;
        yield return null;
    }

    protected IEnumerator MoveToTile(HexTile targetTile)
    {
        var elapsedTime = 0f;
        var startingPos = transform.position;
        var newPosition = targetTile.transform.position;
        while (elapsedTime < moveTime)
        {

            transform.position = Vector3.Lerp(startingPos, newPosition, (elapsedTime / moveTime));
            elapsedTime += Time.deltaTime;

            var dist = Vector3.Distance(transform.position, newPosition);
            if (dist > -0.1f && dist < 0.1f)
            {
                //var success = targetTile.OccupyTile(this.gameObject);
                //if (success) { MyGridPos = targetTile.Coordinates; }
                MyGridPos = targetTile.Coordinates;
            }
            yield return null;
        }
    }
}