using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private float direction;
    private bool hit;
    private float lifeTime;

    private BoxCollider2D boxCollider2D;
    private Animator anim;
    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        if(hit)
        {
            return;
        }
        float movementSpeed = speed * Time.deltaTime;
        transform.Translate(movementSpeed * direction, 0,0);

        lifeTime += Time.deltaTime;
        if(lifeTime >= 5)
        {
            Deactivate();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        boxCollider2D.enabled = false;
        anim.SetTrigger("Explode");
        
        if(collision.tag == "Enemy")
        {
            collision.GetComponent<Health>().TakeDamage(1);
        }
    }
    public void SetDirection(float _direction)
    {
        lifeTime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider2D.enabled = true;

        float localScaleX = transform.localScale.x;
        if(Mathf.Sign(localScaleX) != direction)
        {
            localScaleX = -localScaleX;
        }
        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
