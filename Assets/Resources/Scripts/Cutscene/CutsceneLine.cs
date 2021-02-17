using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CutsceneLine
{
    public string text; //The text associated with this cutscene line.
    public int[] canvasIndexNumbers; //Reference index numbers for the CutsceneContents class. This int should be equal to the index number of the CanvasGroup that goes with this text.
}
