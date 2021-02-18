using System.Collections;
using UnityEngine;

public class ChasingShadow : MonoBehaviour
{
    public float speed = 5f;
    public float fadeTimer = 1f;
    public GameObject spriteObject;

    private Vector3 originalPosition;
    private GameObject playerGameObject;
    private SpriteRenderer spriteRenderer;
    private bool chasing;

    private Coroutine currentCoroutine;

    void Start()
    {
        originalPosition = spriteObject.transform.position;

        spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
    }

    void FixedUpdate()
    {
        if (chasing)
        {
            MoveTowardsPlayer();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == Constants.Tags.Player)
        {
            if (playerGameObject == null)
            {
                playerGameObject = collision.gameObject;
            }

            chasing = true;

            if(currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }

            currentCoroutine = StartCoroutine(FadeInSprite());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == Constants.Tags.Player)
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }

            currentCoroutine = StartCoroutine(FadeOutSprite());
        }
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
    private IEnumerator FadeOutSprite()
    {
        for (float f = spriteRenderer.color.a; f >= 0f; f -= Time.deltaTime / fadeTimer)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, f);
            yield return null;
        }

        currentCoroutine = null;
        MoveBackToOriginalPosition();
    }


    private void MoveTowardsPlayer()
    {
        Vector3 playerPosition = playerGameObject.transform.position;
        float actualSpeed = speed * Time.deltaTime;
        spriteObject.transform.position = Vector3.MoveTowards(spriteObject.transform.position, playerPosition, actualSpeed);
    }

    private void MoveBackToOriginalPosition()
    {
        spriteObject.transform.position = originalPosition;
        chasing = false;
    }
}
