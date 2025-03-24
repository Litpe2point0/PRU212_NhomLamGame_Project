using UnityEngine;

public class DarkWizardMovement : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private PlayerSwitch playerSwitch;
    [SerializeField] private int initialDirection;
    private Animator anim;
    private Rigidbody2D rb;
    private Sensor_Bandit groundSensor;
    private bool m_grounded = false;
    private Transform target;
    private UIManager uiManager;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    private void Update()
    {
        if(GetComponent<Health>().GetCurrentHealth() <= 0)
        {
            uiManager.ShowWin();
        }
        CheckCurrentPlayer();
        //Check if character just landed on the ground
        if (!m_grounded && groundSensor.State())
        {
            m_grounded = true;
            anim.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !groundSensor.State())
        {
            m_grounded = false;
            anim.SetBool("Grounded", m_grounded);
        }

        FlipSprite();
        Move();
    }
    void CheckCurrentPlayer()
    {
        if (playerSwitch.GetPlayer1Active())
        {
            target = playerSwitch.GetPlayer1().transform;
        }
        else
        {
            target = playerSwitch.GetPlayer2().transform;
        }
    }
    private void Move()
    {
        if (target == null) return;

        anim.SetBool("isWalking", true);
        float direction = Mathf.Sign(target.position.x - transform.position.x); // Left (-1) or Right (1)

        // Move only if not already at the target
        if (Vector2.Distance(transform.position, target.position) > 0.3f)
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y); // Move in X direction, keep Y velocity
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Stop moving when close
        }
    }
    void FlipSprite()
    {
        if (Mathf.Sign(target.position.x - transform.position.x) > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (Mathf.Sign(target.position.x - transform.position.x) < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
    public int GetInitialDirection()
    {
        return initialDirection;
    }
}
