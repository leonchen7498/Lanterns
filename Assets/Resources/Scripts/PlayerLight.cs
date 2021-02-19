using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerLight : MonoBehaviour
{
    [SerializeField] private Light2D _light;
    [SerializeField] private float expandInnerPerLantern = 1f;
    [SerializeField] private float expandOuterPerLantern = 2f;
    [SerializeField] private float expandTime = 2f;

    private int timesToExpand = 0;

    private void OnEnable()
    {
        GameManager.instance.OnLanternCollected += GameManager_OnLanternCollected;
    }
    private void OnDisable()
    {
        GameManager.instance.OnLanternCollected -= GameManager_OnLanternCollected;
    }

    private void GameManager_OnLanternCollected()
    {
        if(timesToExpand <= 0)
        {
            timesToExpand++;
            StartCoroutine(ExpandInnerLight());
            StartCoroutine(ExpandOuterLight());
        }
        else
        {
            timesToExpand++;
        }
    }

    private IEnumerator ExpandInnerLight()
    {
        while (timesToExpand > 0)
        {
            float finalSize = _light.pointLightInnerRadius + expandInnerPerLantern;
            float expandSpeed = Mathf.Abs(expandInnerPerLantern) / expandTime;
            while (!Mathf.Approximately(_light.pointLightInnerRadius, finalSize))
            {
                _light.pointLightInnerRadius = Mathf.MoveTowards(_light.pointLightInnerRadius, finalSize, expandSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }
    private IEnumerator ExpandOuterLight()
    {
        while (timesToExpand > 0)
        {
            float finalSize = _light.pointLightOuterRadius + expandOuterPerLantern;
            float expandSpeed = Mathf.Abs(expandOuterPerLantern) / expandTime;
            while (!Mathf.Approximately(_light.pointLightOuterRadius, finalSize))
            {
                _light.pointLightOuterRadius = Mathf.MoveTowards(_light.pointLightOuterRadius, finalSize, expandSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
