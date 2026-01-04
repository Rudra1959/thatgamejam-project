using UnityEngine;
using UnityEngine.InputSystem;
using Spine.Unity;

[RequireComponent(typeof(Rigidbody), typeof(InventoryManager))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public LayerMask groundLayer;
    
    [Tooltip("Size of the circle checking for ground. Increase if jittering on bridge.")]
    public float groundCheckRadius = 0.3f;
    public float groundCheckDistance = 0.4f;

    [Header("Spine Visuals")]
    public SkeletonAnimation spine;
    [SpineAnimation] public string walkAnim;
    [SpineAnimation] public string idleAnim;
    [SpineAnimation] public string jumpAnim;

    [Header("Interactions")]
    public GameObject interactPrompt; 

    private Rigidbody rb;
    private InventoryManager inv;
    private float moveInput;
    private bool isGrounded;
    private ItemPickup currentItem;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        inv = GetComponent<InventoryManager>();

        // Physics Setup: Ensure Z is locked so we don't fall off the 3D path
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    void Update()
    {
        // 1. INPUT
        moveInput = 0;
        if (Keyboard.current.aKey.isPressed) moveInput = -1; 
        if (Keyboard.current.dKey.isPressed) moveInput = 1;  

        // 2. STICKY GROUND CHECK (Using SphereCast for Bridge/Mountain Edges)
        // This checks a wide area so you don't 'jitter' over small gaps
        isGrounded = Physics.SphereCast(transform.position + Vector3.up * 0.5f, groundCheckRadius, Vector3.down, out _, groundCheckDistance + 0.5f, groundLayer);

        // 3. JUMP
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }

        // 4. INTERACTION (E)
        if (currentItem != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            currentItem.Pickup();
            currentItem = null;
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }

        HandleVisuals();
    }

    void FixedUpdate()
    {
        // 5. MOVEMENT: Smooth linear velocity
        rb.linearVelocity = new Vector3(moveInput * moveSpeed, rb.linearVelocity.y, rb.linearVelocity.z);
    }

    void HandleVisuals()
    {
        if (spine == null) return;

        // 6. FLIPPING: Fixed to match your specific Spine Export direction
        if (moveInput > 0) spine.skeleton.ScaleX = -1; // Facing Right
        else if (moveInput < 0) spine.skeleton.ScaleX = 1; // Facing Left

        // 7. ANIMATION STATE MACHINE
        string targetAnim;

        if (!isGrounded)
        {
            targetAnim = jumpAnim;
        }
        else if (moveInput != 0)
        {
            targetAnim = walkAnim;
        }
        else
        {
            targetAnim = idleAnim;
        }

        if (spine.AnimationName != targetAnim)
        {
            spine.AnimationState.SetAnimation(0, targetAnim, true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            currentItem = other.GetComponent<ItemPickup>();
            if (interactPrompt != null) interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            currentItem = null;
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }
    }
}