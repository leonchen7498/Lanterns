using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowBuildLogic : MonoBehaviour
{
    private InputMaster controls;

    private void OnEnable()
    {
        controls = new InputMaster();

        controls.Menu.CloseGame.performed += CloseGame_performed;

        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Menu.CloseGame.performed -= CloseGame_performed;

        controls.Disable();
    }


    private void CloseGame_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Application.Quit();
    }
}
