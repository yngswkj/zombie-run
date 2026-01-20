using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject enemyPrefab;

    [Header("Spawn Area")]
    public Collider2D spawnArea;

    [Header("Refs")]
    public Transform player;
    public Camera targetCamera;

    [Header("Spawn(Outside Camera)")]
    public float outsideMargin = 2.0f;
    public float minDistanceFromPlayer = 2.0f;

    [Header("Start Spawn")]
    public int initialCount = 6;

    [Header("Over Time Spawn")]
    public bool spawnOverTime = true;
    public float spawnInterval = 2f;  // 何秒ごとにスポーンするか
    public int maxAlive = 25;         // 最大同時出現数

    [Header("Difficulty Ramp")]
    public float rampSeconds = 120f;  // 難易度上昇にかかる時間
    public float startInterval = 2f;  // 開始時のスポーン間隔
    public float minInterval = 0.5f;  // 最小スポーン間隔
    public int startMaxAlive = 10;    // 開始時の最大同時出現数
    public int maxAliveAtMax = 40;    // 最大難易度時の最大同時出現数
    public AnimationCurve difficultyCurve = AnimationCurve.Linear(0, 0, 1, 1);

    float timer;

    void Start()
    {
        if (enemyPrefab == null)
        {
            return;
        }

        if (targetCamera == null) targetCamera = Camera.main;

        // 初期スポーン
        for (int i = 0; i < initialCount; i++)
            TrySpawnOutSideCamera();
    }

    void Update()
    {
        if (!spawnOverTime) return;

        float t = Mathf.Clamp01(Time.timeSinceLevelLoad / rampSeconds);
        float d = difficultyCurve.Evaluate(t);

        spawnInterval = Mathf.Lerp(startInterval, minInterval, d);
        maxAlive = Mathf.RoundToInt(Mathf.Lerp(startMaxAlive, maxAliveAtMax, d));

        timer += Time.deltaTime;
        if (timer < spawnInterval) return;
        timer = 0f;

        // 増えすぎ防止
        int alive = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (alive >= maxAlive) return;

        TrySpawnOutSideCamera();
    }

    void TrySpawnOutSideCamera()
    {
        // スポーン位置決定
        Vector2 pos;
        int guard = 0;

        do
        {
            pos = GetRandomPointOutsideCamera(targetCamera, outsideMargin);
            guard++;
            if (guard > 30) break; // 無限ループ防止
        }
        while (player != null && Vector2.Distance(pos, player.position) < minDistanceFromPlayer);

        Instantiate(enemyPrefab, pos, Quaternion.identity);
    }

    static Vector2 GetRandomPointOutsideCamera(Camera cam, float margin)
    {
        // Orthographic 前提（2Dなら通常OK）
        float h = cam.orthographicSize;
        float w = h * cam.aspect;

        Vector3 c = cam.transform.position;
        float left = c.x - w;
        float right = c.x + w;
        float bottom = c.y - h;
        float top = c.y + h;

        int side = Random.Range(0, 4);
        switch (side)
        {
            case 0: // Left
                return new Vector2(left - margin, Random.Range(bottom, top));
            case 1: // Right
                return new Vector2(right + margin, Random.Range(bottom, top));
            case 2: // Bottom
                return new Vector2(Random.Range(left, right), bottom - margin);
            default: // Top
                return new Vector2(Random.Range(left, right), top + margin);
        }
    }
}
