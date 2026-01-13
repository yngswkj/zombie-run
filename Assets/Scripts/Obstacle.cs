using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // 障害物のサイズ変数(プレハブ側で設定可能)
    public float minSize = 0.1f;
    public float maxSize = 0.5f;

    // 障害物の速度変数(プレハブ側で設定可能)
    [Header("Move")]
    public float moveSpeed = 2.5f;        // 障害物の基本速度
    public float steering = 2.0f;          // 速度を寄せる強さ（大きいほど素直に追う）
    public float chaseWeight = 0.85f;     // 追尾の比率（0～1）
    public float wanderWeight = 0.15f;    // ふらつきの比率（0～1）
    public float wanderChangeRate = 1.0f; // ふらつきの更新速度

    public GameObject bounceEffectPrefab;

    Rigidbody2D rb;
    Transform player;
    Vector2 wanderDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 障害物のサイズをランダムに設定
        float randomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomSize, randomSize, 1);

        rb = GetComponent<Rigidbody2D>();

        // プレイヤー取得
        var p = GameObject.FindGameObjectWithTag("Player");
        player = p != null ? p.transform : null;

        // 初期のふらつき方向と初速
        wanderDir = Random.insideUnitCircle.normalized;
        rb.linearVelocity = wanderDir * moveSpeed;
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 contactPoint = collision.GetContact(0).point;
        GameObject bounceEffect = Instantiate(bounceEffectPrefab, contactPoint, Quaternion.identity);
        Destroy(bounceEffect, 1f);
    }
}
