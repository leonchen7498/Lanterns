using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitingShadow : MonoBehaviour
{
    public float maxRange;
    public float speed = 5f;
    public float waitBeforeRestingAgain = 5f;
    public GameObject spriteObject;

    private Vector3 originalPosition;
    private GameObject playerGameObject;
    private bool resting;
    private bool biting;

    private Coroutine restCoroutine;

    void Start()
    {
        originalPosition = spriteObject.transform.position;
    }

    void FixedUpdate()
    {
        if (biting)
        {
            MoveTowardsPlayer();
        }
        else if (!biting && !resting)
        {
            MoveBack();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == Constants.Tags.Player)
        {
            if (restCoroutine != null)
            {
                StopCoroutine(restCoroutine);
            }

            if (playerGameObject == null)
            {
                playerGameObject = collision.gameObject;
            }

            biting = true;
            resting = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == Constants.Tags.Player)
        {
            restCoroutine = StartCoroutine(RestDelay());
        }
    }

    private IEnumerator RestDelay()
    {
        yield return new WaitForSeconds(waitBeforeRestingAgain);
        biting = false;
        restCoroutine = null;
    }

    private void MoveTowardsPlayer()
    {
        Vector3 playerPosition = playerGameObject.transform.position;
        float actualSpeed = speed * Time.deltaTime;
        Vector3 positionAfterMove = Vector3.MoveTowards(spriteObject.transform.position, playerPosition, actualSpeed);

        if (Vector3.Distance(positionAfterMove, originalPosition) < maxRange)
        {
            spriteObject.transform.position = positionAfterMove;
        }
    }

    private void MoveBack()
    {
        float actualSpeed = (speed / 2) * Time.deltaTime;
        spriteObject.transform.position = Vector3.MoveTowards(spriteObject.transform.position, originalPosition, actualSpeed);

        if (Vector3.Distance(spriteObject.transform.position, originalPosition) < 0.01f)
        {
            resting = true;
        }
    }
}
