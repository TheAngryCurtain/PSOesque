using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private AsyncOperation mAsyncOp = null;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        RequestSceneLoad(Enums.eScene.Main);
    }

    public void RequestSceneLoad(Enums.eScene scene)
    {
        int buildIndex = (int)scene;
        SceneManager.LoadScene(buildIndex);
    }

    public void RequestSceneLoadAsync(Enums.eScene scene)
    {
        int buildIndex = (int)scene;
        mAsyncOp = SceneManager.LoadSceneAsync(buildIndex);
    }

    public float QueryLoadProgress()
    {
        if (mAsyncOp == null)
        {
            return -1f;
        }

        return mAsyncOp.progress;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex > (int)Enums.eScene.Boot)
        {
            VSEventManager.Instance.TriggerEvent(new UIEvents.SceneLoadedEvent((Enums.eScene)scene.buildIndex));
        }
    }
}
