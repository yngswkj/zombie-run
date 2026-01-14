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

    void Awake()
    {
        if (owner == null) owner = transform; // WeaponがPlayerに付いてるならこれでOK
    }

    public void TryFire(Vector3 targetWorldPos)
    {
        Debug.Log($"[Weapon] TryFire called. data={(data ? data.name : "null")} firePoint={(firePoint ? firePoint.name : "null")}");

        if (data == null || firePoint == null)
        {
            Debug.LogWarning("[Weapon] data or firePoint is null -> return");
            return;
        }

        if (data.projectilePrefab == null)
        {
            Debug.LogWarning("[Weapon] projectilePrefab is null -> return");
            return;
        }

        float interval = 1f / Mathf.Max(0.01f, data.fireRate);
        if (Time.time < nextFireTime)
        {
            // Debug.Log($"[Weapon] cooldown -> return");
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

    void FireOnce(Vector3 targetWorldPos)
    {
        // 狙う方向
        Vector2 baseDir = ((Vector2)targetWorldPos - (Vector2)firePoint.position).normalized;
        if (baseDir.sqrMagnitude < 0.0001f) baseDir = Vector2.up;

        int pellets = Mathf.Max(1, data.projectilesPerShot);

        for (int i = 0; i < pellets; i++)
        {
            float half = data.spreadAngleDeg * 0.5f;
            float angle = (pellets == 1) ? 0f : Random.Range(-half, half);
            Vector2 dir = Rotate(baseDir, angle);

            // ★確実にシーンに生成
            GameObject go = Instantiate(
                data.projectilePrefab.gameObject,
                firePoint.position,
                Quaternion.identity
            );

            Debug.Log($"[Weapon] Spawned projectile: {go.name} at {go.transform.position}");

            Projectile p = go.GetComponent<Projectile>();
            if (p == null)
            {
                Debug.LogError("[Weapon] Projectile component not found on prefab!");
                Destroy(go);
                return;
            }

            p.Init(dir, data.projectileSpeed, data.damage, owner, ownerTag, data.projectileLifeTime);
        }
    }

    static Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);
        return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y);
    }
}
