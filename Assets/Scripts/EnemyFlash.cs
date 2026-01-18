using UnityEngine;

public class EnemyFlash : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    static readonly int FlashAmountId = Shader.PropertyToID("_FlashAmount");

    MaterialPropertyBlock mpb;
    float currentFlashAmount;

    [Header("Flash settings")]
    public float flashSpeed = 5f; // フラッシュの速さ

    void Awake()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
    }

    public void PlayHitFlash(float peak = 1f)
    {
        currentFlashAmount = peak;
        UpdateFlash(currentFlashAmount);
    }

    void Update()
    {
        // 白さが残っているなら、時間をかけて0に戻していく
        if (currentFlashAmount > 0f)
        {
            currentFlashAmount -= Time.deltaTime * flashSpeed;

            if (currentFlashAmount < 0f) currentFlashAmount = 0f;

            UpdateFlash(currentFlashAmount);
        }
    }

    void UpdateFlash(float amount)
    {
        if (sr == null) return;

        sr.GetPropertyBlock(mpb);
        mpb.SetFloat(FlashAmountId, amount);
        sr.SetPropertyBlock(mpb);
    }
}