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

    //Called when PathRequestManager Returns a path
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


    //Makes the entity move along the tiles in the list oldPath.
    //DeOccupys the tile the entity was standing on when it has reached a new tile.
    protected virtual IEnumerator MoveAlongPath()
    {
        isMoving = true;
        HexTile goalTile = null;
        while (oldPath.Count > 0)
        {          
            ContinuousWalking.Invoke();
            var oldTile = MyCurrentTile;
            var targetTile = oldPath[0];
            goalTile = targetTile;
            
            yield return StartCoroutine(MoveToTile(targetTile));

            pathGizmo.RemovefirstPosition();
            oldTile.DeOccupyTile(this.gameObject);
            oldPath.RemoveAt(0);            
        } 
        
        MyGridPos = goalTile.Coordinates;
        transform.position = goalTile.transform.position;    
        
        isMoving = false;
        StoppingMovement.Invoke();
        yield return null;
    }

    //Lerps entity from its current tile to its new tile and occupies it.
    protected IEnumerator MoveToTile(HexTile targetTile)
    {
        var elapsedTime = 0f;
        var startingPos = transform.position;
        var newPosition = targetTile.transform.position;
        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.Lerp(startingPos, newPosition, (elapsedTime / moveTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        var success = targetTile.OccupyTile(this.gameObject);
        if (success)
        {
            MyCurrentTile = targetTile;
            MyGridPos = targetTile.Coordinates;
        }
    }
}
