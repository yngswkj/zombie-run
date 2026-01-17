using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("Assign")]
    public Transform fill;              // HPBarFillRoot を入れる
    public SpriteRenderer fillRenderer; // HPBarFill の SpriteRenderer（色変えるなら）

    [Header("Tuning")]
    public bool keepWorldSize = true;
    public float showSecondsAfterHit = 0f;

    float hideTimer;
    Transform ownerRoot;

    // ★満タン時の情報を自動で記憶
    float fullScaleX;
    float fullPosX;

    void Awake()
    {
        if (fill == null) return;

        fullScaleX = fill.localScale.x;
        fullPosX = fill.localPosition.x;
    }

    public void BindOwner(Transform owner)
    {
        ownerRoot = owner;
    }

    void LateUpdate()
    {
        if (keepWorldSize && ownerRoot != null)
        {
            Vector3 s = ownerRoot.lossyScale;
            float ix = s.x != 0 ? 1f / s.x : 1f;
            float iy = s.y != 0 ? 1f / s.y : 1f;
            transform.localScale = new Vector3(ix, iy, 1f);
        }

        if (showSecondsAfterHit > 0f)
        {
            hideTimer -= Time.deltaTime;
            gameObject.SetActive(hideTimer > 0f);
        }
    }

    public void SetNormalized(float t)
    {
        t = Mathf.Clamp01(t);
        if (fill == null) return;

        // 1) 幅を満タン基準で縮める
        var s = fill.localScale;
        s.x = fullScaleX * t;
        fill.localScale = s;

        // 2) 左端固定：縮めた分だけXを左にずらす（＝右から減る）
        // 左端を一定にしたいので: pos.x = fullPosX - (fullScaleX - newScaleX)/2
        var p = fill.localPosition;
        p.x = fullPosX - (fullScaleX - s.x) * 0.5f;
        fill.localPosition = p;

        if (showSecondsAfterHit > 0f)
        {
            gameObject.SetActive(true);
            hideTimer = showSecondsAfterHit;
        }

        // 色変化（赤→黄→緑）
        if (fillRenderer != null)
        {
            if (t < 0.25f)
                fillRenderer.color = Color.red;
            else if (t < 0.5f)
                fillRenderer.color = Color.yellow;
            else
                fillRenderer.color = Color.green;
        }
    }
}
