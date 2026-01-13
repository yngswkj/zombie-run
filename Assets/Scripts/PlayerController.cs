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
    void Update()
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
        bool isThrusting = Mouse.current.leftButton.isPressed;

        if (isThrusting)
        {
            // マウス位置を計算
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            Vector2 direction = (mousePos - transform.position).normalized;

            // プレイヤーをマウス方向に向ける
            // transform.up = direction;
            rb.AddForce(direction * thrustForce);

            // 最高速度の制限
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }

            // 左右判定
            if (Mathf.Abs(direction.x) > 0.01f)
            {
                isFacingRight = direction.x > 0f;
            }

            // エフェクトの表示切替
            if (runEffectRight) runEffectRight.SetActive(!isFacingRight);
            if (runEffectLeft) runEffectLeft.SetActive(isFacingRight);
        }
        else
        {
            // 推していない時はエフェクトを非表示
            if (runEffectRight) runEffectRight.SetActive(false);
            if (runEffectLeft) runEffectLeft.SetActive(false);
        }
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
