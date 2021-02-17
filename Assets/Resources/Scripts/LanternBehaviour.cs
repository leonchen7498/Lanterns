using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternBehaviour : MonoBehaviour
{
    public Rigidbody2D rb;

    private bool isActive = false;
    public float movementSpeed = 11;

    private Transform target;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        if (isActive) {
            ActiveBehaviour();
        } else {
            PassiveBehaviour();
        }
    }

    private void ActiveBehaviour () {

    }

    private void PassiveBehaviour () {
        //idk bob up and down or something
    }

    public void Move(Vector2 velocity) {
        transform.position += (Vector3)velocity * movementSpeed * Time.deltaTime;
    }

    public void Activate (Transform target) {
        isActive = true;
        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        renderer.material.color = Color.red;

        this.target = target;
    }

    public bool GetActive () {
        return isActive;
    }
}
