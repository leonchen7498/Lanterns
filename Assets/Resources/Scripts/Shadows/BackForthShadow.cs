using System.Collections.Generic;
using UnityEngine;

public class BackForthShadow : MonoBehaviour
{
    public float speed = 10.0f;
    public float waitBetweenAttacks = 5.0f;
    public Transform endPositionOfAttack;

    public List<AudioClip> getHitSounds;
    private AudioSource audioSource;

    private Vector3 originalPosition;
    private Vector3 endPosition;
    private Vector3 currentGoalPosition;

    private bool isAttacked;
    private SpriteRenderer spriteRenderer;

    private float waitTimer;

    void Start()
    {
        originalPosition = transform.position;
        endPosition = endPositionOfAttack.position;
        currentGoalPosition = endPosition;
        waitTimer = waitBetweenAttacks;

        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        if (EnemyCanMove())
        {
            Move();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(isAttacked)
        {
            return;
        }

        if (collision.gameObject.tag == Constants.Tags.Attack)
        {
            if (getHitSounds != null && getHitSounds.Count > 0)
            {
                audioSource.clip = getHitSounds[Random.Range(0, getHitSounds.Count)];
                audioSource.Play();
            }

            if (waitTimer < waitBetweenAttacks)
            {
                waitTimer = 0f;
                return;
            }

            float distanceOfPlayerToOriginal = Vector3.Distance(collision.transform.position, originalPosition);
            float distanceOfPlayerToEndPosition = Vector3.Distance(collision.transform.position, endPosition);

            if (distanceOfPlayerToOriginal >= distanceOfPlayerToEndPosition)
            {
                float distanceOfShadowToEndPosition = Vector3.Distance(transform.position, endPosition);

                if (distanceOfShadowToEndPosition >= distanceOfPlayerToEndPosition)
                {
                    currentGoalPosition = originalPosition;
                    spriteRenderer.flipX = true;
                }
                else
                {
                    currentGoalPosition = endPosition;
                    spriteRenderer.flipX = false;
                }
            }
            else
            {
                float distanceOfShadowToOriginal = Vector3.Distance(transform.position, originalPosition);

                if (distanceOfShadowToOriginal >= distanceOfPlayerToOriginal)
                {
                    currentGoalPosition = endPosition;
                    spriteRenderer.flipX = false;
                }
                else
                {
                    currentGoalPosition = originalPosition;
                    spriteRenderer.flipX = true;
                }
            }

            isAttacked = true;
        }
    }

    /// <summary>
    /// Checks if <see cref="waitTimer"/> has surpassed <see cref="waitBetweenAttacks"/>
    /// </summary>
    /// <returns>Whether or not the shadow is allowed to attack again</returns>
    private bool EnemyCanMove()
    {
        if (waitTimer < waitBetweenAttacks)
        {
            waitTimer += Time.deltaTime;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Moves the gameobject to the <see cref="currentGoalPosition"/>
    /// If it reached it's goal it will start moving back to it's original position
    /// </summary>
    private void Move()
    {
        float actualSpeed = speed * Time.deltaTime;

        if (isAttacked)
        {
            actualSpeed *= 5f;
        }

        transform.position = Vector3.MoveTowards(transform.position, currentGoalPosition, actualSpeed);

        if (Vector3.Distance(transform.position, currentGoalPosition) < 0.01f)
        {
            if (currentGoalPosition == endPosition)
            {
                currentGoalPosition = originalPosition;
                spriteRenderer.flipX = true;
            }
            else
            {
                currentGoalPosition = endPosition;
                spriteRenderer.flipX = false;
            }

            isAttacked = false;
            waitTimer = 0f;
        }
    }
}
