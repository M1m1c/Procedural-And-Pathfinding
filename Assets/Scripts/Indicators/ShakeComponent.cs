using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShakeComponent : MonoBehaviour
{

    private static ShakeComponent shakeComponentInstance;

    //trauma is between 0f and 1f
    public static void SetupShake(GameObject objectToShake, float trauma)
    {
        var shakeRequest = new shakeRequest(objectToShake, objectToShake.transform.position, trauma);

        shakeComponentInstance.StartCoroutine(shakeComponentInstance.ShakeObject(shakeRequest));
    }

    private void Awake()
    {
        shakeComponentInstance = this;
    }

    //Shakes an object in the x axis based on trauma, whihc is reduced overtime by degredation rate.
    //Returns object to its normal position once trauma is 0.
    private IEnumerator ShakeObject(shakeRequest sR)
    {

        while (sR.trauma > 0f)
        {
            yield return null;
            sR.trauma = Mathf.Clamp(sR.trauma - (Time.deltaTime * sR.degerdationRateTrauma), 0.0f, 1.0f);

            var shake = Mathf.Pow(sR.trauma, 2f);

            float offX = sR.maxOffset * shake * Random.Range(-1.0f, 1.0f);

            Vector3 offsetPos = new Vector3(offX, 0f, 0f);

            sR.objectToshake.transform.position = sR.defaultObjectPos + offsetPos;
        }

        yield return null;

    }
}

struct shakeRequest
{
    public GameObject objectToshake;

    public Vector3 defaultObjectPos;

    public float trauma; //= 0.0f;
    public float degerdationRateTrauma; //= 4.0f;
    public float maxOffset;//= 0.4f;

    public shakeRequest(GameObject obj, Vector3 defPos, float inTrauma)
    {
        objectToshake = obj;
        defaultObjectPos = defPos;
        trauma = Mathf.Clamp(inTrauma, 0f, 1f);
        degerdationRateTrauma = 4.0f;
        maxOffset = 2f;
    }
}
