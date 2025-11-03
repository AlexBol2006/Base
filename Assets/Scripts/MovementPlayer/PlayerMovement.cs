using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float groundDrag;
    [Header("Movements")]
    private float walkSpeed = 2f;
    private float runSpeed = 4f;

    [Header("Stamina")]
    public float maxStamina = 80f;
    public float currentStamina;
    private float staminaDrain = 16f;
    private float stamiRegen = 5f;
    private float stamiRegenDelay = 1.5f;
    private float regenTimer;
    private bool isRunning;
    private bool exhausted;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        moveSpeed = walkSpeed;
        currentStamina = maxStamina;
    }

    private void Update()
    {
        // detector de suelo
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        HandleDrag();
        Run();
        UpdateStamina();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        // calcular movimiento y direcci�n
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * moveSpeed * 3f, ForceMode.VelocityChange);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // limite de velocidad necesario
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    public void HandleDrag()
    {
        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;
    }

    private void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift) && grounded && verticalInput > 0 && !exhausted && currentStamina > 0)
        {
            moveSpeed = runSpeed;
            isRunning = true;
        }
        else
        {
            moveSpeed = walkSpeed;
            isRunning = false;
        }
    }

    private void UpdateStamina()
    {
        if (isRunning)
        {
            currentStamina -= staminaDrain * Time.deltaTime;
            regenTimer = stamiRegenDelay; // reinicia cooldown cada vez que corres

            if (currentStamina <= 0)
            {
                currentStamina = 0;
                exhausted = true;
            }
        }
        else
        {
            if (regenTimer > 0)
            {
                regenTimer -= Time.deltaTime; // espera antes de regenerar
            }
            else if (currentStamina < maxStamina)
            {
                currentStamina += stamiRegen * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

                // desbloquea cuando se regenere al 50%
                if (exhausted && currentStamina >= maxStamina * 0.2f)
                    exhausted = false;
            }
        }
    }
}
