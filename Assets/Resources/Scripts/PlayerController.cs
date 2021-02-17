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

    [Header("Lanterns")]
    private List<LanternBehaviour> lanterns;
    public float lanternSpacing = 1.5f;

    //Runs before all Start functions
    private void Awake() {
        lanterns = new List<LanternBehaviour>();
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
        //Gets the current movement input value, used in FixedUpdate to move the player.
        direction = controls.Player.Movement.ReadValue<Vector2>();

        if (lanterns.Count > 0) {
            /*
            Vector3 averagePoint = Vector3.zero;
            float tempX = 0;
            float tempY = 0;
            for (int i = 0; i < lanterns.Count; i++) {
                tempX += lanterns[i].transform.position.x;
                tempY += lanterns[i].transform.position.y;
            }

            Vector2 lanternsAveragePosition = new Vector2(tempX / lanterns.Count, tempY / lanterns.Count);
            Vector2 groupAveragePosition = (lanternsAveragePosition + (Vector2)transform.position) / 2;
            print(groupAveragePosition);
            foreach (LanternBehaviour lantern in lanterns) {
                lantern.Move(groupAveragePosition - (Vector2)lantern.transform.position);
            }
            */

            foreach (LanternBehaviour lantern in lanterns)
            {
                lantern.Move(CalculateMove(lantern, GetNearbyObjects(lantern)));
            }
        }


        //Dash cooldown timer
        if (dashTimer > dashCooldown) {
            dashTimer -= Time.deltaTime;
        }
    }

    //Movement is handled here for a smoother experience.
    private void FixedUpdate() {
        transform.Translate(direction * movementSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        LanternBehaviour lantern = collision.GetComponent<LanternBehaviour>();

        //If the object is a lantern
        if (lantern != null) {
            //If the lantern isn't active, activate it and add it to the list.
            if (!lantern.GetActive()) {
                print("Activated lantern");

                lantern.Activate(transform);
                lanterns.Add(lantern);
            }
        }
    }

    private List<Transform> GetNearbyObjects (LanternBehaviour lantern) {
        List<Transform> context = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(lantern.transform.position, lanternSpacing);

        //Loops through each collider, checks if they're a lantern. If they're not the current lantern, add them to the list.
        foreach (Collider2D col in contextColliders) {
            LanternBehaviour lanternBehaviour = col.GetComponent<LanternBehaviour>();
            if (lanternBehaviour != lantern && lanternBehaviour != null) {
                context.Add(col.transform);
            }
        }
        return context;
    }

    //Calculates cohesion and avoidance for individual lanterns.
    private Vector2 CalculateMove(LanternBehaviour lantern, List<Transform> lanternsToAvoid) {
        if (lanterns.Count == 0)
            return Vector2.zero;

        //Finds the average position of the group
        Vector2 cohesionMove = Vector2.zero;
        foreach (LanternBehaviour item in lanterns) {
            cohesionMove += (Vector2)item.transform.position;
        }
        cohesionMove /= lanterns.Count;

        cohesionMove = ((Vector2)transform.position + cohesionMove) / 2;

        //Centers the vector on the lantern
        cohesionMove -= (Vector2)lantern.transform.position;

        Vector2 avoidanceMove = Vector2.zero;
        foreach (Transform item in lanternsToAvoid) {
            avoidanceMove += (Vector2)(lantern.transform.position - item.position);
        }

        if (lanternsToAvoid.Count > 0) {
            avoidanceMove /= lanternsToAvoid.Count;
        }


        return cohesionMove + avoidanceMove;
    }
}
