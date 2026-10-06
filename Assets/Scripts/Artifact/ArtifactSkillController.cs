using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArtifactSkillController : MonoBehaviour
{
    [Header("판정")]
    [SerializeField] private Transform skillPoint;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float targetRange = 6.0f;

    [Header("전격선 공격 판정")]
    [SerializeField] private Vector2 lineSize = new Vector2(7.0f, 2.0f);

    [Header("전격선 VFX 크기")]
    [SerializeField] private Vector2 lineVfxSize = new Vector2(7.0f, 2.0f);

    [Header("타겟팅 VFX 크기")]
    [SerializeField] private Vector2 lightningVfxScale = Vector2.one;
    [SerializeField] private Vector2 hammerVfxScale = Vector2.one;

    [Header("스킬 VFX")]
    [SerializeField] private GameObject lightningVfx;
    [SerializeField] private GameObject hammerVfx;
    [SerializeField] private GameObject lineVfx;

    [Header("VFX 재생 속도")]
    [SerializeField] private float lightningVfxSpeed = 2.0f;
    [SerializeField] private float hammerVfxSpeed = 2.0f;
    [SerializeField] private float lineVfxSpeed = 1.0f;

    private const int MaxSkillCount = 3;

    private ArtifactInventory inventory;
    private PlayerController controller;
    private PlayerStatus status;
    private PlayerCombat combat;

    private readonly List<ArtifactSetData> activeSkills = new List<ArtifactSetData>();
    private readonly Dictionary<ArtifactSetData, float> coolTimes = new Dictionary<ArtifactSetData, float>();

    public bool IsLineCasting { get; private set; }

    public IReadOnlyList<ArtifactSetData> ActiveSkills => activeSkills;

    public event Action OnSkillsChanged;

    private void Awake()
    {
        inventory = GetComponent<ArtifactInventory>();
        controller = GetComponent<PlayerController>();
        status = GetComponent<PlayerStatus>();
        combat = GetComponent<PlayerCombat>();
    }
    private void OnEnable()
    {
        inventory.OnEquipmentChanged += RefreshSkills;
    }
    private void Start()
    {
        RefreshSkills();
    }
    private void Update()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            UseSkill(0);
        }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            UseSkill(1);
        }
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            UseSkill(2);
        }
    }
    private void OnDisable()
    {
        inventory.OnEquipmentChanged -= RefreshSkills;
    }

    private void RefreshSkills()
    {
        //현재 비활성화된 스킬만 제거
        for (int i = activeSkills.Count - 1; i >= 0; i--)
        {
            ArtifactSetData skill = activeSkills[i];
            if (skill == null || !inventory.IsSetActive(skill))
            {
                activeSkills.RemoveAt(i);
            }
        }
        //새롭게 활성화된 스킬은 맨 뒤에 추가
        foreach (ArtifactSetData setData in inventory.SetDatas)
        {
            if (setData == null) continue;
            if (!inventory.IsSetActive(setData)) continue;
            if (activeSkills.Contains(setData)) continue;

            activeSkills.Add(setData);
            if (activeSkills.Count >= MaxSkillCount) break;
        }
        OnSkillsChanged?.Invoke();
    }
    public bool UseSkill(int index)
    {
        if (index < 0 || index >= activeSkills.Count) return false;
        if (status.IsDead || status.IsHit || controller.IsDash || controller.IsControlLocked || combat.IsAttack || IsLineCasting)
        {
            return false;
        }

        ArtifactSetData skill = activeSkills[index];
        if (GetRemainCoolTime(index) > 0.0f) return false;

        bool used = false;
        switch (skill.SkillType)
        {
            case ArtifactSkillType.Lightning:
                used = UseLightning(skill);
                break;
            case ArtifactSkillType.JudgmentHammer:
                used = UseHammer(skill);
                break;
            case ArtifactSkillType.ElectricLine:
                used = UseElectricLine(skill);
                break;
        }

        if (!used) return false;

        coolTimes[skill] = Time.time + skill.SkillCoolTime;
        return true;
    }
    private bool UseLightning(ArtifactSetData skill)
    {
        if (lightningVfx == null) return false;

        List<Component> targets = GetRangeTargets();
        if (targets.Count == 0) return false;

        //랜덤으로 섞기
        for (int i = targets.Count - 1; i > 0; i--)
        {
            int random = UnityEngine.Random.Range(0, i + 1);
            Component temp = targets[i];
            targets[i] = targets[random];
            targets[random] = temp;
        }

        int count = Mathf.Min(3, targets.Count);
        for (int i = 0; i < count; i++)
        {
            SpawnTargetVfx(lightningVfx, skill, targets[i], lightningVfxSpeed, lightningVfxScale);
        }
        return true;
    }
    private bool UseHammer(ArtifactSetData skill)
    {
        if (hammerVfx == null) return false;

        List<Component> targets = GetRangeTargets();
        if (targets.Count == 0) return false;

        Component nearest = null;
        float nearestDistance = float.MaxValue;
        foreach (Component target in targets)
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance >= nearestDistance) continue;
            nearestDistance = distance;
            nearest = target;
        }

        if (nearest == null) return false;

        SpawnTargetVfx(hammerVfx, skill, nearest, hammerVfxSpeed, hammerVfxScale);
        return true;
    }
    private bool UseElectricLine(ArtifactSetData skill)
    {
        if (lineVfx == null) return false;

        Vector2 origin = skillPoint != null ? skillPoint.position : transform.position;
        int dir = controller.FacingDir;
        //전격선이 끝날 때까지 조작 잠금
        IsLineCasting = true;
        SpawnLineVfx(lineVfx, skill, origin, dir);
        //전격선은 적이 없어도 사용 가능
        return true;
    }
    private void SpawnTargetVfx(GameObject prefab, ArtifactSetData skill, Component target, float animationSpeed, Vector2 vfxScale)
    {
        if (target == null) return;

        Bounds targetBounds = GetTargetBounds(target);
        GameObject effectObject = Instantiate(prefab, targetBounds.center, Quaternion.identity);
        //인스펙터에서 지정한 Scale을 그대로 사용
        effectObject.transform.localScale = new Vector3(vfxScale.x, vfxScale.y, effectObject.transform.localScale.z);
        //VFX의 실제 그림 중심을 몬스터 중앙에 맞춤
        AlignVisualCenter(effectObject, targetBounds.center);
        //몬스터가 움직일 때, VFX도 같이 따라가도록 월드 크기를 유지한 상태로 자식으로 붙임
        effectObject.transform.SetParent(target.transform, true);

        ArtifactSkillEffect effect = effectObject.GetComponent<ArtifactSkillEffect>();
        if (effect == null)
        {
            effect = effectObject.AddComponent<ArtifactSkillEffect>();
        }

        effect.Initialize(this, skill, target, targetBounds.center, 
            new Vector2(targetBounds.size.x, targetBounds.size.y),
            targetBounds.center);
        SetupAnimation(effectObject, effect, animationSpeed);
    }
    private void SpawnLineVfx(GameObject prefab, ArtifactSetData skill, Vector2 origin, int dir)
    {
        GameObject effectObject = Instantiate(prefab, origin, Quaternion.identity);
        //바라보는 방향으로 VFX 뒤집기
        Vector3 scale = effectObject.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * dir;
        effectObject.transform.localScale = scale;
        //전격선 VFX는 Inspector 값으로 직접 크기 조절
        ResizeVisual(effectObject, lineVfxSize);
        //VFX 끝단을 정확히 SkillPoint에 맞춤
        AlignLineStart(effectObject, origin, dir);
        Vector2 hitCenter = origin + Vector2.right * dir * (lineSize.x * 0.5f);

        ArtifactSkillEffect effect = effectObject.GetComponent<ArtifactSkillEffect>();
        if (effect == null)
        {
            effect = effectObject.AddComponent<ArtifactSkillEffect>();
        }

        effect.Initialize(this, skill, null, hitCenter, lineSize, origin);
        SetupAnimation(effectObject, effect, lineVfxSpeed);
    }
    public void EndSkillEffect(ArtifactSkillEffect effect)
    {
        if (effect == null || effect.SkillData == null) return;
        if (effect.SkillData.SkillType == ArtifactSkillType.ElectricLine) IsLineCasting = false;
    }
    private void SetupAnimation(GameObject effectObject, ArtifactSkillEffect effect, float animationSpeed)
    {
        Animator[] animators = effectObject.GetComponentsInChildren<Animator>(true);
        foreach (Animator animator in animators)
        {
            animator.speed = Mathf.Max(0.01f, animationSpeed);
            ArtifactSkillAnimationEvent animationEvent = animator.GetComponent<ArtifactSkillAnimationEvent>();
            if (animationEvent == null)
            {
                animationEvent = animator.gameObject.AddComponent<ArtifactSkillAnimationEvent>();
            }
            animationEvent.Initialize(effect);
        }
    }
    public void ResolveSkillHit(ArtifactSkillEffect effect)
    {
        if (effect == null || effect.SkillData == null) return;

        switch (effect.SkillData.SkillType)
        {
            case ArtifactSkillType.Lightning:
            case ArtifactSkillType.JudgmentHammer:
                ResolveTargetSkillHit(effect);
                break;
            case ArtifactSkillType.ElectricLine:
                ResolveLineSkillHit(effect);
                break;
        }
    }
    private void ResolveTargetSkillHit(ArtifactSkillEffect effect)
    {
        if (effect.Target == null) return;

        Bounds currentBounds = GetTargetBounds(effect.Target);
        Collider2D[] hits = Physics2D.OverlapBoxAll(currentBounds.center, effect.HitSize, 0.0f, enemyLayer);
        foreach (Collider2D hit in hits)
        {
            Component target = GetTarget(hit);
            if (target != effect.Target) continue;
            DamageTarget(target, effect.SkillData.SkillDamage);
            return;
        }
    }
    private void ResolveLineSkillHit(ArtifactSkillEffect effect)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(effect.HitCenter, effect.HitSize, 0.0f, enemyLayer);
        List<Component> targets = GetUniqueTargets(hits);
        //캐릭터에서 가까운 순서
        targets.Sort((a, b) => Vector2.Distance(effect.CastOrigin, a.transform.position)
        .CompareTo(Vector2.Distance(effect.CastOrigin, b.transform.position)));

        int count = Mathf.Min(3, targets.Count);
        for (int i = 0; i < count; i++)
        {
            DamageTarget(targets[i], effect.SkillData.SkillDamage);
        }
    }
    private void ResizeVisual(GameObject effectObject, Vector2 targetSize)
    {
        if (!TryGetVisualBounds(effectObject, out Bounds bounds))
        {
            return;
        }
        if (bounds.size.x <= 0.001f || bounds.size.y <= 0.001f)
        {
            return;
        }

        float scaleX = targetSize.x / bounds.size.x;
        float scaleY = targetSize.y / bounds.size.y;
        Vector3 scale = effectObject.transform.localScale;

        scale.x *= scaleX;
        scale.y *= scaleY;

        effectObject.transform.localScale = scale;
    }
    private void AlignVisualCenter(GameObject effectObject, Vector2 targetCenter)
    {
        if (!TryGetVisualBounds(effectObject, out Bounds bounds))
        {
            return;
        }

        Vector3 move = (Vector3)targetCenter - bounds.center;
        move.z = 0.0f;
        effectObject.transform.position += move;
    }
    private void AlignLineStart(GameObject effectObject, Vector2 skillPosition, int dir)
    {
        if (!TryGetVisualBounds(effectObject, out Bounds bounds))
        {
            return;
        }

        float edgeX = dir > 0 ? bounds.min.x : bounds.max.x;
        float moveX = skillPosition.x - edgeX;
        float moveY = skillPosition.y - bounds.center.y;

        effectObject.transform.position += new Vector3(moveX, moveY, 0.0f);
    }
    private Bounds GetTargetBounds(Component target)
    {
        SpriteRenderer[] renderers = target.GetComponentsInChildren<SpriteRenderer>();
        bool found = false;
        Bounds bounds = new Bounds();

        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer.sprite == null) continue;
            //이미 붙어 있는 스킬 VFX는 몬스터 크기 계산에서 제외
            ArtifactSkillEffect skillEffect = renderer.GetComponentInParent<ArtifactSkillEffect>();
            if (skillEffect != null) continue;
            if (!found)
            {
                bounds = renderer.bounds;
                found = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        if (found) return bounds;

        Collider2D collider = target.GetComponentInChildren<Collider2D>();
        if (collider != null) return collider.bounds;

        return new Bounds(target.transform.position, Vector3.one);
    }
    private bool TryGetVisualBounds(GameObject obj, out Bounds bounds)
    {
        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>(true);
        bool found = false;
        bounds = new Bounds();
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer.sprite == null) continue;
            if (!found)
            {
                bounds = renderer.bounds;
                found = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }
        return found;
    }
    private List<Component> GetRangeTargets()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, targetRange, enemyLayer);
        return GetUniqueTargets(hits);
    }
    private List<Component> GetUniqueTargets(Collider2D[] hits)
    {
        List<Component> targets = new List<Component>();
        HashSet<Component> seen = new HashSet<Component>();
        foreach (Collider2D hit in hits)
        {
            Component target = GetTarget(hit);
            if (target == null) continue;
            if (!seen.Add(target)) continue;
            targets.Add(target);
        }
        return targets;
    }
    private Component GetTarget(Collider2D hit)
    {
        EnemyStatus enemy = hit.GetComponentInParent<EnemyStatus>();
        if (enemy != null && enemy.CurrentHp > 0) return enemy;

        BossStatus boss = hit.GetComponentInParent<BossStatus>();
        if (boss != null && boss.CurrentHp > 0) return boss;

        return null;
    }
    private void DamageTarget(Component target, int damage)
    {
        if (target == null) return;

        EnemyStatus enemy = target as EnemyStatus;
        if (enemy != null)
        {
            if (enemy.CurrentHp > 0)
            {
                enemy.TakeDamage(damage);
            }
            return;
        }

        BossStatus boss = target as BossStatus;
        if (boss != null && boss.CurrentHp > 0)
        {
            boss.TakeDamage(damage);
        }
    }
    public float GetRemainCoolTime( int index)
    {
        if (index < 0 || index >= activeSkills.Count)
        {
            return 0.0f;
        }

        ArtifactSetData skill = activeSkills[index];
        if (!coolTimes.TryGetValue(skill, out float endTime))
        {
            return 0.0f;
        }

        return Mathf.Max(0.0f, endTime - Time.time);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, targetRange);

        if (skillPoint == null) return;

        int dir = 1;
        PlayerController player = GetComponent<PlayerController>();
        if (player != null)
        {
            dir = player.FacingDir;
        }

        Vector2 center = (Vector2)skillPoint.position + Vector2.right * dir * (lineSize.x * 0.5f);
        Gizmos.DrawWireCube(center, lineSize);
    }
}