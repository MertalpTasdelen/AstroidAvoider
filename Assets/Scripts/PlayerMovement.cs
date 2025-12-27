using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{
    [SerializeField] private float forceMagnitude;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private GameObject playerFlame;
    [SerializeField] private FloatingJoystick joystick;

    [Header("Screen Wrap")]
    [Tooltip("How far past the edge (in viewport units) the ship must go before wrapping.")]
    [SerializeField, Range(0f, 0.25f)] private float screenWrapPaddingViewport = 0.02f;


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
        // Joystick yönüne göre geminin hızını doğrudan ayarla (kaymayı engelle)
        if (inputIntensity > 0.01f)
        {
            Vector3 desiredVelocity = lastKnownDirection * (maxVelocity * inputIntensity);
            rb.linearVelocity = desiredVelocity;
            rb.angularVelocity = Vector3.zero;

            // Debug: Joystick yönü ve gemi rotasyonu
            Debug.Log($"Joystick Yönü: {lastKnownDirection} | Gemi Rotation: {transform.eulerAngles} | Gemi Forward: {transform.forward}");
        }
        else
        {
            // Giriş yoksa hızını yumuşakça azalt
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, 10f * Time.fixedDeltaTime);
            rb.angularVelocity = Vector3.zero;
        }
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
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                return;
            }
        }

        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);
        float pad = screenWrapPaddingViewport;

        bool wrapped = false;

        if (viewportPosition.x > 1f + pad)
        {
            viewportPosition.x = -pad;
            wrapped = true;
        }
        else if (viewportPosition.x < -pad)
        {
            viewportPosition.x = 1f + pad;
            wrapped = true;
        }

        if (viewportPosition.y > 1f + pad)
        {
            viewportPosition.y = -pad;
            wrapped = true;
        }
        else if (viewportPosition.y < -pad)
        {
            viewportPosition.y = 1f + pad;
            wrapped = true;
        }

        if (!wrapped)
        {
            return;
        }

        Vector3 wrappedWorld = mainCamera.ViewportToWorldPoint(viewportPosition);
        wrappedWorld.z = 0f;
        transform.position = wrappedWorld;

    }

    private void RotatePlayerFace()
    {
        // Joystick girişi varsa geminin X rotasyonunu (bakış yönü) değiştir
        if (inputIntensity < 0.01f) return;

        // Joystick yönüne göre X rotasyonunu hesapla (forward ile joystick'i hizalamak için y'i tersle)
        float angle = -Mathf.Atan2(lastKnownDirection.y, lastKnownDirection.x) * Mathf.Rad2Deg;
        
        // Y=90° sabit, Z=-90° sabit, X açısı joystick yönüne göre değişir (bakış yönü)
        Quaternion targetRotation = Quaternion.Euler(angle, 90f, -90f);
        
        // Rotasyonu anında değiştir
        transform.rotation = targetRotation;
    }

    private void OnCollisionEnter(Collision c)
    {
        if (c.collider.CompareTag("Asteroid"))
        {
            // Asteroide çarptığında angular velocity'yi sıfırla
            rb.angularVelocity = Vector3.zero;
            // Y=90°, Z=-90° kalırken X'i koru
            var e = transform.eulerAngles;
            transform.rotation = Quaternion.Euler(e.x, 90f, -90f);
        }
    }

}
