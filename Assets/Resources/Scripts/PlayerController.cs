using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Player player;

    //User input
    private InputMaster controls;

    [Header("Basic Movement")]
    private Rigidbody2D rb = null;
    public float movementSpeed = 10;
    [HideInInspector] public Vector2 direction;

    [Header("Dash")]
    public float dashCooldown = 3f;
    private float dashTimer = 0f;

    [Header("Lanterns")]
    private List<LanternBehaviour> lanterns;
    public float lanternSpacing = 1.5f;

    [Header("Attack")]
    public List<Sprite> attackSprites;
    public GameObject attackGameObject;
    public SpriteRenderer attackSpriteRenderer;
    public float attackDelay = 5f;
    public float attackGrowDuration = 0.5f;
    public float attackDuration = 1f;
    public float rotationSpeed = 50f;
    public float fadeOutTimer = 0.5f;
    public float attackScale = 1f;
    private bool canAttack;

    private Camera camera;

    //Runs before all Start functions
    private void Awake() {
        lanterns = new List<LanternBehaviour>();
        rb = GetComponent<Rigidbody2D>();
        //Ensures that the inputs have been initialized
        if (controls == null)
            controls = new InputMaster();

        controls.Player.Glow.performed += attack => Attack();
        /* //Old camera controls commented out.
        Camera camera = FindObjectOfType<Camera>();
        camera.transform.parent = transform;
        camera.transform.localPosition = new Vector3(0, 0, -3);
        */

        canAttack = true;
    }

    //Controls must be enabled and disabled with the object, otherwise it will not be read.
    private void OnEnable() {
        controls.Enable();
        GameManager.instance.activePlayer = this;
    }
    private void OnDisable() {
        controls.Disable();
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
        //I made it so the vertical movement is slower cus idk, it seemed weird that it moves vertically so quick -leon uwu
        direction.y = direction.y / 1.5f;
        rb.velocity = direction * movementSpeed * Time.fixedDeltaTime;
        CameraController.instance.MoveCamera(); //Called within this frame update to sync the camera with the player. If done via its own script, camera lags a frame behind, causing stuttering.
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        LanternBehaviour lantern = collision.GetComponent<LanternBehaviour>();

        //If the object is a lantern
        if (lantern != null) {
            //If the lantern isn't active, activate it and add it to the list.
            if (!lantern.GetActive()) {
                print("Activated lantern");

                player.SetLivesToMax();
                lantern.Activate(transform);
                lanterns.Add(lantern);

                GameManager.instance.LanternCollected(lanterns.Count); //Informs the GameManager that the player has gained a new lanterns, and reports the new amount.
                lantern.TurnLightOnOrOff(false); //Fades out the light.
            }
        }
    }

    private void Attack()
    {
        if (canAttack)
        {
            canAttack = false;
            attackSpriteRenderer.sprite = attackSprites[UnityEngine.Random.Range(0, 2)];

            StartCoroutine(StartAttack());
            StartCoroutine(RotatingAttack());
        }
    }

    private IEnumerator StartAttack()
    {
        for (float f = 0; f <= 1f; f += Time.deltaTime / attackGrowDuration)
        {
            Vector3 scale = attackGameObject.transform.localScale;
            scale.x = f * attackScale;
            scale.y = f * attackScale;

            attackGameObject.transform.localScale = scale;

            yield return null;
        }

        StartCoroutine(HoldAttack());
    }

    private IEnumerator HoldAttack()
    {
        float timer = 0f;

        while (timer < attackDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(FadeOutAttack());
    }

    private IEnumerator FadeOutAttack()
    {
        for (float f = 1; f >= 0f; f -= Time.deltaTime / fadeOutTimer)
        {
            Color color = attackSpriteRenderer.color;
            color.a = f;
            attackSpriteRenderer.color = color;
            yield return null;
        }

        Vector3 scale = attackGameObject.transform.localScale;
        scale.x = 0f;
        scale.y = 0f;

        attackGameObject.transform.localScale = scale;

        Color originalColor = attackSpriteRenderer.color;
        originalColor.a = 1f;
        attackSpriteRenderer.color = originalColor;

        StartCoroutine(StartAttackDelay());
    }

    private IEnumerator StartAttackDelay()
    {
        float timer = 0f;

        while (timer < attackDelay)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        canAttack = true;
    }

    private IEnumerator RotatingAttack()
    {
        attackGameObject.transform.localScale = new Vector3(0, 0, UnityEngine.Random.Range(0f, 1f));

        for (float f = 0; f <= 1f; f += Time.deltaTime / (attackGrowDuration + attackDuration))
        {
            attackGameObject.transform.Rotate(0, 0, Time.deltaTime * rotationSpeed);
            yield return null;
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
        if (Vector2.Distance(lantern.transform.position, transform.position) < lanternSpacing)
            avoidanceMove += (Vector2)(lantern.transform.position - transform.position);

        if (lanternsToAvoid.Count > 0) {
            avoidanceMove /= lanternsToAvoid.Count;
        }


        return cohesionMove + avoidanceMove;
    }

    public int GetLanternCount() {
        return lanterns.Count;
    }
}
