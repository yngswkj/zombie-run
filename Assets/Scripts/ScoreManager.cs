using Unity.VisualScripting;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Survival score")]
    public int survivalPointsPerSecond = 1;

    [Header("Kill score(HP Linked)")]
    public int baseKillScore = 5;
    public float hpScoreMultiplier = 2f;

    public int Score { get; private set; }
    public int KillCount { get; private set; }
    public int HighScore { get; private set; }

    float survivalAccum;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        HighScore = PlayerPrefs.GetInt("HIGH_SCORE", 0);
    }

    void Update()
    {
        // 生存時間スコア（秒加算）
        survivalAccum += Time.deltaTime * survivalPointsPerSecond;
        int add = Mathf.FloorToInt(survivalAccum);
        if (add > 0)
        {
            survivalAccum -= add;
            AddScore(add);
        }
    }

    // 敵撃破時スコア
    public void OnEnemyKilled(float enemyMaxHp)
    {
        KillCount++;

        int killScore = baseKillScore + Mathf.RoundToInt(enemyMaxHp * hpScoreMultiplier);
        AddScore(killScore);
    }

    // スコア加算処理
    void AddScore(int add)
    {
        Score += add;

        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt("HIGH_SCORE", HighScore);
            PlayerPrefs.Save();
        }
    }

    // ランリセット時処理
    public void ResetRun()
    {
        Score = 0;
        KillCount = 0;
        survivalAccum = 0;
    }

}
