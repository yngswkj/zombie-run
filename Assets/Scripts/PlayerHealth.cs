using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("HP")]
    public float maxHp = 100f;

    [Header("Damage Tuning")]
    public float invincibleSeconds = 0.35f; // ダメージ無敵時間

    float hp;
    float nextDamageTime;
    bool isDead;

    public float Hp => hp;
    public float Normalized => (maxHp <= 0f) ? 0f : hp / maxHp;
    public bool IsDead => isDead;

    void Start()
    {
        hp = maxHp;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        if (Time.time < nextDamageTime) return;
        if (amount <= 0f) return;

        hp = Mathf.Max(0f, hp - amount);
        nextDamageTime = Time.time + invincibleSeconds;

        if (hp <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("Player Dead");
    }
}
