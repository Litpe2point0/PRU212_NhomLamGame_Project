using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private float damage = 1f;
    private float direction;
    private bool hit;
    private float lifeTime;

    private BoxCollider2D boxCollider2D;
    private Animator anim;
    private AudioPlayer audioPlayer;
    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
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
    public void SetDamage(float value)
    {
        damage = value;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        boxCollider2D.enabled = false;
        anim.SetTrigger("Explode");
        audioPlayer.PlayMagicOrbHitClip();
        if (collision.tag == "Enemy")
        {
            collision.GetComponent<Health>().TakeDamage(damage);
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
