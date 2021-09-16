using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentScript : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
