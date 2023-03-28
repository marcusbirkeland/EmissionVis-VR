using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class ChangeScene : MonoBehaviour
{
    public string sceneName;
    private AsyncOperation loadingOperation;
    public UniversalRenderPipelineAsset pipeline;

    public void ChangeToScene()
    {

        loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        enabled = true;
        loadingOperation.completed += Test;
        // pipeline.mainLightRenderingMode = LightRenderingMode.PerPixel; <- IS READ ONLY
    }

    void Test(AsyncOperation obj)
    {
        Debug.Log("FINISHED!");
    }

    // Start is called before the first frame update
    void Start()
    {
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        Debug.Log(loadingOperation.progress);
        if (loadingOperation.isDone)
        {
            Debug.Log("FINISHED");
            // TODO: change lighting here?
        }*/
    }

}
