using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [SerializeField] private SceneController sceneController; //Manually set to avoid conflicts on the first frame of the game.
    private Camera cam;
    public Transform followTarget;
    private Vector3 movePosition;

    public float followSpeed = 5f;
    public float projectionSize = 10f;

    public float offsetY = 0f;

    public float howFarAhead = 0f;

    private void OnEnable()
    {        
        //Singleton design. Only one CameraController should ever exist, and is always referred to as "SceneController.instance"
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogWarning("Two CameraControllers detected, destroying newest one.");
            Destroy(gameObject);
        }

        cam = GetComponent<Camera>();
        sceneController.OnAfterSceneLoad += SceneController_OnAfterSceneLoad;
    }
    private void OnDisable()
    {
        sceneController.OnAfterSceneLoad -= SceneController_OnAfterSceneLoad;
    }

    // Update is called once per frame
    void Update()
    {
        cam.orthographicSize = projectionSize;
    }

    private void SceneController_OnAfterSceneLoad()
    {
        if(GameManager.instance.activePlayer != null)
            followTarget = GameManager.instance.activePlayer.gameObject.transform;
    }

    //This function is called by the PlayerController script to sync up the camera's movement with the player better.
    public void MoveCamera()
    {
        if (followTarget != null)
        {
            movePosition = new Vector3(followTarget.transform.position.x + GameManager.instance.activePlayer.direction.x * howFarAhead, followTarget.transform.position.y + +GameManager.instance.activePlayer.direction.y * howFarAhead + offsetY, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, movePosition, followSpeed);
        }
    }
}
