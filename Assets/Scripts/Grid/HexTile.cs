using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour, IHeapItem<HexTile>
{
    //public HexCoordinates coordinates;

    public Vector2Int Coordinates { get; private set; }

    [EnumFlags]
    public TileTags tileProperties;

    public Color highlightColor;

    public Sprite WalkableSprite;

    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }

    public int HeapIndex { get; set; }

    public HexTile parent;

    public List<GameObject> Occupants { get; private set; }

    private SpriteRenderer spriteRenderer;

    private Color defaultColor;
    private Color walkableColor = new Color(0.764151f, 0.7598256f, 0.7598256f, 1f);
    

    private int maxHealth = 3;
    private int health = 3;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Occupants = new List<GameObject>();
        defaultColor = spriteRenderer.color;
    }

    public void Setup(Vector2Int startCoord)
    {
        Coordinates = startCoord;
    }

    public int CompareTo(HexTile hexTileToCompare)
    {
        var compare = fCost.CompareTo(hexTileToCompare.fCost);

        if (compare == 0) { compare = hCost.CompareTo(hexTileToCompare.hCost); }

        return -compare;
    }

    //TODO add special circumstance where player can walk onto tiles with pickups
    public bool OccupyTile(GameObject potentialOccupier)
    {
        var retval = false;
        if (potentialOccupier)
        {
            if (Occupants.Count == 0)
            {
                Occupants.Add(potentialOccupier);
                retval = true;
            }
            else if (!Occupants.Contains(potentialOccupier))
            {
                Occupants.Add(potentialOccupier);
                retval = true;
            }
        }

        return retval;
    }

    public void DeOccupyTile(GameObject deOccupier)
    {
        if (deOccupier)
        {
            if (Occupants.Count != 0)
            {
                if (Occupants.Contains(deOccupier))
                {
                    Occupants.Remove(deOccupier);                
                }
            }
        }
    }

    public void HighLightTile(bool shouldLighten)
    {
        if (shouldLighten) { ChangeTileColor(highlightColor,false); }
        else { ChangeTileColor(defaultColor,false); }
    }

    public void ChangeTileColor(Color newColor,bool shouldChangeDefault)
    {
        spriteRenderer.color = newColor;
        if (shouldChangeDefault) { defaultColor = newColor; }
    }

    public void ChangeToDefaultColor()
    {
        ChangeTileColor(defaultColor, false);
    }

    public void ReduceHealth()
    {
        health = Mathf.Clamp(health - 1, 0, maxHealth);
        if (health != 0) { return; }
        tileProperties = 0;
        ChangeTileColor(walkableColor,true);
        spriteRenderer.sprite = WalkableSprite;
    }

    public void ChangeSprite(Sprite newSprite)
    {
        if (!newSprite) { return; }
        spriteRenderer.sprite = newSprite;
    }
}
