using UnityEngine;

public class PickupEntity : MonoBehaviour
{

    public int CashValue { get; private set; }

    private void Awake()
    {
        var num = Random.Range(1, 11);

        CashValue = num * 10;
    }
}
