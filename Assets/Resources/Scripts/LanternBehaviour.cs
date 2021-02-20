using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LanternBehaviour : MonoBehaviour
{
    public Rigidbody2D rb;

    //Light2D variables
    [SerializeField] private Light2D _light; //Since the component is in a child object, it is more computationally efficient to set this in editor rather than using GetComponentInChildren.
    private float initialInnerRadius;
    private float initialOuterRadius;
    private bool isLightFading = false;

    private bool isActive = false;
    public float movementSpeed = 11;
    public float lightFadeTime = 2f;

    public float smoothTime = 0.5f;         //Smoothes out the movements, preventing jittering.

    private Transform target;

    private Vector2 currentVelocity = Vector2.zero;
    private Vector2 targetVelocity = Vector2.zero;
    private Vector2 refVelocity = Vector2.zero;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        initialInnerRadius = _light.pointLightInnerRadius;
        initialOuterRadius = _light.pointLightOuterRadius;
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
        currentVelocity = Vector2.SmoothDamp(currentVelocity, targetVelocity, ref refVelocity, smoothTime);
        transform.position += (Vector3)currentVelocity * movementSpeed * Time.deltaTime;
    }

    private void PassiveBehaviour () {
        //idk bob up and down or something
    }

    public void Move(Vector2 velocity) {
        //transform.position += (Vector3)velocity * movementSpeed * Time.deltaTime;
        targetVelocity = velocity;
    }

    public void SetActivate (Transform target) {
        isActive = true;
        this.target = target;
    }

    public bool GetActive () {
        return isActive;
    }

    public void TurnLightOnOrOff(bool shouldTheLightBeOn)
    {
        if (!isLightFading)
        {
            if (shouldTheLightBeOn)
            {
                StartCoroutine(FadeLight(initialInnerRadius, initialOuterRadius, lightFadeTime));
            }
            else
            {
                StartCoroutine(FadeLight(0, 0, lightFadeTime));
            }
        }
    }

    private IEnumerator FadeLight(float finalInnerRadius, float finalOuterRadius, float fadeTime)
    {
        isLightFading = true;
        float fadeSpeed = Mathf.Abs(_light.pointLightOuterRadius - finalOuterRadius) / fadeTime;
        while (!Mathf.Approximately(_light.pointLightOuterRadius, finalOuterRadius))
        {
            _light.pointLightInnerRadius = Mathf.MoveTowards(_light.pointLightInnerRadius, finalInnerRadius, fadeTime * Time.deltaTime);
            _light.pointLightOuterRadius = Mathf.MoveTowards(_light.pointLightOuterRadius, finalOuterRadius, fadeTime * Time.deltaTime);
            yield return null;
        }
        isLightFading = false;
    }
}
