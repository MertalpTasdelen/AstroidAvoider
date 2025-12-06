using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{
    [SerializeField] private float forceMagnitude;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private GameObject playerFlame;
    [SerializeField] private DynamicJoystick joystick;


    private Rigidbody rb;
    private Camera mainCamera;
    private Vector3 movementDirection;
    private Vector3 lastKnownDirection = Vector3.zero;
    private float inputIntensity = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;


        // 2D kilitler - sadece Z pozisyonunu kilitle, rotasyonları serbest bırak
        rb.constraints = RigidbodyConstraints.FreezePositionZ;

        // emin olmak için z'yi 0'a çak
        var p = transform.position; p.z = 0f; transform.position = p;
    }


    void Update()
    {
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;
        Vector2 joystickVector = new Vector2(horizontal, vertical);
        inputIntensity = Mathf.Clamp01(joystickVector.magnitude);

        bool isJoystickActive = inputIntensity > 0.01f;

        playerFlame.SetActive(isJoystickActive);

        rb.linearDamping = isJoystickActive ? 0f : 0.05f;

        ProcessInput();
        KeepPlayerOnScreen();
        RotatePlayerFace();
    }



    void FixedUpdate()
    {
        if (inputIntensity > 0.01f)
        {
            // Joystick aktifken kuvvet uygula
            rb.AddForce(lastKnownDirection * forceMagnitude * inputIntensity, ForceMode.Force);
        }

        // Hızı maksimumla sınırlamaya devam
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxVelocity);
    }

    public Vector3 GetCurrentMovementDirection()
    {
        return rb.linearVelocity.normalized;
    }
    private void ProcessInput()
    {
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        Vector2 joystickVector = new Vector2(horizontal, vertical);
        inputIntensity = Mathf.Clamp01(joystickVector.magnitude);

        if (inputIntensity > 0.01f)
        {
            movementDirection = new Vector3(horizontal, vertical, 0f).normalized;
            lastKnownDirection = movementDirection;
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
        if (rb.linearVelocity.magnitude < 0.1f) { return; }

        // 3D model X=-90'da yerleşmiş, hareket yönüne bakması için Y ekseni etrafında döndür
        Vector3 direction = rb.linearVelocity.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        
        // X=-90 sabit, Y ekseni hareket yönüne göre dönecek, Z=0
        Quaternion targetRotation = Quaternion.Euler(-90f, angle, 0f);

        transform.rotation = Quaternion.Lerp(
            transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision c)
    {
        if (c.collider.CompareTag("Asteroid"))
        {
            rb.angularVelocity = Vector3.zero;
            // X=-90 sabit, Y'yi koru, Z=0
            var e = transform.eulerAngles;
            transform.rotation = Quaternion.Euler(-90, e.y, 0);
        }
    }

}
