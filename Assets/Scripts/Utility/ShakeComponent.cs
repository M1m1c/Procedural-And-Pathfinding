using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShakeComponent : MonoBehaviour
{

    bool shouldShake = false;
    bool isCurrentlyShaking = false;

    private static ShakeComponent shakeComponentInstance;

    List<shakeRequest> shakeRequests = new List<shakeRequest>();

    public static void SetupShake(GameObject objectToShake)
    {
        //TODO pass in an object to be shaken and its properties for rates.
        //add object to shake requests list
        var shakeRequest = new shakeRequest(objectToShake, objectToShake.transform.position, 4f, 0.4f);

        shakeComponentInstance.shakeRequests.Add(shakeRequest);
    }

    private void Awake()
    {
        shakeComponentInstance = this;
    }

    private void Update()
    {
        if (shakeRequests.Count == 0) { return; }
        foreach (var item in shakeRequests)
        {
            ShakeObject(item);
        }
    }

    private void ShakeObject(shakeRequest sR)
    {

        sR.trauma = Mathf.Clamp(sR.trauma - (Time.deltaTime * sR.degerdationRateTrauma), 0.0f, 1.0f);

        var shake = Mathf.Pow(sR.trauma, 2f);

        float offX = sR.maxOffset * shake * Random.Range(-1.0f, 1.0f);
        float offY = sR.maxOffset * shake * Random.Range(-1.0f, 1.0f);


        Vector3 offsetPos = new Vector3(offX, offY, 0);

        sR.objectToshake.transform.position = sR.defaultObjectPos + offsetPos;

        if (Mathf.Approximately(sR.trauma, 0f))
        {
            shakeRequests.RemoveAll(q => q.objectToshake == sR.objectToshake);
        }
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
