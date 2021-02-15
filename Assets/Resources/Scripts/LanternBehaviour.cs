using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternBehaviour : MonoBehaviour
{
    public Rigidbody2D rb;

    private bool isActive = false;
    private readonly float movementSpeed = 11;

    private Transform target;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start() {

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
        Vector3 direction = target.position - transform.position;
        rb.AddRelativeForce(direction.normalized * movementSpeed);
    }

    private void PassiveBehaviour () {
        //idk bob up and down or something
    }

    private void Move(Vector2 velocity) {
        transform.position += (Vector3)velocity * Time.deltaTime;
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
