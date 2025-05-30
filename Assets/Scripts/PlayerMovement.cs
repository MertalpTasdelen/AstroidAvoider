using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{
    [SerializeField] private float forceMagnitude;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private GameObject playerFlame;


    private Rigidbody rb;
    private Camera mainCamera;
    private Vector3 movementDirection;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        playerFlame.SetActive(movementDirection != Vector3.zero);

        ProcessInput();

        KeepPlayerOnScreen();

        RotatePlayerFace();
    }

    void FixedUpdate()
    {
        if (movementDirection == Vector3.zero) { return; }

        rb.AddForce(movementDirection * forceMagnitude, ForceMode.Force);

        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxVelocity);
    }

    public Vector3 GetCurrentMovementDirection()
    {
        return rb.linearVelocity.normalized;
    }
    private void ProcessInput()
    {
        if (Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            movementDirection = worldPosition - transform.position;
            movementDirection.z = 0f;
            movementDirection.Normalize();

        }
        else
        {
            movementDirection = Vector3.zero;
        }
    }

    private void KeepPlayerOnScreen()
    {
        Vector3 newPosition = transform.position;

        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

        if (viewportPosition.x > 1)
        {
            newPosition.x = -newPosition.x + 0.1f;
        }
        if (viewportPosition.x < 0)
        {
            newPosition.x = -newPosition.x - 0.1f;
        }
        if (viewportPosition.y > 1)
        {
            newPosition.y = -newPosition.y + 0.1f;
        }
        if (viewportPosition.y < 0)
        {
            newPosition.y = -newPosition.y - 0.1f;
        }

        transform.position = newPosition;

    }

    private void RotatePlayerFace()
    {
        if (rb.linearVelocity == Vector3.zero) { return; }

        Quaternion targetRotation = Quaternion.LookRotation(rb.linearVelocity, Vector3.back);

        transform.rotation = Quaternion.Lerp(
            transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
