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
    private float originalGravityScale;

    [Header("Lanterns")]
    private List<LanternBehaviour> lanterns;    //The list of lanterns.
    public float lanternSpacing = 2f;           //How far the lanterns should stay from each other and the main lantern.
    public float spacingWeightMultiplier = 2;   //Determines the ratio between cohesion and avoidance.

    [Header("Attack")]
    public List<Sprite> attackSprites;
    public GameObject attackGameObject;
    public Animator animator;
    public SpriteRenderer attackSpriteRenderer;
    public float attackDelay = 5f;
    public float attackDuration = 2f;
    public float amountOfLanternsNeeded = 10f;
    private bool canAttack;

    public List<AudioClip> attackSounds;
    private AudioSource audioSource;

    [Header("Other")]
    public GameObject MovementControls;
    public GameObject AttackControls;

    //private Camera camera;

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

        audioSource = GetComponent<AudioSource>();
        animator.enabled = false;
        attackGameObject.SetActive(false);
        originalGravityScale = rb.gravityScale;
        MovementControls.SetActive(true);
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

        if (direction != Vector2.zero)
        {
            if (rb.gravityScale == 0)
            {
                rb.gravityScale = originalGravityScale;
            }
            if (MovementControls.activeSelf)
            {
                MovementControls.SetActive(false);
            }
        }

        if (lanterns.Count > 0) {
            foreach (LanternBehaviour lantern in lanterns)
            {
                lantern.Move(CalculateMove(lantern, GetNearbyObjects(lantern)));
            }
        }
    }

    //Movement is handled here for a smoother experience.
    private void FixedUpdate() {
        direction.y = direction.y * 1.5f;
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

                player.CollectLantern(collision.gameObject.transform.position);
                lantern.SetActivate(transform);
                lanterns.Add(lantern);
                lantern.PlayCollectSound();

                if (GetLanternCount() == amountOfLanternsNeeded)
                {
                    canAttack = true;
                    AttackControls.SetActive(true);
                }

                GameManager.instance.LanternCollected(lanterns.Count); //Informs the GameManager that the player has gained a new lanterns, and reports the new amount.
                lantern.TurnLightOnOrOff(false); //Fades out the light.
            }
        }
    }

    private void Attack()
    {
        if (canAttack)
        {
            if (AttackControls.activeSelf)
            {
                AttackControls.SetActive(false);
            }

            canAttack = false;
            attackSpriteRenderer.sprite = attackSprites[UnityEngine.Random.Range(0, attackSprites.Count)];
            attackGameObject.SetActive(true);
            animator.enabled = true;

            audioSource.clip = attackSounds[UnityEngine.Random.Range(0, attackSounds.Count)];
            audioSource.Play();

            StartCoroutine(AttackTimer());
        }
    }

    private IEnumerator AttackTimer()
    {
        float attackTimer = 0f;

        while (attackTimer < attackDuration)
        {
            attackTimer += Time.deltaTime;
            yield return null;
        }
        animator.enabled = false;
        attackGameObject.SetActive(false);

        float attackDelayTimer = 0f;

        while (attackDelayTimer < attackDelay)
        {
            attackDelayTimer += Time.deltaTime;
            yield return null;
        }

        canAttack = true;
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

        //cohesionMove = Vector2.SmoothDamp(currentVelocity, cohesionMove, ref currentVelocity, smoothTime);

        //Smoothes out the movement.
        Vector2 combinedMove = cohesionMove + (avoidanceMove * spacingWeightMultiplier);

        return combinedMove;
    }

    public int GetLanternCount() {
        return lanterns.Count;
    }

    public void PlayerRespawn(float respawnTimer)
    {
        rb.gravityScale = 0f;
        controls.Disable();
        StartCoroutine(WaitForRespawn(respawnTimer));
    }

    private IEnumerator WaitForRespawn(float respawnTimer)
    {
        float timer = 0f;

        while (timer < respawnTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        controls.Enable();
    }
}
