using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int lives = 3;
    public float invulnerableTimer = 3.0f;
    public float blinkInterval = 0.2f;
    public float deathFadeTimer = 0.5f;
    public float deathScreenTimer = 2f;
    public SpriteRenderer spriteRenderer;
    public Sprite noDamageSprite;
    public Sprite lightDamagedSprite;
    public Sprite heavyDamagedSprite;
    public GameObject parentGameObject;
    public PlayerController controller;
    public Image deathScreen;

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

    public void CollectLantern(Vector2 lanternPosition)
    {
        currentLives = lives;
        spriteRenderer.sprite = noDamageSprite;
        checkpointPosition = lanternPosition;
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
        spriteRenderer.sprite = sprite;
        StartCoroutine(StartInvulnerableTimer());
    }

    private void Respawn()
    {
        canBlink = false;
        invulnerable = true;
        StartCoroutine(FadeOut());
        GameManager.instance.PlayerRespawn();
    }

    private IEnumerator FadeOut()
    {
        float totalTime = deathFadeTimer * 3 + deathScreenTimer;
        controller.PlayerRespawn(totalTime);

        for (float f = spriteRenderer.color.a; f >= 0f; f -= Time.deltaTime / deathFadeTimer)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, f);
            yield return null;
        }

        StartCoroutine(DeathScreen());
    }

    private IEnumerator DeathScreen()
    {
        for (float f = deathScreen.color.a; f <= 1f; f += Time.deltaTime / deathFadeTimer)
        {
            deathScreen.color = new Color(deathScreen.color.r, deathScreen.color.g, deathScreen.color.b, f);
            yield return null;
        }
        
        parentGameObject.transform.position = checkpointPosition;
        spriteRenderer.sprite = noDamageSprite;
        currentLives = lives;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);

        float deathTimer = 0f;

        while (deathTimer < deathScreenTimer)
        {
            deathTimer += Time.deltaTime;
            yield return null;
        }

        for (float f = deathScreen.color.a; f >= 0f; f -= Time.deltaTime / deathFadeTimer)
        {
            deathScreen.color = new Color(deathScreen.color.r, deathScreen.color.g, deathScreen.color.b, f);
            yield return null;
        }

        invulnerable = false;
        canBlink = true;
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
