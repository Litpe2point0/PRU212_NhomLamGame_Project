using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class PlayerKnight : MonoBehaviour
{

    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float m_rollForce = 6.0f;
    [SerializeField] bool m_noBlood = false;
    [SerializeField] GameObject m_slideDust;

    [Header("Layer Mask")]
    public LayerMask slopeMask;

    private float base_speed;
    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_PlayerKnight m_groundSensor;
    private Sensor_PlayerKnight m_wallSensorR1;
    private Sensor_PlayerKnight m_wallSensorR2;
    private Sensor_PlayerKnight m_wallSensorL1;
    private Sensor_PlayerKnight m_wallSensorL2;
    private bool m_isWallSliding = false;
    private bool m_grounded = false;
    private bool m_rolling = false;
    private int m_facingDirection = 1;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;
    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;
    private Vector2 moveInput;
    private bool isOnSlope;
    private Vector3 initScale;
    private BlockAndParry bap;
    private Health playerHealth;
    private void Awake()
    {
        initScale = transform.localScale;
    }
    // Use this for initialization
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_PlayerKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_PlayerKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_PlayerKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_PlayerKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_PlayerKnight>();
        base_speed = m_speed;
        bap = GetComponent<BlockAndParry>();
        playerHealth = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        bap.CheckParryWindow();
        bap.SetDirection(m_facingDirection);
        isOnSlope = Physics2D.Raycast(transform.position, Vector2.down, 1.0f, slopeMask);
        AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        // Increase timer that checks roll duration
        if (m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if (m_rollCurrentTime > m_rollDuration)
        {
            m_rollCurrentTime = 0.0f;
            m_rolling = false;
        }

        //Check if character just landed on the ground
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
        if (!m_rolling)
            Movement(inputX);

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.linearVelocity.y);

        // -- Handle Animations --
        //Wall Slide
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);



        //Attack
        if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            m_currentAttack++;

            // Loop back to one after third attack
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            m_animator.SetTrigger("Attack" + m_currentAttack);

            // Reset timer
            m_timeSinceAttack = 0.0f;
        }

        // Block
        else if (Input.GetMouseButtonDown(1) && !m_rolling)
        {
            m_speed = 1.5f;
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
            bap.StartBlockAndParry();
        }

        else if (Input.GetMouseButtonUp(1))
        {
            m_speed = base_speed;
            m_animator.SetBool("IdleBlock", false);
            bap.EndBlockAndParry();
        }


        // Roll
        else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding)
        {
            Roll();
        }


        //Jump
        else if (Input.GetKeyDown("space") && m_grounded && !m_rolling)
        {
            Jump();
        }

        //Run
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


        if (m_grounded && (stateInfo.IsName("Idle") || stateInfo.IsName("Hurt") || stateInfo.IsName("Death")) && isOnSlope)
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
            m_facingDirection = 1;
        }

        else if (inputX < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(initScale.x) * inputX, initScale.y, initScale.z);
            m_facingDirection = -1;
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

    void Roll()
    {
        m_rolling = true;
        m_body2d.bodyType = RigidbodyType2D.Dynamic;
        m_animator.SetTrigger("Roll");
        playerHealth.SetInvunerbility(true);
        Physics2D.IgnoreLayerCollision(7, 10, true);
        m_body2d.linearVelocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.linearVelocity.y);
        StartCoroutine(ReenableCollision(0.5f));
    }
    IEnumerator ReenableCollision(float delay)
    {
        yield return new WaitForSeconds(delay);
        playerHealth.SetInvunerbility(false);
        Physics2D.IgnoreLayerCollision(7, 10, false);
    }
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Animation Events
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }
    public void ResetInput()
    {
        moveInput = Vector2.zero;  // 🛠 Reset movement input
        if (m_body2d != null)
        {
            m_body2d.linearVelocity = Vector2.zero;  // 🛠 Stop velocity
            m_body2d.angularVelocity = 0f;
        }
    }
}
