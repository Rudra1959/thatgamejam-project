using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public bool isIsometric = true;

    [Header("Camera")]
    public Camera mainCamera;
    public float cameraSmoothTime = 0.15f;
    public Vector3 isoOffset = new Vector3(-8, 8, -8);
    public Vector3 sideOffset = new Vector3(0, 3, -10);

    private Rigidbody rb;
    private Vector3 moveDirection;
    private Vector3 camVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (!mainCamera)
            mainCamera = Camera.main;
    }

    void Update()
    {
        float h = 0;
        float v = 0;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) v = 1;
            if (Keyboard.current.sKey.isPressed) v = -1;
            if (Keyboard.current.aKey.isPressed) h = -1;
            if (Keyboard.current.dKey.isPressed) h = 1;
        }

        if (isIsometric)
            moveDirection = new Vector3(h - v, 0, h + v).normalized;
        else
            moveDirection = new Vector3(h, 0, 0).normalized;
    }

    void FixedUpdate()
    {
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, 0.2f));
        }
    }

    void LateUpdate()
    {
        Vector3 targetCamPos = transform.position + (isIsometric ? isoOffset : sideOffset);
        mainCamera.transform.position =
            Vector3.SmoothDamp(mainCamera.transform.position, targetCamPos, ref camVelocity, cameraSmoothTime);

        mainCamera.transform.LookAt(transform.position + Vector3.up);
    }
}
