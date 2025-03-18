using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWizard : MonoBehaviour
{
    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;

    [Header("Layer Mask")]
    public LayerMask slopeMask;

    private float base_speed;
    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_PlayerKnight m_groundSensor;
    private float m_attackCooldown = 1.1f;
    private float m_attackCooldownTimer = 1.1f;
    private float m_delayToIdle = 0.0f;
    private bool m_grounded = false;
    private Vector2 moveInput;
    private bool isOnSlope;
    private Vector3 initScale;
    private RangePlayer rangePlayer;
    private void Awake()
    {
        initScale = transform.localScale;
    }
    void Start()
    {
        rangePlayer = GetComponent<RangePlayer>();
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_PlayerKnight>();
        base_speed = m_speed;
    }

    // Update is called once per frame
    void Update()
    {
        isOnSlope = Physics2D.Raycast(transform.position, Vector2.down, 1.0f, slopeMask);
        AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
        m_attackCooldownTimer += Time.deltaTime;
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = moveInput.x;

        // Swap direction of sprite depending on walk direction
        FlipSprite(inputX);

        // Move
        if(m_attackCooldownTimer >= m_attackCooldown)
            Movement(inputX);

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.linearVelocity.y);

        //Attack
        if (Input.GetMouseButtonDown(0) && m_attackCooldownTimer >= m_attackCooldown)
        {
            m_body2d.linearVelocityX = 0;
            m_animator.SetTrigger("Attack");
            // Reset timer
            m_attackCooldownTimer = 0.0f;
        }

        //Jump
        else if (Input.GetKeyDown("space") && m_grounded && m_attackCooldownTimer >= m_attackCooldown)
        {
            Jump();
        }

        //Walk
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetBool("isWalking", true);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetBool("isWalking", false);
        }


        if (m_grounded && (stateInfo.IsName("Idle") || stateInfo.IsName("Attack") || stateInfo.IsName("Hurt") || stateInfo.IsName("Death")) && isOnSlope)
        {
            m_body2d.bodyType = RigidbodyType2D.Kinematic;
            m_body2d.linearVelocity = new Vector2(0, 0);
        }
        else if (!stateInfo.IsName("Idle") || !m_grounded)
            m_body2d.bodyType = RigidbodyType2D.Dynamic;
    }
    void FlipSprite(float inputX)
    {
        if (inputX > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(initScale.x) * inputX, initScale.y, initScale.z);
        }

        else if (inputX < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(initScale.x) * inputX, initScale.y, initScale.z);
        }
    }

    void Movement(float inputX)
    {
        m_body2d.linearVelocity = new Vector2(inputX * m_speed, m_body2d.linearVelocity.y);
    }
    void Jump()
    {
        m_animator.SetTrigger("Jump");
        m_grounded = false;
        m_animator.SetBool("Grounded", m_grounded);
        m_body2d.linearVelocity = new Vector2(m_body2d.linearVelocity.x, m_jumpForce);
        m_groundSensor.Disable(0.2f);
    }
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    public void Fire()
    {
        rangePlayer.Attack();
    }
}
