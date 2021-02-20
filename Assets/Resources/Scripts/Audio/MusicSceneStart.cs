using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSceneStart : MonoBehaviour
{
    public AudioClip trackToPlay;

    private void OnEnable()
    {
        SceneController.instance.OnAfterSceneLoad += SceneController_OnAfterSceneLoad;
    }
    private void OnDisable()
    {
        SceneController.instance.OnAfterSceneLoad -= SceneController_OnAfterSceneLoad;
    }

    private void SceneController_OnAfterSceneLoad()
    {
        if(GameManager.instance.audioSource.clip != trackToPlay)
        {
            GameManager.instance.musicManager.FadeOutFadeInTrack(trackToPlay);
        }
    }
}
