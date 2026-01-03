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
    public float groundCheckDistance = 0.5f;

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

        // Physics Setup
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    void Update()
    {
        // 1. INPUT: D is Right (+1), A is Left (-1)
        moveInput = 0;
        if (Keyboard.current.aKey.isPressed) moveInput = -1; 
        if (Keyboard.current.dKey.isPressed) moveInput = 1;  

        // 2. STABLE GROUND CHECK
        // We use a Raycast with a small offset to ensure it hits the 3D mesh correctly
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, groundCheckDistance, groundLayer);

        // 3. JUMP LOGIC
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }

        // 4. INTERACTION
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
        // Move the player physically
        rb.linearVelocity = new Vector3(moveInput * moveSpeed, rb.linearVelocity.y, rb.linearVelocity.z);
    }

    void HandleVisuals()
    {
        if (spine == null) return;

        // FIXED FLIPPING LOGIC: Ensuring ScaleX matches direction exactly
        if (moveInput > 0) spine.skeleton.ScaleX = -1; // Facing Right
        else if (moveInput < 0) spine.skeleton.ScaleX = 1; // Facing Left

        // ANIMATION STATE MACHINE
        string targetAnim = idleAnim;

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

        // Play animation only if it's not already playing
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