using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerCombat : MonoBehaviour
{
    [Header("애니메이터")]
    [SerializeField] private Animator animator;

    [Header("공격")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private Vector2 attackSize = new Vector2(2.5f, 2.5f);
    [SerializeField] private LayerMask enemyLayer;

    [Header("UI")]
    [SerializeField] private ArtifactInventoryUI inventoryUI;


    private PlayerStatus status;
    private PlayerController controller;
    private ArtifactSkillController skillController;

    private int artifactAttackBonus;
    private float artifactAttackSpeedBonus;

    public bool IsAttack { get; private set; }

    public int AttackDamage => attackDamage + artifactAttackBonus;
    public float AttackSpeed => Mathf.Max(0.1f, 1.0f + artifactAttackSpeedBonus);


    private void Awake()
    {
        status = GetComponent<PlayerStatus>();
        controller = GetComponent<PlayerController>();
        skillController = GetComponent<ArtifactSkillController>();

        UpdateAttackSpeed();
    }
    void Update()
    {
        if (status.IsDead || status.IsHit || 
            controller.IsControlLocked || controller.IsDash || 
            IsAttack || (skillController != null && skillController.IsLineCasting)) return;
        
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) return;
        //인벤토리 관련 입력이 일반 공격으로 전달되지 않게
        if (inventoryUI != null && inventoryUI.BlocksAttack) return;
        //버튼, 슬롯 등 UI를 클릭했을 때 공격 방지
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Attack();
    }

    private void Attack()
    {
        IsAttack = true;
        animator.SetTrigger("AttackSlash");
    }
    //AttackSlash 애니메이션 이벤트
    public void AttackHit()
    {
        if (!IsAttack) return;

        Vector2 center = (Vector2)attackPoint.position + Vector2.right * controller.FacingDir * (attackSize.x * 0.5f);
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, attackSize, 0.0f, enemyLayer);
        foreach (Collider2D hit in hits)
        {
            EnemyStatus enemy = hit.GetComponentInParent<EnemyStatus>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
                continue;
            }

            BossStatus boss = hit.GetComponentInParent<BossStatus>();
            if (boss != null) boss.TakeDamage(attackDamage);
        }
    }
    //AttackSlash 애니메이션 이벤트
    public void AttackEnd()
    {
        if (!IsAttack) return;
        IsAttack = false;
        controller.ResetAnimation();
    }
    //피격, 사망 시 공격 강제 중단
    public void CancelAttack()
    {
        if (!IsAttack) return;
        IsAttack = false;
        animator.ResetTrigger("AttackSlash");
        controller.ResetAnimation();
    }
    public void SetArtifactStats(int attackBonus, float attackSpeedBonus)
    {
        artifactAttackBonus = Mathf.Max(attackBonus, 0);
        artifactAttackSpeedBonus = Mathf.Max(attackSpeedBonus, 0.0f);
        UpdateAttackSpeed();
        //확인용
        Debug.Log($"Artifact Combat : ATK {AttackDamage}, AttackSpeed {AttackSpeed:F2}");
    }
    private void UpdateAttackSpeed()
    {
        animator.SetFloat("AttackAnimSpeed", AttackSpeed);
    }



    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        int dir = 1;
        PlayerController player = GetComponent<PlayerController>();
        if (player != null) dir = player.FacingDir;

        Vector2 center = (Vector2)attackPoint.position + Vector2.right * dir * (attackSize.x * 0.5f);
        Gizmos.DrawWireCube(center, attackSize);
    }
}
