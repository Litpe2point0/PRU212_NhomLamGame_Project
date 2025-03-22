using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private Vector2 parallaxEffectMultiplier;
    [SerializeField] private bool InfiniteHorizontal;
    [SerializeField] private bool InfiniteVertical;

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float textureUnitSizeX;
    private float textureUnitSizeY;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("ParallaxBackground: Missing SpriteRenderer!");
            return;
        }

        Sprite sprite = spriteRenderer.sprite;
        Texture2D texture = sprite.texture;

        // Correct texture size by considering scale
        textureUnitSizeX = (texture.width / sprite.pixelsPerUnit) * transform.lossyScale.x;
        textureUnitSizeY = (texture.height / sprite.pixelsPerUnit) * transform.lossyScale.y;
    }

    private void LateUpdate()
    {
        if (cameraTransform == null) return;

        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier.x, deltaMovement.y * parallaxEffectMultiplier.y, 0);
        lastCameraPosition = cameraTransform.position;

        // Wrap background for infinite scrolling
        if (InfiniteHorizontal)
        {
            float distanceX = Mathf.Abs(cameraTransform.position.x - transform.position.x);
            if (distanceX >= textureUnitSizeX)
            {
                float offsetX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
                transform.position = new Vector3(cameraTransform.position.x + offsetX, transform.position.y, transform.position.z);
            }
        }

        if (InfiniteVertical)
        {
            float distanceY = Mathf.Abs(cameraTransform.position.y - transform.position.y);
            if (distanceY >= textureUnitSizeY)
            {
                float offsetY = (cameraTransform.position.y - transform.position.y) % textureUnitSizeY;
                transform.position = new Vector3(transform.position.x, cameraTransform.position.y + offsetY, transform.position.z);
            }
        }
    }
}
