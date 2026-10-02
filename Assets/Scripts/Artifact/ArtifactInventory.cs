using System;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactInventory : MonoBehaviour
{
    [Header("가방")]
    [SerializeField] private int capacity = 15;

    [Header("장착")]
    [SerializeField] private int maxEquipCount = 6;

    [Header("세트")]
    [SerializeField] private ArtifactSetData[] setDatas;

    [Header("월드 드랍")]
    [SerializeField] private ArtifactPickup pickupPrefab;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float pickupHeight = 1.0f;

    private readonly List<ArtifactData> artifacts = new List<ArtifactData>();
    private readonly HashSet<ArtifactData> equippedArtifacts = new HashSet<ArtifactData>();

    private PlayerStatus status;
    private PlayerCombat combat;
    private PlayerController controller;

    public IReadOnlyList<ArtifactData> Artifacts => artifacts;
    public IReadOnlyList<ArtifactSetData> SetDatas => setDatas;

    public int Capacity => capacity;
    public int EquippedCount => equippedArtifacts.Count;
    public int MaxEquipCount => maxEquipCount;

    public event Action OnInventoryChanged;
    public event Action OnEquipmentChanged;

    private void Awake()
    {
        status = GetComponent<PlayerStatus>();
        combat = GetComponent<PlayerCombat>();
        controller = GetComponent<PlayerController>();
    }

    public bool AddArtifact(ArtifactData artifact)
    {
        if (artifact == null) return false;
        if (artifacts.Count >= capacity)
        {
            Debug.Log("아티팩트 가방이 가득 찼습니다.");
            return false;
        }
        //고유 아티팩트는 보유 중일 때만 중복 획득 불가
        if (artifact.IsUnique && artifacts.Contains(artifact))
        {
            Debug.Log($"[고유] {artifact.ArtifactName} 이미 보유 중");
            return false;
        }

        artifacts.Add(artifact);

        Debug.Log($"아티팩트 획득 : {artifact.ArtifactName}");

        OnInventoryChanged?.Invoke();

        return true;
    }
    public bool EquipArtifact(ArtifactData artifact)
    {
        if (artifact == null) return false;
        if (!artifacts.Contains(artifact)) return false;
        if (equippedArtifacts.Contains(artifact)) return false;
        if (equippedArtifacts.Count >= maxEquipCount)
        {
            Debug.Log($"최대 {maxEquipCount}개까지 장착 가능합니다.");
            return false;
        }

        equippedArtifacts.Add(artifact);

        RefreshStats();

        Debug.Log($"아티팩트 장착 : {artifact.ArtifactName}");

        OnEquipmentChanged?.Invoke();

        return true;
    }
    public bool UnequipArtifact(ArtifactData artifact)
    {
        if (artifact == null) return false;
        if (!equippedArtifacts.Remove(artifact)) return false;

        RefreshStats();

        Debug.Log($"아티팩트 장착 해제 : {artifact.ArtifactName}");

        OnEquipmentChanged?.Invoke();

        return true;
    }
    public bool DropArtifact(ArtifactData artifact)
    {
        if (artifact == null) return false;
        if (!artifacts.Contains(artifact)) return false;
        if (pickupPrefab == null)
        {
            Debug.LogWarning("ArtifactPickup Prefab이 없습니다.");
            return false;
        }

        int dir = controller.FacingDir;
        Vector2 origin = (Vector2)transform.position + new Vector2(dir * 1.2f, 1.0f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 20.0f, groundLayer);
        if (hit.collider == null)
        {
            Debug.LogWarning("드랍 위치에서 지면을 찾지 못했습니다.");
            return false;
        }

        Vector3 dropPos = new Vector3(origin.x, hit.point.y, transform.position.z);
        ArtifactPickup pickup = Instantiate(pickupPrefab, dropPos, Quaternion.identity);
        pickup.Initialize(artifact);
        //실제 그림 높이를 동일하게 맞추고 바닥에 정렬
        pickup.SetVisualSize(pickupHeight);

        equippedArtifacts.Remove(artifact);
        artifacts.Remove(artifact);

        RefreshStats();

        Debug.Log($"아티팩트 버리기 : {artifact.ArtifactName}");

        OnInventoryChanged?.Invoke();
        OnEquipmentChanged?.Invoke();

        return true;
    }
    public bool CanEquip(ArtifactData artifact)
    {
        if (artifact == null) return false;
        if (!artifacts.Contains(artifact)) return false;
        if (equippedArtifacts.Contains(artifact)) return false;

        return equippedArtifacts.Count < maxEquipCount;
    }
    public bool HasArtifact(ArtifactData artifact)
    {
        return artifacts.Contains(artifact);
    }
    public bool IsEquipped(ArtifactData artifact)
    {
        return equippedArtifacts.Contains(artifact);
    }
    public ArtifactSetData GetSetData(ArtifactData artifact)
    {
        if (artifact == null || setDatas == null) return null;

        foreach (ArtifactSetData setData in setDatas)
        {
            if (setData == null) continue;

            if (setData.FirstArtifact == artifact || setData.SecondArtifact == artifact) return setData;
        }
        return null;
    }
    public bool IsSetActive(ArtifactSetData setData)
    {
        if (setData == null) return false;
        return IsEquipped(setData.FirstArtifact) && IsEquipped(setData.SecondArtifact);
    }
    private void RefreshStats()
    {
        int attackBonus = 0;
        int defenseBonus = 0;
        int maxHpBonus = 0;
        float attackSpeedBonus = 0.0f;

        foreach (ArtifactData artifact in equippedArtifacts)
        {
            attackBonus += artifact.AttackBonus;
            defenseBonus += artifact.DefenseBonus;
            maxHpBonus += artifact.MaxHpBonus;
            attackSpeedBonus += artifact.AttackSpeedBonus;
        }

        status.SetArtifactStats(defenseBonus, maxHpBonus);
        combat.SetArtifactStats(attackBonus, attackSpeedBonus);
    }
}
