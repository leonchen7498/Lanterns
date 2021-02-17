using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingShadow : MonoBehaviour
{
    public float speed = 5f;
    public GameObject spriteObject;

    private Vector3 originalPosition;
    private GameObject playerGameObject;
    private bool chasing;

    void Start()
    {
        originalPosition = spriteObject.transform.position;
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
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == Constants.Tags.Player)
        {
            MoveBackToOriginalPosition();
        }
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
