using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    // 敵のサイズ変数
    [Header("Size")]
    public float minSize = 0.1f;
    public float maxSize = 0.5f;
    public EnemyHealthBar healthBar;

    [Header("HP(depends on size)")]
    public float baseHpAtMinSize = 2f;    // 最小サイズ時の基本HP
    public float baseHpAtMaxSize = 10f;   // 最大サイズ時の基本HP
    public bool hpScaleByArea = true;     // HPを面積比でスケーリング

    // 敵の速度変数
    [Header("Move")]
    public float moveSpeed = 2.5f;        // 敵の基本速度
    public float steering = 2.0f;         // 速度を寄せる強さ（大きいほど素直に追う）
    public float chaseWeight = 0.85f;     // 追尾の比率（0～1）
    public float wanderWeight = 0.15f;    // ふらつきの比率（0～1）
    public float wanderChangeRate = 1.0f; // ふらつきの更新速度

    [Header("VFX")]
    public GameObject deathEffectPrefab;
    public GameObject hitEffectPrefab;
    public GameObject bounceEffectPrefab;

    Rigidbody2D rb;
    Transform player;

    float maxHp;
    float hp;
    Vector2 wanderDir;

    void Start()
    {
        // 敵のサイズをランダムに設定
        float randomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomSize, randomSize, 1);

        // HP計算（サイズを0..1に正規化してLerp）
        float t = Mathf.InverseLerp(minSize, maxSize, randomSize);

        // まず線形でベースHPを作る
        float linearHp = Mathf.Lerp(baseHpAtMinSize, baseHpAtMaxSize, t);

        // 面積スケール
        if (hpScaleByArea)
        {
            // サイズを（0..1→0.5..1.5程度に補正して二乗）
            float areaFactor = Mathf.Lerp(0.7f, 1.3f, t); // 面積比の近似値
            linearHp *= areaFactor * areaFactor;
        }

        maxHp = Mathf.Max(1f, linearHp);
        hp = maxHp;

        if (healthBar != null)
        {
            healthBar.BindOwner(transform);
            healthBar.SetNormalized(1f);
        }

        // Rigidbody2D取得＆初期移動方向設定
        rb = GetComponent<Rigidbody2D>();
        wanderDir = Random.insideUnitCircle.normalized;
        rb.linearVelocity = wanderDir * moveSpeed;

        // プレイヤー取得
        var p = GameObject.FindGameObjectWithTag("Player");
        player = p != null ? p.transform : null;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // ふらつき方向を少しずつ変える
        var rnd = Random.insideUnitCircle.normalized;
        wanderDir = Vector2.Lerp(wanderDir, rnd, wanderChangeRate * Time.fixedDeltaTime).normalized;

        // プレイヤー方向
        Vector2 toPlayer = (Vector2)player.position - rb.position;
        Vector2 chaseDir = toPlayer.sqrMagnitude > 0.0001f ? toPlayer.normalized : Vector2.zero;

        // 追尾方向 + ふらつきを混ぜる
        Vector2 desiredDir = (chaseDir * chaseWeight) + (wanderDir * wanderWeight);
        if (desiredDir.sqrMagnitude > 0.0001f)
        {
            desiredDir = chaseDir;
        }

        Vector2 desiredVel = desiredDir.normalized * moveSpeed;

        // 今の速度を徐々に目的の速度に近づける
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, desiredVel, steering * Time.fixedDeltaTime);
    }

    public void TakeDamage(float amount)
    {
        hp -= amount;

        if (healthBar != null)
        {
            healthBar.SetNormalized(hp / maxHp);
        }

        // ヒットエフェクト
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        if (hp <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 contactPoint = collision.GetContact(0).point;
        GameObject bounceEffect = Instantiate(bounceEffectPrefab, contactPoint, Quaternion.identity);
        Destroy(bounceEffect, 1f);
    }
}
