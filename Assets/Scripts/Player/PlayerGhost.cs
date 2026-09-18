using UnityEngine;

public class PlayerGhost : MonoBehaviour
{
    private const float LifeTime = 0.18f;
    private const float StartAlpha = 0.5f;

    private SpriteRenderer spriteRenderer;
    private float currentTime;

    public void Initialize(SpriteRenderer source)
    {
        transform.position = source.transform.position;
        transform.rotation = source.transform.rotation;
        transform.localScale = source.transform.lossyScale;

        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        spriteRenderer.sprite = source.sprite;
        spriteRenderer.flipX = source.flipX;
        spriteRenderer.flipY = source.flipY;

        spriteRenderer.sortingLayerID = source.sortingLayerID;
        spriteRenderer.sortingOrder = source.sortingOrder - 1;

        Color color = source.color;
        color.a = StartAlpha;

        spriteRenderer.color = color;
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        float ratio = Mathf.Clamp01(1.0f - currentTime / LifeTime);

        Color color = spriteRenderer.color;
        color.a = StartAlpha * ratio;
        spriteRenderer.color = color;

        if (currentTime >= LifeTime) Destroy(gameObject);
    }
}
