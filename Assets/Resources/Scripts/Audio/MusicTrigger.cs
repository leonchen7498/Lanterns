using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip trackToPlay;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.transform.parent == GameManager.instance.activePlayer.gameObject.transform) //This is computationally efficient spaghetti.
        {
            if(GameManager.instance.audioSource.clip != trackToPlay)
            {
                GameManager.instance.musicManager.FadeOutFadeInTrack(trackToPlay);
            }
        }
    }
}
