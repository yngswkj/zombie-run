using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Projectile")]
    public Projectile projectilePrefab;   // 発射する弾丸のプレハブ
    public float projectileSpeed = 12f;   // 弾丸の速度
    public float damage = 1f;             // 弾丸のダメージ
    public float projectileLifeTime = 3f; // 弾丸の寿命（秒）

    [Header("Fire")]
    public float fireRate = 6f;           // 1秒あたりの発射数
    public int projectilesPerShot = 1;    // 1回の発射で出る弾丸の数
    public float spreadAngleDeg = 0f;     // 弾丸の拡散角度（度）

    [Header("Burst (optional)")]
    public int burstCount = 1;            // バースト発射の弾丸数
    public float burstInterval = 0.06f;   // バースト内の弾丸間隔（秒）

}
