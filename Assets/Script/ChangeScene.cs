using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public float loadProgress = 0.0f;
    private AsyncOperation loadingOperation;

    public void ChangeToScene(string sceneName)
    {

        loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        enabled = true;

    }

    // Start is called before the first frame update
    void Start()
    {
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        loadProgress = loadingOperation.progress;
        Debug.Log(loadProgress);
        if (loadingOperation.isDone)
        {
            Debug.Log("FINISHED");
            // TODO: change lighting here?
        }
    }

}
