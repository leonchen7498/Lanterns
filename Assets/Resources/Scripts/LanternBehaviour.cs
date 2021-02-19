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

    private Transform target;

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

    }

    private void PassiveBehaviour () {
        //idk bob up and down or something
    }

    public void Move(Vector2 velocity) {
        transform.position += (Vector3)velocity * movementSpeed * Time.deltaTime;
    }

    public void Activate (Transform target) {
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
