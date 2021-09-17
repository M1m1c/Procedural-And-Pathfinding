using UnityEngine;

public class PickupEntity : MonoBehaviour
{

    public int CashValue { get; private set; }

    public Sprite TreasureSprite_1;
    public Sprite TreasureSprite_2;

    private void Awake()
    {
        //Randomises CashValue
        var num = Random.Range(1, 11);
        CashValue = num * 10;

        //Randomises sprite
        var renderer = this.GetComponent<SpriteRenderer>();
        var rand = Random.Range(0, 2);

        if (rand == 0) { renderer.sprite = TreasureSprite_1; }
        else { renderer.sprite = TreasureSprite_2; }
    }
}
