using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class MovableEntity : MonoBehaviour
{

    public UnityEvent Walking;
    public UnityEvent StoppingMovement;
    public UnityEvent RequestingPath;

    public Vector2Int MyGridPos { get; protected set; }

    protected List<HexTile> oldPath = new List<HexTile>();

    protected PathIndicatorGizmo pathGizmo;

    [SerializeField] protected float moveTime = 0.6f;

    protected bool isMoving = false;

    public void Setup(Vector2Int startCoord)
    {
        MyGridPos = startCoord;
    }

    public virtual void OnPathFound(List<HexTile> path, bool succeded) { }

    private void Awake()
    {
        pathGizmo = GetComponentInChildren<PathIndicatorGizmo>();
    }

    protected IEnumerator MoveAlongPath()
    {
        isMoving = true;
        HexTile goalTile = null;
        while (oldPath.Count > 0)
        {
            var targetTile = oldPath[0];
            goalTile = targetTile;
            yield return StartCoroutine(MoveToTile(targetTile));
            pathGizmo.RemovefirstPosition();
            oldPath.RemoveAt(0);
        }
        MyGridPos = goalTile.Coordinates;
        StoppingMovement.Invoke();
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
