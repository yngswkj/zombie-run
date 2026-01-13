using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float minSize = 0.1f;
    public float maxSize = 0.5f;
    public float minSpeed = 150f;
    public float maxSpeed = 300f;
    public float maxSpinSpeed = 10f;
    public GameObject bounceEffectPrefab;

    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float randomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomSize, randomSize, 1);

        float randomSpeed = Random.Range(minSpeed, maxSpeed) / randomSize;
        Vector2 randomDirection = Random.insideUnitCircle;

        // float randomTorque = Random.Range(-maxSpinSpeed, maxSpinSpeed);

        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(randomDirection * randomSpeed);
        // rb.AddTorque(randomTorque);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 contactPoint = collision.GetContact(0).point;
        GameObject bounceEffect = Instantiate(bounceEffectPrefab, contactPoint, Quaternion.identity);
        Destroy(bounceEffect, 1f);
    }
}
