using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeComponent : MonoBehaviour
{

    bool shouldShake = false;
    bool isCurrentlyShaking = false;

    List<shakeRequest> shakeRequests = new List<shakeRequest>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeRequests.Count == 0) { return; }
        foreach (var item in shakeRequests)
        {
            ShakeObject(item);
        }
    }

    public void SetupShake()
    {
        //TODO pass in an object to be shaken and its properties for rates.
        //add object top shake requests list
    }

    private void ShakeObject(shakeRequest sR)
    {

        sR.trauma = Mathf.Clamp(sR.trauma - (Time.deltaTime * sR.degerdationRateTrauma), 0.0f, 1.0f);

        var shake = Mathf.Pow(sR.trauma, 2f);

        float offX = sR.maxOffset * shake * Random.Range(-1.0f, 1.0f);
        float offY = sR.maxOffset * shake * Random.Range(-1.0f, 1.0f);


       Vector3 offsetPos = new Vector3(offX, offY, 0);

        sR.objectToshake.transform.position = sR.defaultObjectPos + offsetPos;
    }
}

struct shakeRequest
{
    public GameObject objectToshake;

    public Vector3 defaultObjectPos;

    public float trauma; //= 0.0f;
    public float degerdationRateTrauma; //= 4.0f;
    public float maxOffset;//= 0.4f;

    public shakeRequest(GameObject obj, Vector3 defPos, float inDegredation, float inMaxOffset)
    {
        objectToshake = obj;
        defaultObjectPos = defPos;
        trauma = 0f;
        degerdationRateTrauma = inDegredation;
        maxOffset = inMaxOffset;
    }
}
