using System.Collections;
using System.Linq;
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
    [SerializeField] private TextMeshProUGUI tmp;

    //Variables for internal logic
    private int currentIndex;
    private bool isTyping;
    private bool isFading;
    private int replacingGroupsCount = 0;

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
        List<CanvasGroup> initialCanvases = new List<CanvasGroup>();
        foreach (int index in contents.lines[0].canvasIndexNumbers)
        {
            initialCanvases.Add(contents.canvases[index]);
        }
        contents.canvases[0].alpha = 1;
        StartCoroutine(TypeText(contents.lines[0].text));
    }

    private IEnumerator NextLine()
    {
        currentIndex++;

        if (currentIndex <= contents.lines.Length - 1)
        {
            int[] currentNumbers = contents.lines[currentIndex].canvasIndexNumbers;
            int[] prevNumbers = contents.lines[currentIndex - 1].canvasIndexNumbers;
            tmp.text = "";
            if (prevNumbers != currentNumbers)
            {
                var toAdd = currentNumbers.Except(prevNumbers);
                var toDelete = prevNumbers.Except(currentNumbers);

                List<CanvasGroup> canvasGroupsToAdd = new List<CanvasGroup>();
                foreach (int index in toAdd)
                {
                    canvasGroupsToAdd.Add(contents.canvases[index]);
                }
                List<CanvasGroup> canvasGroupsToDelete = new List<CanvasGroup>();
                foreach (int index in toDelete)
                {
                    canvasGroupsToDelete.Add(contents.canvases[index]);
                }

                StartCoroutine(CanvasReplacingFade(canvasGroupsToAdd, canvasGroupsToDelete));

                while (replacingGroupsCount > 0)
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

    private IEnumerator CanvasReplacingFade(List<CanvasGroup> incomingCanvasGroups, List<CanvasGroup> outgoingCanvasGroups)
    {
        //This Coroutine fades out a sprite from view, changes the sprite, and fades the image back in.
        replacingGroupsCount++;

        isFading = true;
        if (outgoingCanvasGroups.Count > 0)
        {
            foreach (CanvasGroup group in outgoingCanvasGroups)
            {
                StartCoroutine(Fade(0f, group));
            }

            while (isFading)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        isFading = true;

        if (incomingCanvasGroups.Count > 0)
        {
            foreach (CanvasGroup group in incomingCanvasGroups)
            {
                StartCoroutine(Fade(1f, group));
            }

            while (isFading)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        isFading = false;

        replacingGroupsCount--;
    }

    private IEnumerator Fade(float finalAlpha, CanvasGroup fadingGroup)
    {
        //Generic fading Coroutine.

        isFading = true;
        float fadeSpeed = Mathf.Abs(fadingGroup.alpha - finalAlpha) / fadeTime;
        while (!Mathf.Approximately(fadingGroup.alpha, finalAlpha))
        {
            fadingGroup.alpha = Mathf.MoveTowards(fadingGroup.alpha, finalAlpha, fadeTime * Time.deltaTime);
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
