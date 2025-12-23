using UnityEngine;
using UnityEngine.InputSystem;
using Spine.Unity; // Required for Spine

public class PlayerController25D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float jumpForce = 12f;
    
    [Header("Spine Animations")]
    public SkeletonAnimation skeletonAnimation;
    [SpineAnimation] public string idleAnim = "idle";
    [SpineAnimation] public string walkAnim = "walk";

    [Header("Camera")]
    public Vector3 cameraOffset = new Vector3(0, 3, -10);
    public float camSmoothSpeed = 0.12f;

    private Rigidbody rb;
    private Vector2 input;
    private Vector3 camVelocity = Vector3.zero;
    private Camera mainCam;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCam = Camera.main;
        
        // Ensure Rigidbody is set for 2.5D
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
    }

    void Update()
    {
        // 1. Get Input (Left/Right only)
        float h = 0;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) h = -1;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) h = 1;
        input = new Vector2(h, 0);

        // 2. Jump
        if (Keyboard.current.spaceKey.wasPressedThisFrame && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // 3. Flip Spine Character
        if (h != 0) skeletonAnimation.skeleton.ScaleX = (h > 0) ? 1 : -1;

        // 4. Set Animation
        string requiredAnim = (h != 0) ? walkAnim : idleAnim;
        if (skeletonAnimation.AnimationName != requiredAnim)
        {
            skeletonAnimation.state.SetAnimation(0, requiredAnim, true);
        }
    }

    void FixedUpdate()
    {
        // 5. Physics Move
        rb.linearVelocity = new Vector3(input.x * moveSpeed, rb.linearVelocity.y, 0);

        // 6. Camera Follow (Perfectly smooth)
        Vector3 targetPos = transform.position + cameraOffset;
        mainCam.transform.position = Vector3.SmoothDamp(mainCam.transform.position, targetPos, ref camVelocity, camSmoothSpeed);
    }
}