using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [Header("Damage")]
    public float damageAtMinSize = 5f;
    public float damageAtMaxSize = 20f;

    [Header("Tick")]
    public float damageInterval = 0.4f;

    float nextDamageTime;
    Enemy enemy;

    void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (Time.time < nextDamageTime) return;

        var health = collision.collider.GetComponentInParent<PlayerHealth>();
        if (health == null) return;

        float dmg = CalcDamageBySize();
        health.TakeDamage(dmg);

        nextDamageTime = Time.time + damageInterval;
    }

    float CalcDamageBySize()
    {
        // Enemyが持つmin / max sizeが使えるならそれを使う（なければscaleで代用）
        float size = transform.localScale.x;

        float min = (enemy != null) ? enemy.minSize : 0.1f;
        float max = (enemy != null) ? enemy.maxSize : 0.5f;

        float t = Mathf.InverseLerp(min, max, size);
        return Mathf.Lerp(damageAtMinSize, damageAtMaxSize, t);
    }
}
