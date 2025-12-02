using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float speed = 5f;

    [Header("Chão")]
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private Transform groundCheck;

    [Header("Visual")]
    [SerializeField] private bool flipSprite = true;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator; // opcional (bool "Climbing", bool "Walk")
    [SerializeField] private GameObject _dust;

    [Header("Escada")]
    [SerializeField] private string ladderTag = "Ladder";
    [SerializeField] private float climbSpeed = 3.5f;   // vel. de subida/descida
    [SerializeField] private float snapXSpeed = 18f;    // força de alinhamento ao centro da escada

    private Rigidbody2D rb;
    private Collider2D col;
    private Vector2 input;
    private float inputY;
    private bool grounded;
    private int groundContacts;

    // --- failsafe ---
    private float groundCheckTimer = 0f;
    private float midairTimer = 0f;

    // --- escada ---
    private bool climbing = false;
    private Collider2D currentLadder = null;
    private float originalGravity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        if (!sprite) sprite = GetComponentInChildren<SpriteRenderer>();

        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = true;

        originalGravity = rb.gravityScale;

        _dust.SetActive(false);
        EventManager.AddListener("StartRotation", OnLevelRotation);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener("StartRotation", OnLevelRotation);
    }

    private void OnLevelRotation()
    {
        grounded = false;
        groundContacts = 0;
        groundCheckTimer = 0f;
        rb.linearVelocity = Vector2.zero;

        // sair da escada se estiver escalando
        if (climbing) StopClimbing();
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical"); // lê aqui para usar no FixedUpdate
        input = new Vector2(x, 0f);

        // se estiver escalando, não flipar com x
        if (!climbing && flipSprite && sprite && Mathf.Abs(x) > 0.01f)
            sprite.flipX = x * Mathf.Sign(speed) < 0f;

        // --- Atualiza animações ---
        if (animator)
        {
            bool isWalking =        Mathf.Abs(x) > 0.01f && !climbing;
            animator.SetBool("Walk", isWalking);
            animator.SetBool("Climbing", climbing && Mathf.Abs(inputY) > 0.01f);
        }

        if (_dust)
        {
            bool movingHoriz = Mathf.Abs(input.x) > 0.01f;

            if (grounded && movingHoriz && !climbing)
            {
                if (!_dust.activeSelf)
                    _dust.SetActive(true);
            }
            else
            {
                if (_dust.activeSelf)
                    _dust.SetActive(false);
            }
        }
    }

    private void FixedUpdate()
    {
        // -------- FAILSAFE: usando GroundCheck transform --------
        if (groundCheck)
        {
            bool touching = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundMask);
            if (touching)
            {
                groundCheckTimer += Time.fixedDeltaTime;
                if (groundCheckTimer >= 0.5f && !grounded)
                {
                    grounded = true;
                    midairTimer = 0f;
                    EventManager.TriggerEvent("PlayerHitGround");
                }
            }
            else
            {
                groundCheckTimer = 0f;
                midairTimer += Time.fixedDeltaTime;
                if (midairTimer >= 0.1f && !grounded)
                {
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                }
            }
        }
        // -------------------------------------------------------

        // LÓGICA DE ESCADA
        if (climbing && currentLadder)
        {
            rb.linearVelocity = Vector2.zero;

            Bounds l = currentLadder.bounds;
            float halfH = col.bounds.size.y / 2;

            float minY = l.min.y + halfH;
            float maxY = l.max.y - halfH;

            float nextY = Mathf.Clamp(
                transform.position.y + inputY * climbSpeed * Time.fixedDeltaTime,
                minY, maxY
            );

            float nextX = Mathf.Lerp(
                transform.position.x,
                l.center.x,
                Time.fixedDeltaTime * snapXSpeed
            );

            transform.position = new Vector3(nextX, nextY, transform.position.z);

            if (Mathf.Approximately(nextY, maxY) && inputY > 0.01f)
                StopClimbing();
            if (Mathf.Approximately(nextY, minY) && inputY < -0.01f)
                StopClimbing();

            return;
        }

        // -------- WALL CHECK --------
        float xDir = Mathf.Sign(input.x*speed);
        float rayDistance = 0.6f; // how far ahead to check
        if (Mathf.Abs(input.x) > 0.01f)
        {
            Vector2 origin = (Vector2)transform.position + Vector2.up * (col.bounds.size.y * 0.25f);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * xDir, rayDistance, groundMask);
            if (hit.collider != null)
            {
                // wall detected → cancel horizontal movement
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                return;
            }
        }

        // Movimento horizontal normal
        float targetX = input.x * speed;
        rb.linearVelocity = new Vector2(targetX, rb.linearVelocity.y);
    }

    // ====== DETECÇÃO DE CHÃO VIA CALLBACKS ======
    private void OnCollisionEnter2D(Collision2D c)
    {
        if (!IsGroundLayer(c.collider.gameObject.layer)) return;
        if (c.collider.isTrigger) return;

        int prev = groundContacts;
        groundContacts++;

        if (prev == 0 && groundContacts > 0)
        {
            grounded = true;
            groundCheckTimer = 0f;
            EventManager.TriggerEvent("PlayerHitGround");
        }
    }

    private void OnCollisionExit2D(Collision2D c)
    {
        if (!IsGroundLayer(c.collider.gameObject.layer)) return;
        if (c.collider.isTrigger) return;

        groundContacts = Mathf.Max(groundContacts - 1, 0);
        if (groundContacts == 0)
            grounded = false;
    }

    private bool IsGroundLayer(int layer)
    {
        return (groundMask.value & (1 << layer)) != 0;
    }

    // ====== TRIGGERS DA ESCADA ======
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger) return;
        if (!other.CompareTag(ladderTag)) return;

        // escolhe a mais próxima se já houver uma
        if (currentLadder == null)
        {
            currentLadder = other;
        }
        else
        {
            float dNew = Mathf.Abs(other.bounds.center.x - transform.position.x);
            float dOld = Mathf.Abs(currentLadder.bounds.center.x - transform.position.x);
            if (dNew < dOld) currentLadder = other;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.isTrigger) return;
        if (!other.CompareTag(ladderTag)) return;
        if (!grounded) return;

        Bounds l = other.bounds;
        float halfH = col.bounds.size.y;
        float minY = l.min.y + halfH;
        float maxY = l.max.y - halfH;

        const float EPS = 0.02f;
        float y = transform.position.y;

        bool atBase = y <= (minY + EPS);
        bool atTop = y >= (maxY - EPS);

        if (!climbing && Mathf.Abs(inputY) > 0.01f)
        {
            if (inputY < 0f && atBase) return;
            if (inputY > 0f && atTop) return;

            currentLadder = other;
            StartClimbing();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.isTrigger) return;
        if (!other.CompareTag(ladderTag)) return;

        if (other == currentLadder)
        {
            currentLadder = null;
            if (climbing) StopClimbing();
        }
    }

    // ====== CONTROLES DE ESTADO DA ESCALADA ======
    private void StartClimbing()
    {
        climbing = true;
        grounded = false;
        col.isTrigger = true;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
    }

    private void StopClimbing()
    {
        climbing = false;
        col.isTrigger = false;
        rb.gravityScale = originalGravity;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

        if (animator)
        {
            animator.SetBool("Climbing", false);
            animator.SetBool("Walk", false);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
        }
    }
#endif
}
