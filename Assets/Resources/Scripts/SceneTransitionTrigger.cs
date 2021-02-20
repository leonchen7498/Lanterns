using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionTrigger : MonoBehaviour
{
    public string sceneToLoad;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.transform.parent == GameManager.instance.activePlayer.gameObject.transform) //This is computationally efficient spaghetti.
        {
            SceneController.instance.FadeAndLoadScene(sceneToLoad);
        }
    }
}
