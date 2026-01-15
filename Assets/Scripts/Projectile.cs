using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rb;

    Transform owner;
    string ownerTag;

    // 初期化処理
    public void Init(Vector2 direction, float speed, Transform owner, string ownerTag, float lifeTime)
    {
        this.owner = owner;
        this.ownerTag = ownerTag;

        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction.normalized * speed;

        Destroy(gameObject, lifeTime);
    }

    // 衝突処理
    void OnTriggerEnter2D(Collider2D other)
    {
        //  プレイヤー配下（PlayerBody等）に当たったら無視
        if (owner != null && other.transform.IsChildOf(owner)) return;
        Destroy(gameObject);
    }
}
