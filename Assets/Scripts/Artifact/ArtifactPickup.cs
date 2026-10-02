using UnityEngine;
using UnityEngine.InputSystem;

public class ArtifactPickup : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private ArtifactData artifact;
    private ArtifactInventory inventory;

    private bool isPlayerInside;
    private bool isTaken;

    public void Initialize(ArtifactData data)
    {
        artifact = data;

        if (spriteRenderer != null) spriteRenderer.sprite = artifact.Icon;

    }
    public void SetVisualSize(float targetHeight)
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null || targetHeight <= 0.0f) return;

        Sprite sprite = spriteRenderer.sprite;
        Texture2D texture = sprite.texture;
        if (!texture.isReadable)
        {
            Debug.LogWarning($"{sprite.name}의 Read/Write Enabled를 켜주세요.");
            return;
        }

        Rect rect = sprite.rect;

        int startX = Mathf.RoundToInt(rect.xMin);
        int startY = Mathf.RoundToInt(rect.yMin);
        int width = Mathf.RoundToInt(rect.width);
        int height = Mathf.RoundToInt(rect.height);

        Color32[] pixels = texture.GetPixels32();

        int minX = startX + width;
        int minY = startY + height;
        int maxX = -1;
        int maxY = -1;

        //실제로 그려진 픽셀 영역 찾기
        for (int y = startY; y < startY + height; y++)
        {
            for (int x = startX; x < startX + width; x++)
            {
                Color32 color = pixels[y * texture.width + x];
                if (color.a <= 8) continue;
                minX = Mathf.Min(minX, x);
                minY = Mathf.Min(minY, y);
                maxX = Mathf.Max(maxX, x);
                maxY = Mathf.Max(maxY, y);
            }
        }

        if (maxX < minX || maxY < minY) return;

        float ppu = sprite.pixelsPerUnit;
        //투명 여백을 제외한 실제 그림 높이
        float visibleHeight = (maxY - minY + 1.0f) / ppu;
        float scale = targetHeight / visibleHeight;
        //실제로 그려진 그림의 가로 중심
        float centerX = ((minX + maxX + 1.0f) * 0.5f - startX - sprite.pivot.x) / ppu;
        //실제로 그려진 그림의 아래쪽 끝
        float bottomY = (minY - startY - sprite.pivot.y) / ppu;

        Transform visual = spriteRenderer.transform;
        visual.localScale = new Vector3(scale, scale, 1.0f);
        //그림의 중심은 루트 X에,
        //그림의 아래쪽은 루트 Y(지면)에 맞춤
        visual.localPosition = new Vector3(-centerX * scale, -bottomY * scale, 0.0f);
    }

    private void Update()
    {
        if (!isPlayerInside || isTaken) return;
        if (Keyboard.current == null) return;
        if (!Keyboard.current.fKey.wasPressedThisFrame) return;

        PickUp();
    }

    private void PickUp()
    {
        if (inventory == null || artifact == null) return;
        if (!inventory.AddArtifact(artifact)) return;

        isTaken = true;

        Debug.Log($"아티팩트 재획득 : {artifact.ArtifactName}");

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ArtifactInventory playerInventory = other.GetComponentInParent<ArtifactInventory>();

        if (playerInventory == null) return;

        inventory = playerInventory;
        isPlayerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ArtifactInventory playerInventory = other.GetComponentInParent<ArtifactInventory>();

        if (playerInventory == null || playerInventory != inventory) return;

        inventory = null;
        isPlayerInside = false;
    }
}