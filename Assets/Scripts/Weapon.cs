using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponData data;
    public Transform firePoint;
    public Transform owner;          // 追加：弾が自分に当たらないように
    public string ownerTag = "Player";

    float nextFireTime;
    Coroutine burstRoutine;

    // 初期化
    void Awake()
    {
        if (owner == null) owner = transform; // WeaponがPlayerに付いてるならこれでOK
    }

    // 発射処理
    public void TryFire(Vector3 targetWorldPos)
    {

        // 発射レート制限
        float interval = 1f / Mathf.Max(0.01f, data.fireRate);
        if (Time.time < nextFireTime)
        {
            return;
        }
        nextFireTime = Time.time + interval;

        // バースト対応（不要なら burstCount=1 のままでOK）
        if (burstRoutine != null) StopCoroutine(burstRoutine);
        burstRoutine = StartCoroutine(FireBurst(targetWorldPos));
    }

    IEnumerator FireBurst(Vector3 targetWorldPos)
    {
        int count = Mathf.Max(1, data.burstCount);

        for (int i = 0; i < count; i++)
        {
            FireOnce(targetWorldPos);

            if (i < count - 1)
                yield return new WaitForSeconds(data.burstInterval);
        }

        burstRoutine = null;
    }

    // 弾丸を1回発射
    void FireOnce(Vector3 targetWorldPos)
    {

        // 狙う方向を計算
        Vector2 baseDir = ((Vector2)targetWorldPos - (Vector2)firePoint.position).normalized;
        if (baseDir.sqrMagnitude < 0.0001f) baseDir = Vector2.up;

        // 弾丸を複数発射（拡散対応）
        int pellets = Mathf.Max(1, data.projectilesPerShot);

        // 弾丸発射ループ
        for (int i = 0; i < pellets; i++)
        {
            float half = data.spreadAngleDeg * 0.5f;
            float angle = (pellets == 1) ? 0f : Random.Range(-half, half);
            Vector2 dir = Rotate(baseDir, angle);

            // 弾丸生成
            GameObject go = Instantiate(
                data.projectilePrefab.gameObject,
                firePoint.position,
                Quaternion.identity
            );

            // 初期化
            Projectile p = go.GetComponent<Projectile>();
            if (p == null)
            {
                Destroy(go);
                return;
            }

            p.Init(dir, data.projectileSpeed, owner, ownerTag, data.projectileLifeTime);
        }
    }

    // 2Dベクトルを指定角度回転させる
    static Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);
        return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y);
    }
}
