using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextLevelMenu : MonoBehaviour
{
    private static NextLevelMenu NextLevelMenuInstance;
    private Button nextLevelButton;
    private Vector3 activeButtonPos = new Vector3(0, -490, 0);
    private Vector3 disabledButtonPos = new Vector3(0, -600, 0);
    private bool isButtonActive = true;
    private float SlideTime = 0.2f;

    private void Awake()
    {
        NextLevelMenuInstance = this;
        nextLevelButton = GetComponentInChildren<Button>();
        //DisplayButton();
    }

    public void ClickButton()
    {
        Debug.Log("Click");
        if (!isButtonActive) { return; }
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public static void DisplayButton()
    {
        var menu = NextLevelMenuInstance;
        if (menu.isButtonActive == true) { return; }
        menu.isButtonActive = true;
        menu.StartCoroutine(menu.SlideButtonIntoPlace(menu.disabledButtonPos, menu.activeButtonPos));
    }

    public static void HideButton()
    {
        var menu = NextLevelMenuInstance;
        if (menu.isButtonActive == false) { return; }
        menu.isButtonActive = false;
        menu.StartCoroutine(menu.SlideButtonIntoPlace(menu.activeButtonPos, menu.disabledButtonPos));
    }

    private IEnumerator SlideButtonIntoPlace(Vector3 start, Vector3 end)
    {
        var elapsedTime = 0f;
        var rect = nextLevelButton.GetComponent<RectTransform>();
        while (elapsedTime < SlideTime)
        {
            rect.anchoredPosition = Vector3.Lerp(start, end, Mathf.Pow((elapsedTime / SlideTime), 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

}
