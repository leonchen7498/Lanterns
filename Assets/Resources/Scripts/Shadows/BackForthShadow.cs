using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackForthShadow : MonoBehaviour
{
    public float speed = 10.0f;
    public float waitBetweenAttacks = 5.0f;
    public Transform endPositionOfAttack;

    private Vector3 originalPosition;
    private Vector3 endPosition;
    private Vector3 currentGoalPosition;

    private float waitTimer;

    void Start()
    {
        originalPosition = transform.position;
        endPosition = endPositionOfAttack.position;
        currentGoalPosition = endPosition;
        waitTimer = waitBetweenAttacks;
    }

    void FixedUpdate()
    {
        if (EnemyCanMove())
        {
            Move();
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
        transform.position = Vector3.MoveTowards(transform.position, currentGoalPosition, actualSpeed);

        if (Vector3.Distance(transform.position, currentGoalPosition) < 0.01f)
        {
            if (currentGoalPosition == endPosition)
            {
                currentGoalPosition = originalPosition;
            }
            else
            {
                currentGoalPosition = endPosition;
            }

            waitTimer = 0f;
        }
    }
}
