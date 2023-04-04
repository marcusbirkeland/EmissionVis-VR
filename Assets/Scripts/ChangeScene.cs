using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour
{
    public string sceneName;
    private AsyncOperation loadingOperation;
    public GameObject loadingScreen;
    public Camera mainCamera;

    public void ChangeToScene()
    {
        // remove layer mask containing all objects. This change is not permanent, and is reset upon a scene change
        mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Default"));
        loadingScreen.SetActive(true);
        loadingOperation = SceneManager.LoadSceneAsync(sceneName);
    }
}
