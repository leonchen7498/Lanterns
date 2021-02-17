using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(CutsceneManager))]
public class CutsceneContents : MonoBehaviour
{
    public CanvasGroup[] canvases; //Collection of all the sprites included in this cutscene.
    public CutsceneLine[] lines; //The lines are displayed in order by the CutsceneManager.

    public float textSpeed = 0.01f; //Pause time between each letter appearing, measured in seconds.
}
