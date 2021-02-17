using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    public event Action OnBeforeSceneUnload;
    public event Action OnAfterSceneLoad;
    public string startingSceneName;
    public float fadeTime = 1f;
    public CanvasGroup faderCanvasGroup;

    private bool isFading;

    private void Awake()
    {
        //Singleton design. Only one SceneController should ever exist, and is always referred to as "SceneController.instance"
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogWarning("Two SceneControllers detected, destroying newest one.");
            Destroy(gameObject);
        }

    }
    private IEnumerator Start()
    {
        //Starts the game on the loading screen image, and starts loading the first scene.
        //The first scene needs to be set in the Unity inspector by name.
        //After the first scene is done loading, the loading screen image fades out.
        faderCanvasGroup.alpha = 1f;
        yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName));
        OnAfterSceneLoad?.Invoke();
        yield return StartCoroutine(Fade(0f));
    }

    public void FadeAndLoadScene(string sceneName)
    {
        if (!isFading)
        {
            StartCoroutine(FadeAndSwitchScenes(sceneName));
        }
    }

    private IEnumerator FadeAndSwitchScenes(string sceneName)
    {
        //Starts fading to the loading screen image, and invokes OnBeforeSceneUnload.
        yield return StartCoroutine(Fade(1f));
        OnBeforeSceneUnload?.Invoke();

        //Once the game has finished fading to the image, the currently active scene is unloaded.
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        //The new scene, passed on to SceneController by name, is loaded in.
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));
        OnAfterSceneLoad?.Invoke();

        //Once the scene is loaded, the loading screen image fades back out.
        yield return StartCoroutine(Fade(0f));
    }

    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        //Loads a new scene, and sets it as the active scene afterwards.
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        yield return newlyLoadedScene.isLoaded;
        SceneManager.SetActiveScene(newlyLoadedScene);
    }

    //The Fade coroutine handles an image that blocks the camera during load times.
    //Set finalAlpha to 0 to fade to the image, or to 1 to fade back in.
    public IEnumerator Fade(float finalAlpha)
    {
        isFading = true;
        faderCanvasGroup.blocksRaycasts = true;
        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeTime;
        while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha, fadeTime * Time.deltaTime);
            yield return null;
        }
        isFading = false;
        faderCanvasGroup.blocksRaycasts = false;
    }
}
