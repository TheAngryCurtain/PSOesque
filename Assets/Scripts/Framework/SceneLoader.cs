using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    private AsyncOperation mAsyncOp = null;
    private UIEvents.AsyncSceneLoadProgressEvent mProgressEvent;

    public override void Awake()
    {
        base.Awake();

        SceneManager.sceneLoaded += OnSceneLoaded;

        mProgressEvent = new UIEvents.AsyncSceneLoadProgressEvent();
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

    private void Update()
    {
        if (mAsyncOp != null)
        {
            mProgressEvent.Progress = mAsyncOp.progress;
            VSEventManager.Instance.TriggerEvent(mProgressEvent);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex > (int)Enums.eScene.Boot)
        {
            VSEventManager.Instance.TriggerEvent(new UIEvents.SceneLoadedEvent((Enums.eScene)scene.buildIndex));
            mAsyncOp = null;
        }
    }
}
