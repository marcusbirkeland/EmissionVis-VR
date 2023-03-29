using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour
{
    public string sceneName;
    private AsyncOperation loadingOperation;
    public GameObject loadingScrene;
    public Camera mainCamera;

    public void ChangeToScene()
    {
        // remove layer mask containing all objects
        mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Default"));
        loadingScrene.SetActive(true);
        loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        loadingOperation.completed += LoadingFinished;
    }

    void LoadingFinished(AsyncOperation obj)
    {
        loadingScrene.SetActive(false);
        // add mask back to scene
        mainCamera.cullingMask |= (1 << LayerMask.NameToLayer("Default"));
    }
}
