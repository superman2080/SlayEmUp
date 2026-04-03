using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : Singleton<SceneLoader>
{
    public float Progress { get; private set; } = 0f;

    public bool IsLoading { get; private set; } = false;

    public event Action<string> OnLoadStarted;
    public event Action<string> OnSceneLoaded;

    private AsyncOperation currentAsyncOperation;

    [SerializeField] private Canvas asyncLoadCanvas;
    [SerializeField] private Slider progressSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        asyncLoadCanvas = gameObject.GetComponentInChildren<Canvas>();
        progressSlider = gameObject.GetComponentInChildren<Slider>();
        asyncLoadCanvas.gameObject.SetActive(false);
    }

    private void Reset()
    {
        asyncLoadCanvas = gameObject.GetComponentInChildren<Canvas>();
        progressSlider = gameObject.GetComponentInChildren<Slider>();
    }

    public void SceneLoadAsync(string sceneName)
    {
        StartCoroutine(SceneLoadAsyncCoroutine(sceneName));
    }

    private IEnumerator SceneLoadAsyncCoroutine(string sceneName)
    {
        if(IsLoading)
        {
            Debug.LogError("Scene loading is already processing");
            yield break;
        }

        OnLoadStarted?.Invoke(sceneName);
        IsLoading = true;
        Progress = 0f;
        asyncLoadCanvas.gameObject.SetActive(true);

        currentAsyncOperation = SceneManager.LoadSceneAsync(sceneName);

        while(currentAsyncOperation.isDone == false)
        {
            yield return null;

            if(currentAsyncOperation.progress < 0.9f)
            {
                Progress = currentAsyncOperation.progress;
            }
            else
            {
                Progress = 1f;
                currentAsyncOperation.allowSceneActivation = true;
            }
            LoadProgress(Progress);
        }

        asyncLoadCanvas.gameObject.SetActive(false);
        IsLoading = false;
        OnSceneLoaded?.Invoke(sceneName);
    }

    private void LoadProgress(float value)
    {
        progressSlider.value = value;
    }
}
