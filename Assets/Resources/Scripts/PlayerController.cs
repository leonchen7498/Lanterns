using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //User input
    private InputMaster controls;

    [Header("Basic Movement")]
    public float movementSpeed = 10;
    private Vector2 direction;

    [Header("Dash")]
    public float dashCooldown = 3f;
    private float dashTimer = 0f;

    //Runs before all Start functions
    private void Awake() {

        //Ensures that the inputs have been initialized
        if (controls == null)
            controls = new InputMaster();
    }

    //Controls must be enabled and disabled with the object, otherwise it will not be read.
    private void OnEnable() {
        controls.Enable();
    }
    private void OnDisable() {
        controls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        //Gets the current movement input value
        direction = controls.Player.Movement.ReadValue<Vector2>();

        //Dash cooldown timer
        if (dashTimer > dashCooldown) {
            dashTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate() {
        transform.Translate(direction * movementSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        LanternBehaviour lantern = collision.GetComponent<LanternBehaviour>();

        //If the object is a lantern
        if (lantern != null) {
            //If the lantern isn't active
            if (!lantern.GetActive()) {
                lantern.Activate(transform);
                print("Activated lantern");
            }
        }
    }
}
