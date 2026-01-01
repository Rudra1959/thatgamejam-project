using UnityEngine;
using UnityEngine.InputSystem;
using Spine.Unity;

[RequireComponent(typeof(Rigidbody), typeof(PlayerStats), typeof(InventoryManager))]
public class PlayerController25D : MonoBehaviour
{
    [Header("Movement Physics")]
    public float moveSpeed = 8f;
    public float jumpForce = 14f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.4f;

    [Header("Spine Visuals")]
    public SkeletonAnimation spine;
    public bool flipInitialDirection = false; 
    [SpineAnimation] public string walkAnim;
    [SpineAnimation] public string jumpAnim;
    [SpineAnimation] public string idleAnim;

    [Header("Camera Settings")]
    public Vector3 camOffset = new Vector3(0, 5, -12);
    public float camSmoothTime = 0.15f;

    private Rigidbody rb;
    private float moveInput;
    private Vector3 camVelocity;
    private bool isGrounded;
    private InventoryManager inv;

    void Start() {
        rb = GetComponent<Rigidbody>();
        inv = GetComponent<InventoryManager>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        
        if (spine == null) spine = GetComponentInChildren<SkeletonAnimation>();
    }

    void Update() {
        // 1. Movement Input
        moveInput = 0;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveInput = -1;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveInput = 1;

        // 2. Jumping Logic
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance, groundLayer);
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded) {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, 0);
        }

        // 3. Survival Interactions
        if (Keyboard.current.eKey.wasPressedThisFrame) HandleManualPickup();
        if (Keyboard.current.rKey.wasPressedThisFrame) HandleFireLighting();
        if (Keyboard.current.qKey.wasPressedThisFrame) inv.ToggleInventory();

        HandleVisuals();
    }

    void HandleManualPickup() {
        Collider[] items = Physics.OverlapSphere(transform.position, 2f);
        foreach (var hit in items) {
            if (hit.CompareTag("Pickup")) {
                hit.GetComponent<ItemPickup>()?.Pickup();
                break; 
            }
        }
    }

    void HandleFireLighting() {
        Collider[] hits = Physics.OverlapSphere(transform.position, 2f);
        foreach (var hit in hits) {
            if (hit.CompareTag("FirePit")) {
                if (inv.sticks >= 3) {
                    hit.GetComponent<Campfire>()?.AttemptToLight();
                    inv.sticks -= 3;
                } else {
                    Debug.Log("Not sufficient sticks! You need 3.");
                }
            }
        }
    }

    void HandleVisuals() {
        if (moveInput != 0) {
            float direction = moveInput;
            if (flipInitialDirection) direction *= -1;
            spine.skeleton.ScaleX = direction > 0 ? 1 : -1;
        }

        if (!isGrounded) {
            UpdateSpine(jumpAnim, 1f, true); 
        } else if (moveInput != 0) {
            UpdateSpine(walkAnim, 1f, true); 
        } else {
            if (!string.IsNullOrEmpty(idleAnim)) {
                UpdateSpine(idleAnim, 1f, true); 
            } else {
                UpdateSpine(walkAnim, 0f, true); // Freeze walk as Idle
            }
        }
    }

    void UpdateSpine(string name, float speed, bool loop) {
        if (string.IsNullOrEmpty(name)) return;
        spine.timeScale = speed;
        if (spine.AnimationName != name) {
            spine.AnimationState.SetAnimation(0, name, loop);
        }
    }

    void FixedUpdate() {
        rb.velocity = new Vector3(moveInput * moveSpeed, rb.velocity.y, 0);
        Vector3 targetPos = transform.position + camOffset;
        Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, targetPos, ref camVelocity, camSmoothTime);
        Camera.main.transform.LookAt(transform.position + Vector3.up * 1.5f);
    }
}