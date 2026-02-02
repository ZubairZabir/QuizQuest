using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Sprite[] sprites;
    public float strength = 5f;
    public float gravity = -9.81f;
    public float tilt = 5f;

    private SpriteRenderer spriteRenderer;
    private Vector3 direction;
    private int spriteIndex;
    private Keyboard keyboard;
    private Mouse mouse;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Ensure Player has required components for collision detection
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f; // We handle gravity manually
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent physics rotation
            Debug.LogWarning("Player missing Rigidbody2D - added automatically. Make sure to set gravity scale to 0 in Inspector.");
        }
        
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) {
            CircleCollider2D circleCol = gameObject.AddComponent<CircleCollider2D>();
            circleCol.isTrigger = false; // Player should have non-trigger collider
            Debug.LogWarning("Player missing Collider2D - added CircleCollider2D automatically.");
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
        keyboard = Keyboard.current;
        mouse = Mouse.current;
    }

    private void OnEnable()
    {
        Vector3 position = transform.position;
        position.y = 0f;
        transform.position = position;
        direction = Vector3.zero;
    }

    private void Update()
    {
        // Use new Input System
        bool jumpPressed = false;
        if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame) {
            jumpPressed = true;
        }
        if (mouse != null && mouse.leftButton.wasPressedThisFrame) {
            jumpPressed = true;
        }
        
        if (jumpPressed) {
            direction = Vector3.up * strength;
        }

        // Apply gravity and update the position
        direction.y += gravity * Time.deltaTime;
        transform.position += direction * Time.deltaTime;

        // Tilt the bird based on the direction
        Vector3 rotation = transform.eulerAngles;
        rotation.z = direction.y * tilt;
        transform.eulerAngles = rotation;
    }

    private void AnimateSprite()
    {
        spriteIndex++;

        if (spriteIndex >= sprites.Length) {
            spriteIndex = 0;
        }

        if (spriteIndex < sprites.Length && spriteIndex >= 0) {
            spriteRenderer.sprite = sprites[spriteIndex];
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance == null) {
            Debug.LogWarning("GameManager.Instance is null! Make sure GameManager is in the scene.");
            return;
        }

        if (other.gameObject.CompareTag("Obstacle")) {
            Debug.Log("Hit Obstacle!");
            GameManager.Instance.GameOver();
        } else if (other.gameObject.CompareTag("Scoring")) {
            // Check if this scoring zone has already been scored
            ScoringZoneTracker tracker = other.GetComponent<ScoringZoneTracker>();
            if (tracker != null && !tracker.HasScored()) {
                Debug.Log("Scored! Current score: " + (GameManager.Instance.GetScore() + 1));
                GameManager.Instance.IncreaseScore();
                tracker.MarkAsScored();
            }
        } else {
            Debug.Log("Trigger entered with: " + other.gameObject.name + " (Tag: " + other.gameObject.tag + ")");
        }
    }

}
