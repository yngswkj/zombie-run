using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // プレイヤーの速度関連変数
    public float thrustForce = 1f;
    public float maxSpeed = 5f;
    public GameObject runEffectRight;
    public GameObject runEffectLeft;
    private bool isFacingRight = true;
    public float moveSpeed = 5f;
    public float accel = 30f;
    public float deadZone = 0.05f;

    // スコア関連変数
    private float elapsedTime = 0f;
    private float score = 0f;
    public float scoreMutiplier = 10f;

    // UI関連変数
    public UIDocument uiDocument;
    private Label scoreText;
    private Button restartButton;
    private Label highScoreText;

    // エフェクト関連変数
    public GameObject explosionEffect;

    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // UI要素の取得
        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
        highScoreText = uiDocument.rootVisualElement.Q<Label>("HighScoreLabel");

        // ハイスコアの表示
        int highScore = PlayerPrefs.GetInt("highScore", 0);
        if (highScoreText != null)
        {
            highScoreText.text = "ハイスコア: " + highScore;
        }

        restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");
        restartButton.style.display = DisplayStyle.None;
        restartButton.clicked += ReloadScene;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateScore();
        MovePlayer();
    }

    // スコア更新処理  
    void UpdateScore()
    {
        // 経過時間を計算
        elapsedTime += Time.deltaTime;

        // 経過時間にスコア乗数をかけてスコア算出
        score = Mathf.FloorToInt(elapsedTime * scoreMutiplier);

        // UIにスコア表示
        scoreText.text = "スコア: " + score;
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

        // 走行エフェクト（移動ベクトルで左右判定）
        Vector2 v = rb.linearVelocity;

        if (Mathf.Abs(v.x) > deadZone)
            isFacingRight = v.x > 0f;

        bool isMoving = v.magnitude > deadZone;

        // エフェクトの表示切替
        if (runEffectRight) runEffectRight.SetActive(!isFacingRight && isMoving);
        if (runEffectLeft) runEffectLeft.SetActive(isFacingRight && isMoving);

        // 推していない時はエフェクトを非表示
        if (runEffectRight) runEffectRight.SetActive(false);
        if (runEffectLeft) runEffectLeft.SetActive(false);
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
            Debug.Log("New High Score: " + currentScore);
        }

        Instantiate(explosionEffect, transform.position, transform.rotation);
        restartButton.style.display = DisplayStyle.Flex;
        Destroy(gameObject);
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
