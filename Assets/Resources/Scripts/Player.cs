using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int lives = 3;
    public float invulnerableTimer = 3.0f;
    public float blinkInterval = 0.2f;
    public SpriteRenderer spriteRenderer;
    public Sprite noDamageSprite;
    public Sprite lightDamagedSprite;
    public Sprite heavyDamagedSprite;
    public GameObject parentGameObject;

    private bool invulnerable;
    private int currentLives;
    private Vector2 checkpointPosition;
    private List<Collision2D> dangerousCollisions;
    private List<Collider2D> shadowCollisions;
    private bool spriteVisible = true;
    private bool canBlink = true;
    private Color spriteColor;

    void Start()
    {
        currentLives = lives;
        checkpointPosition = parentGameObject.transform.position;
        dangerousCollisions = new List<Collision2D>();
        shadowCollisions = new List<Collider2D>();
        spriteColor = spriteRenderer.color;
    }

    void FixedUpdate()
    {
        if (invulnerable)
        {
            if (canBlink)
            {
                StartCoroutine(Blink());
            }
            return;
        }

        if (dangerousCollisions.Count > 0 || shadowCollisions.Count > 0)
        {
            LoseLife();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == Constants.Tags.Hazard)
        {
            dangerousCollisions.Add(collision);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == Constants.Tags.Shadow)
        {
            shadowCollisions.Add(collision);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == Constants.Tags.Hazard)
        {
            dangerousCollisions.Remove(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == Constants.Tags.Shadow)
        {
            shadowCollisions.Remove(collision);
        }
    }

    public void SetLivesToMax()
    {
        currentLives = lives;
        spriteRenderer.sprite = noDamageSprite;
    }

    private void LoseLife()
    {
        currentLives--;

        switch (currentLives)
        {
            case 2:
                TakeDamage(lightDamagedSprite);
                break;
            case 1:
                TakeDamage(heavyDamagedSprite);
                break;
            case 0:
                Respawn();
                break;
        }
    }

    private void TakeDamage(Sprite sprite)
    {
        //maybe do something else? like instantly flashing light
        spriteRenderer.sprite = sprite;
        StartCoroutine(StartInvulnerableTimer());
    }

    private void Respawn()
    {
        //maybe play an animation?
        parentGameObject.transform.position = checkpointPosition;
        spriteRenderer.sprite = noDamageSprite;
        currentLives = lives;
    }

    private IEnumerator StartInvulnerableTimer()
    {
        invulnerable = true;

        yield return new WaitForSeconds(invulnerableTimer);
        
        invulnerable = false;
        spriteRenderer.color = spriteColor;
    }

    private IEnumerator Blink()
    {
        canBlink = false;

        if (spriteVisible)
        {
            spriteRenderer.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, 0f);
        }
        else
        {
            spriteRenderer.color = spriteColor;
        }

        spriteVisible = !spriteVisible;

        yield return new WaitForSeconds(blinkInterval);
        canBlink = true;
    }
}
