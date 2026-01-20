using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rb;
    Transform owner;
    string ownerTag;

    float damage;

    // 初期化処理
    public void Init(Vector2 direction, float speed, float damage, Transform owner, string ownerTag, float lifeTime)
    {
        this.damage = damage;
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
        if (other.isTrigger) return;  // ★Triggerは無視

        // ダメージ処理
        var dmg = other.GetComponentInParent<IDamageable>();
        if (dmg != null)
        {
            dmg.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
