using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerController : MonoBehaviour
{
    // プレイヤーの速度関連変数
    public float thrustForce = 1f;
    public float maxSpeed = 5f;
    public float moveSpeed = 5f;
    public float accel = 30f;
    public float deadZone = 0.05f;

    // スコア関連変数
    private float score = 0f;
    public float scoreMutiplier = 10f;

    // UI関連変数
    public UIDocument uiDocument;
    private Label scoreText;
    private Button restartButton;
    private Label highScoreText;
    private Label killText;

    PlayerHealth health;
    bool deathHandeld = false;
    Collider2D col;
    SpriteRenderer sr;

    // エフェクト関連変数
    public GameObject explosionEffect;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        health = GetComponent<PlayerHealth>();
        col = GetComponent<Collider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        // UI要素の取得
        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
        killText = uiDocument.rootVisualElement.Q<Label>("KillLabel");
        highScoreText = uiDocument.rootVisualElement.Q<Label>("HighScoreLabel");

        // ハイスコアの表示
        if (highScoreText != null)
        {
            highScoreText.text = "ハイスコア: " + ScoreManager.Instance.HighScore.ToString();
        }

        restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");
        restartButton.style.display = DisplayStyle.None;
        restartButton.clicked += ReloadScene;
    }

    void FixedUpdate()
    {
        UpdateScore();

        if (health != null && health.IsDead)
        {
            if (!deathHandeld) HandleDeathOnce();
            return;
        }

        MovePlayer();
    }

    // スコア更新処理  
    void UpdateScore()
    {
        // UIにスコア表示
        scoreText.text = "スコア: " + ScoreManager.Instance.Score.ToString();
        killText.text = "キル数: " + ScoreManager.Instance.KillCount.ToString();
    }

    // プレイヤー移動処理
    void MovePlayer()
    {

        // WASD入力
        float x = 0f;
        float y = 0f;

        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.aKey.isPressed) x -= 1f;
        if (kb.dKey.isPressed) x += 1f;
        if (kb.sKey.isPressed) y -= 1f;
        if (kb.wKey.isPressed) y += 1f;

        Vector2 input = new Vector2(x, y);
        if (input.sqrMagnitude > 1f) input.Normalize();

        // 目標速度
        Vector2 targetVel = input * moveSpeed;

        // 現在速度を目標へじわっと寄せる
        rb.linearVelocity = Vector2.MoveTowards(
            rb.linearVelocity,
            targetVel,
            accel * Time.fixedDeltaTime
        );
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        // ハイスコア更新処理
        int currentScore = (int)score;
        int highScore = PlayerPrefs.GetInt("highScore", 0);

        if (currentScore > highScore)
        {
            // ハイスコア更新
            PlayerPrefs.SetInt("highScore", currentScore);
            PlayerPrefs.Save();

            // ハイスコアUI更新
            if (highScoreText != null)
            {
                highScoreText.text = "ハイスコア: " + currentScore;
            }
        }
    }

    void HandleDeathOnce()
    {
        deathHandeld = true;

        if (rb) rb.linearVelocity = Vector2.zero;
        if (col) col.enabled = false;  // 衝突無効化
        if (sr) sr.enabled = false;    // 見た目だけ消す

        if (explosionEffect) Instantiate(explosionEffect, transform.position, transform.rotation);
        if (restartButton != null) restartButton.style.display = DisplayStyle.Flex;
    }

    void ReloadScene()
    {
        ScoreManager.Instance.ResetRun();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
