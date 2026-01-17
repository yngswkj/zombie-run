using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    public SpriteRenderer renderer;

    public Sprite idleSprite;
    public Sprite runSprite;
    public Sprite shootSprite;

    public float moveThreshold = 0.05f;
    public float shootShowTime = 0.12f;

    Rigidbody2D rb;
    float shootTimer;

    void Awake()
    {
        if (renderer == null) renderer = GetComponent<SpriteRenderer>();
        rb = GetComponentInParent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogWarning("PlayerVisual: Rigidbody2D not found in parent.");
        }
    }

    public void OnFired()
    {
        shootTimer = shootShowTime;
    }

    void Update()
    {
        if (renderer == null) return;

        // 撃った直後はshootを優先
        if (shootTimer > 0f)
        {
            shootTimer -= Time.deltaTime;
            if (shootSprite != null) renderer.sprite = shootSprite;
            return;
        }

        // 走り判定
        bool moving = rb.linearVelocity.magnitude > moveThreshold;
        if (moving)
        {
            if (runSprite != null) renderer.sprite = runSprite;
            return;
        }
        // 停止中
        else
        {
            if (idleSprite != null) renderer.sprite = idleSprite;
            return;
        }
    }
}
