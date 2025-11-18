using UnityEngine;
using UnityEngine.SceneManagement;

public class BoxCollisions : MonoBehaviour
{
    [Header("Crush Settings")]
    [SerializeField] private float downEpsilon = 0.01f;
    [SerializeField] private float verticalBias = 0.05f;
    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryCrushPlayer(collision);

        if (collision.collider.CompareTag("BreakableWall") && collision.relativeVelocity.magnitude >= 0.5f)
            Destroy(collision.collider.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryCrushPlayer(collision);

        if (collision.collider.CompareTag("BreakableWall") && collision.relativeVelocity.magnitude >= 0.5f)
            Destroy(collision.collider.gameObject);

    }

    private void TryCrushPlayer(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        bool movingDown = _rb.linearVelocity.y < -downEpsilon;
        float selfY = transform.position.y;
        float playerY = collision.collider.bounds.center.y;
        bool above = selfY > playerY + verticalBias;

        if (movingDown && above)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 0);
    }
}
