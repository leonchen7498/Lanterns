using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBlockade : MonoBehaviour
{
    public int amountOfLanternsToUnlock;
    public float fadeOutTimer = 3f;
    private List<SpriteRenderer> shadowList;

    void Start()
    {
        GameManager.instance.OnLanternCollected += CheckAmountOfLanterns;
        shadowList = new List<SpriteRenderer>();
        shadowList.AddRange(GetComponentsInChildren<SpriteRenderer>());
    }

    private void CheckAmountOfLanterns()
    {
        if (GameManager.instance.currentLanternCount >= amountOfLanternsToUnlock)
        {
            StartCoroutine(FadeOutShadows());
        }
    }

    private IEnumerator FadeOutShadows()
    {
        for (float f = 1f; f >= 0f; f -= Time.deltaTime / fadeOutTimer)
        {
            shadowList.ForEach(shadow => shadow.color = new Color(shadow.color.r, shadow.color.g, shadow.color.b, f));
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
