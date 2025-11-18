using UnityEngine;
using UnityEngine.SceneManagement;

public class Twump : MonoBehaviour
{
    [SerializeField] private Transform rotatingViewRoot;
    [SerializeField] private Transform spriteRoot;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _awakenSprite;
    [SerializeField] private ParticleSystem _particleSystem;

    [Header("Player Detection Ray")]
    [SerializeField] private float detectionRange = 3f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Ground Stick (optional)")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float minGroundNormalY = 0.5f;
    [SerializeField] private bool makeStaticWhenStuck = true;

    [Header("Crush Settings")]
    [SerializeField] private float downEpsilon = 0.01f;
    [SerializeField] private float verticalBias = 0.05f;

    private Rigidbody2D _rb;
    private bool _armed = false;
    private bool _isStuck = false;

    private void Start()
    {
        EventManager.AddListener("StartRotation", DisableDetection);
        EventManager.AddListener("EndRotation", EnableDetection);

        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener("StartRotation", DisableDetection);
        EventManager.RemoveListener("EndRotation", EnableDetection);
    }

    private void LateUpdate()
    {
        // Stop rotating once stuck
        if (_isStuck || !rotatingViewRoot || !spriteRoot) return;

        float counterZ = -rotatingViewRoot.eulerAngles.z;
        spriteRoot.localRotation = Quaternion.Euler(0f, 0f, counterZ);
    }

    private void Update()
    {
        if (_isStuck) return;

        // Raycast straight down to detect player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, detectionRange, playerLayer);

        if (hit.collider != null)
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            ArmAndDrop();
        }
        else
        {
            Debug.DrawRay(transform.position, Vector2.down * detectionRange, Color.yellow);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryCrushPlayer(collision);

        if (collision.collider.CompareTag("BreakableWall") && collision.relativeVelocity.magnitude >= 0.5f)
            Destroy(collision.collider.gameObject);

        if (!_isStuck && IsGroundCollision(collision))
            StickToGround();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryCrushPlayer(collision);

        if (collision.collider.CompareTag("BreakableWall") && collision.relativeVelocity.magnitude >= 0.5f)
            Destroy(collision.collider.gameObject);

        if (!_isStuck && IsGroundCollision(collision))
            StickToGround();
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

    private void DisableDetection()
    {
        _armed = true;
    }
    private void EnableDetection() 
    {
        if (!_isStuck)
            _armed = false;
    }

    private bool IsGroundCollision(Collision2D collision)
    {
        bool isGroundLayer = (groundLayer.value & (1 << collision.collider.gameObject.layer)) != 0;
        if (!isGroundLayer) return false;

        if (collision.contactCount > 0)
            return collision.GetContact(0).normal.y >= minGroundNormalY;

        return true;
    }

    private void StickToGround()
    {
        _isStuck = true;
        _armed = false;
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;

        // Fully freeze in place without rotation updates
        if (makeStaticWhenStuck)
            _rb.bodyType = RigidbodyType2D.Static;
        else
            _rb.constraints = RigidbodyConstraints2D.FreezeAll;

        Debug.Log("[Twump] Stuck to ground");
    }

    private void ArmAndDrop()
    {
        if (_armed) return;

        _armed = true;
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        _spriteRenderer.sprite = _awakenSprite;
        _particleSystem.Play();
        Debug.Log("[Twump] Dropping");
    }
}
