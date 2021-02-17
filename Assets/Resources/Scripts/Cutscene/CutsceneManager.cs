using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CutsceneContents))]
public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager instance; //Singleton name.

    private InputMaster controls;

    public float fadeTime = 1.0f; //Fade time in and out of an image being displayed.

    //Required components. Automatically aquired in SceneController_OnAfterSceneLoad()
    private CutsceneContents contents;
    private CanvasGroup imageCanvasGroup;
    [SerializeField] private TextMeshProUGUI tmp;

    //Variables for internal logic
    private int currentIndex;
    private bool isTyping;
    private bool isFading;
    private bool isReplacingSprite;

    private void OnEnable()
    {
        //Singleton design for ease of reference.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Two or more CutsceneManagers detected.");
        }

        controls = new InputMaster();

        //The CutsceneManager uses the Menu control bindings.
        controls.Menu.Confirm.performed += ctx => OnConfirm_performed();
        SceneController.instance.OnAfterSceneLoad += SceneController_OnAfterSceneLoad;

        controls.Enable();
    }

    private void OnDisable()
    {
        //Cleanup.
        controls.Menu.Confirm.performed -= ctx => OnConfirm_performed();
        SceneController.instance.OnAfterSceneLoad += SceneController_OnAfterSceneLoad;

        controls.Disable();
    }
    private void OnConfirm_performed()
    {
        if (!isTyping && !isFading) //Won't let new line be displayed while old one is still being "printed".
        {
            StartCoroutine(NextLine());
        }
    }

    private void SceneController_OnAfterSceneLoad()
    {
        //Automatic component aquisition.
        contents = GetComponent<CutsceneContents>();

        //Current implementation starts the cutscene immediately when the scene is loaded.
        StartCutscene();
    }

    private void StartCutscene()
    {
        currentIndex = 0;
        imageCanvasGroup = contents.canvases[0];
        contents.canvases[0].alpha = 1;
        StartCoroutine(TypeText(contents.lines[0].text));
    }

    private IEnumerator NextLine()
    {
        currentIndex++;
        if(currentIndex <= contents.lines.Length - 1)
        {
            tmp.text = "";
            if ((contents.lines[currentIndex-1].canvasIndexNumber != contents.lines[currentIndex].canvasIndexNumber))
            {
                isReplacingSprite = true;
                StartCoroutine(CanvasReplacingFade(contents.canvases[currentIndex]));
                while (isReplacingSprite)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            StartCoroutine(TypeText(contents.lines[currentIndex].text));
        }
        else
        {
            CutsceneComplete();
        }

        //Debug.Log("Line displayed successfully.");
        yield return null;
    }

    private IEnumerator TypeText(string textToType)
    {
        isTyping = true;
        tmp.text = "";
        int textLenght = textToType.Length;

        //Types text one letter at a time, pausing in between each depending on textSpeed.
        for (int i = 0; i < textLenght; i++)
        {
            tmp.text += textToType.Substring(i, 1);
            yield return new WaitForSeconds(contents.textSpeed);
        }

        isTyping = false;
        yield return null;
    }

    private IEnumerator CanvasReplacingFade(CanvasGroup newCanvasGroup)
    {
        //This Coroutine fades out a sprite from view, changes the sprite, and fades the image back in.
        isReplacingSprite = true;

        isFading = true;
        StartCoroutine(Fade(0f));
        while (isFading)
        {
            yield return new WaitForEndOfFrame();
        }

        imageCanvasGroup = newCanvasGroup;

        isFading = true;
        imageCanvasGroup = contents.canvases[currentIndex];
        StartCoroutine(Fade(1f));
        while (isFading)
        {
            yield return new WaitForEndOfFrame();
        }

        isReplacingSprite = false;
    }

    private IEnumerator Fade(float finalAlpha)
    {
        //Generic fading Coroutine.

        isFading = true;
        float fadeSpeed = Mathf.Abs(imageCanvasGroup.alpha - finalAlpha) / fadeTime;
        while (!Mathf.Approximately(imageCanvasGroup.alpha, finalAlpha))
        {
            imageCanvasGroup.alpha = Mathf.MoveTowards(imageCanvasGroup.alpha, finalAlpha, fadeTime * Time.deltaTime);
            yield return null;
        }
        isFading = false;
    }

    private void CutsceneComplete()
    {
        //TODO: Figure out implementation
        Debug.Log("The cutscene is over. No implementation is present beyond this point.");
    }
}
