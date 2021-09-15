using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System.Linq;

public class MovableEntity : MonoBehaviour
{

    public UnityEvent StartWalking;
    public UnityEvent StoppingMovement;
    public UnityEvent SelectionAction;
    public UnityEvent ContinuousWalking;

    public Vector2Int MyGridPos { get; protected set; }
    protected HexTile MyCurrentTile = null;

    protected List<HexTile> oldPath = new List<HexTile>();

    protected PathIndicatorGizmo pathGizmo;

    [SerializeField]protected float moveTime = 0.6f;

    protected bool isMoving = false;

    protected bool isRequestingPath = false;

    //TODO implement a step system that counts how many steps every entiety can make in the folowing action.
    //make sure that other movable enteties are listening to player and that player waits for them before bieng able to
    //perform a new action.
    //for instance if the player performs a attack tile,
    //player cannot perform another attack tile until all enemies have finished moving one tile or decided a path.

    public HexTile GetGoalTile()
    {
        return oldPath.LastOrDefault();
    }

    public virtual void Setup(Vector2Int startCoord, HexTile startTile)
    {
        MyGridPos = startCoord;
        MyCurrentTile = startTile;
        startTile.OccupyTile(this.gameObject);
    }

    public virtual void OnPathFound(List<HexTile> path, bool succeded)
    {
        if (!succeded) { return; }

        oldPath = path;

        pathGizmo.SetupPath(oldPath, transform.position);
    }

    private void Awake()
    {
        OnAwake();
    }

    protected virtual void OnAwake()
    {
        pathGizmo = GetComponentInChildren<PathIndicatorGizmo>();
    }

    protected virtual IEnumerator MoveAlongPath()
    {
        isMoving = true;
        HexTile goalTile = null;
        while (oldPath.Count > 0)
        {
            ContinuousWalking.Invoke();
            var targetTile = oldPath[0];
            goalTile = targetTile;
            
            yield return StartCoroutine(MoveToTile(targetTile));
            pathGizmo.RemovefirstPosition();
            StartCoroutine(targetTile.DeOccupyTile(this.gameObject));
            oldPath.RemoveAt(0);            
        }       
        MyGridPos = goalTile.Coordinates;
        transform.position = goalTile.transform.position;      
        isMoving = false;
        StoppingMovement.Invoke();
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
                var success = targetTile.OccupyTile(this.gameObject);
                if (success)
                {
                    MyCurrentTile = targetTile;
                    MyGridPos = targetTile.Coordinates;
                }
            }
            yield return null;
        }
    }
}
