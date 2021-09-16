using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentScript : MonoBehaviour
{
    public static int TotalScore { get; set; }
    public static int CurrentHealth { get; set; }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
