using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Enemy")]
    [SerializeField] private int initialDirection = 1;
    [SerializeField] private Rigidbody2D enemy;

    [Header("Movement Parameter")]
    [SerializeField] private float speed;
    private Vector3 initScale;
    private bool movingLeft = true;

    [Header("Enemy Animator")]
    [SerializeField] private Animator anim;

    [Header("Idle Behaviour")]
    [SerializeField] private float idleDuration;
    private float idleTimer;
    private void Awake()
    {
        initScale = enemy.transform.localScale;
    }
    private void Update()
    {
        if (GetComponentInChildren<Health>().IsDead())
        {
            this.enabled = false;
        }
        if (movingLeft)
        {
            if(enemy.transform.position.x >= pointA.position.x)
                moveDirection(-1);
            else
                DirectionChange();
        }
        else
        {
            if (enemy.transform.position.x <= pointB.position.x)
                moveDirection(1);
            else
                DirectionChange();
        }
    }
    private void OnDisable()
    {
        anim.SetBool("isWalking", false);
    }
    private void DirectionChange()
    {
        anim.SetBool("isWalking", false);
        enemy.linearVelocity = new Vector2(0, enemy.linearVelocity.y);
        idleTimer += Time.deltaTime;
        if(idleTimer > idleDuration)
        {
            movingLeft = !movingLeft;
        }
    }
    private void moveDirection(int direction)
    {
        idleTimer = 0;
        anim.SetBool("isWalking", true);
        enemy.transform.localScale = new Vector3(Mathf.Abs(initScale.x) * direction * initialDirection, initScale.y, initScale.z);

        enemy.linearVelocity = new Vector2(direction * speed, enemy.linearVelocity.y);
    }
    public int GetInitialDirection()
    {
        return initialDirection;
    }
}
