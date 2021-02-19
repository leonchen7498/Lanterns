using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingShadowHitbox : MonoBehaviour
{
    public ChasingShadow chasingShadow;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == Constants.Tags.Attack)
        {
            chasingShadow.AttackedByPlayer();
        }
    }
}
