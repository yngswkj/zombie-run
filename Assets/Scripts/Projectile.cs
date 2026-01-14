using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rb;

    Transform owner;
    string ownerTag;

    public void Init(Vector2 direction, float speed, float damage, Transform owner, string ownerTag, float lifeTime)
    {
        this.owner = owner;
        this.ownerTag = ownerTag;

        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction.normalized * speed;

        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ★プレイヤー配下（PlayerBody等）に当たったら無視
        if (owner != null && other.transform.IsChildOf(owner)) return;

        // ★タグでも無視（保険）
        if (!string.IsNullOrEmpty(ownerTag) && other.CompareTag(ownerTag)) return;

        Destroy(gameObject);
    }
}
