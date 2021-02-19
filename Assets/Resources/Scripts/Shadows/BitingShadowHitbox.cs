using UnityEngine;

public class BitingShadowHitbox : MonoBehaviour
{
    public BitingShadow bitingShadow;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == Constants.Tags.Attack)
        {
            bitingShadow.AttackedByPlayer();
        }
    }
}
