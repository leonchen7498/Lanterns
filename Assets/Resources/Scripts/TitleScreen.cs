using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    private InputMaster controls;
    public string sceneToLoadOnConfirm = "Cutscene";

    private void OnEnable()
    {
        controls = new InputMaster();

        controls.Menu.Confirm.performed += Confirm_performed;

        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Confirm_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        SceneController.instance.FadeAndLoadScene(sceneToLoadOnConfirm);
    }
}
