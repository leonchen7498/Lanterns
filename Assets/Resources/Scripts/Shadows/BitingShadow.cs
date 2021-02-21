using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitingShadow : MonoBehaviour
{
    public float maxRange;
    public float speed = 5f;
    public float waitBeforeRestingAgain = 5f;
    public float stunTimer = 3f;
    public GameObject spriteObject;
    public Animator animator;

    public List<AudioClip> startAttackSounds;
    public List<AudioClip> getHitSounds;
    private AudioSource audioSource;

    private Vector3 originalPosition;
    private GameObject playerGameObject;
    private bool resting;
    private bool biting;
    private bool isStunned;
    private bool playerIsInArea;
    private float originalVolume;

    private Coroutine restCoroutine;
    private Coroutine stunCoroutine;

    void Start()
    {
        originalPosition = spriteObject.transform.position;
        audioSource = GetComponent<AudioSource>();
        originalVolume = audioSource.volume;
        animator.enabled = false;
    }

    void FixedUpdate()
    {
        if (isStunned)
        {
            if (!resting)
            {
                MoveBack(true);
            }

            return;
        }

        if (biting)
        {
            MoveTowardsPlayer();
        }
        else
        {
            if (playerIsInArea)
            {
                StartBiting();
            }

            if (!resting)
            {
                MoveBack(false);
            }
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
                StartBiting();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == Constants.Tags.Player)
        {
            restCoroutine = StartCoroutine(RestDelay());
            playerIsInArea = false;
        }
    }

    private void StartBiting()
    {
        animator.enabled = true;
        if (startAttackSounds != null && startAttackSounds.Count > 0)
        {
            audioSource.volume = originalVolume;
            audioSource.clip = startAttackSounds[Random.Range(0, startAttackSounds.Count)];
            audioSource.Play();
        }

        StopRestCoroutine();
        biting = true;
        resting = false;
    }

    public void AttackedByPlayer()
    {
        animator.enabled = false;
        if (!isStunned && getHitSounds != null && getHitSounds.Count > 0)
        {
            audioSource.volume = originalVolume / 2;
            audioSource.clip = getHitSounds[Random.Range(0, getHitSounds.Count)];
            audioSource.Play();
        }

        biting = false;
        isStunned = true;
        resting = false;

        StopRestCoroutine();
        StopStunCoroutine();

        stunCoroutine = StartCoroutine(StunEnemy());
    }

    private IEnumerator RestDelay()
    {
        yield return new WaitForSeconds(waitBeforeRestingAgain);
        animator.enabled = false;
        biting = false;
        restCoroutine = null;
    }

    private void MoveTowardsPlayer()
    {
        spriteObject.transform.right = playerGameObject.transform.position - spriteObject.transform.position;
        Vector3 playerPosition = playerGameObject.transform.position;
        float actualSpeed = speed * Time.deltaTime;
        Vector3 positionAfterMove = Vector3.MoveTowards(spriteObject.transform.position, playerPosition, actualSpeed);

        if (Vector3.Distance(positionAfterMove, originalPosition) < maxRange)
        {
            spriteObject.transform.position = positionAfterMove;
        }
    }

    private void MoveBack(bool isBeingAttacked)
    {
        float speedMultiplier = 1f;

        if (isBeingAttacked)
        {
            speedMultiplier = 10f;
        }

        float actualSpeed = speed / 2 * speedMultiplier * Time.deltaTime;
        spriteObject.transform.position = Vector3.MoveTowards(spriteObject.transform.position, originalPosition, actualSpeed);

        if (Vector3.Distance(spriteObject.transform.position, originalPosition) < 0.01f)
        {
            resting = true;
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

    private void StopRestCoroutine()
    {
        if (restCoroutine != null)
        {
            StopCoroutine(restCoroutine);
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
