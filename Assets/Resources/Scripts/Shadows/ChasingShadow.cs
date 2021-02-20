using System.Collections;
using UnityEngine;

public class ChasingShadow : MonoBehaviour
{
    public float speed = 5f;
    public float fadeTimer = 1f;
    public float stunTimer = 3f;
    public GameObject spriteObject;

    private Vector3 originalPosition;
    private GameObject playerGameObject;
    private SpriteRenderer spriteRenderer;
    private bool chasing;
    private bool isStunned;
    private bool playerIsInArea;

    private Coroutine currentCoroutine;
    private Coroutine stunCoroutine;

    void Start()
    {
        originalPosition = spriteObject.transform.position;

        spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
    }

    void FixedUpdate()
    {
        if (isStunned)
        {
            return;
        }

        if (chasing)
        {
            MoveTowardsPlayer();
        }
        else if (playerIsInArea)
        {
            StartChasing();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == Constants.Tags.Player)
        {
            playerGameObject = collision.gameObject;
            playerIsInArea = true;

            if (!isStunned)
            {
                StartChasing();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == Constants.Tags.Player)
        {
            StopCurrentCoroutine();

            currentCoroutine = StartCoroutine(FadeOutSprite(false));
        }
    }

    public void AttackedByPlayer()
    {
        chasing = false;
        spriteObject.SetActive(false);
        isStunned = true;
        StopCurrentCoroutine();
        StopStunCoroutine();
        
        currentCoroutine = StartCoroutine(FadeOutSprite(true));
        stunCoroutine = StartCoroutine(StunEnemy());
    }

    private void StartChasing()
    {
        chasing = true;
        spriteObject.SetActive(true);
        StopCurrentCoroutine();

        currentCoroutine = StartCoroutine(FadeInSprite());
    }

    private IEnumerator FadeInSprite()
    {
        for (float f = spriteRenderer.color.a; f <= 1f; f += Time.deltaTime / fadeTimer)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, f);
            yield return null;
        }

        currentCoroutine = null;
    }

    private IEnumerator FadeOutSprite(bool isBeingAttacked)
    {
        float speed = 1f;

        if (isBeingAttacked)
        {
            speed = 3f;
        }

        for (float f = spriteRenderer.color.a; f >= 0f; f -= Time.deltaTime / fadeTimer * speed)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, f);
            yield return null;
        }

        currentCoroutine = null;
        spriteObject.transform.position = originalPosition;
        chasing = false;
        spriteObject.SetActive(false);

        if (!isBeingAttacked)
        {
            playerIsInArea = false;
        }
    }

    private IEnumerator StunEnemy()
    {
        float f = 0;

        while (f < stunTimer)
        {
            f += Time.deltaTime;
            yield return null;
        }

        isStunned = false;
        stunCoroutine = null;
    }

    private void MoveTowardsPlayer()
    {
        Vector3 playerPosition = playerGameObject.transform.position;
        float actualSpeed = speed * Time.deltaTime;
        spriteObject.transform.position = Vector3.MoveTowards(spriteObject.transform.position, playerPosition, actualSpeed);
    }

    private void StopCurrentCoroutine()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
    }

    private void StopStunCoroutine()
    {
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }
    }
}
