using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private Vector2 attackSize = new Vector2(1.2f, 1.2f);
    [SerializeField] private LayerMask enemyLayer;

    public bool IsAttack { get; private set; }

    private PlayerStatus status;
    private PlayerController controller;


    private void Awake()
    {
        status = GetComponent<PlayerStatus>();
        controller = GetComponent<PlayerController>();
    }
    void Update()
    {
        if (status.IsDead || status.IsHit || IsAttack) return;
        if (Mouse.current.leftButton.wasPressedThisFrame) Attack();
    }

    private void Attack()
    {
        IsAttack = true;
        animator.SetTrigger("AttackSlash");
    }
    //AttackSlash 애니메이션 이벤트
    public void AttackHit()
    {
        Vector2 center = (Vector2)attackPoint.position + Vector2.right * controller.FacingDir * (attackSize.x * 0.5f);
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, attackSize, 0.0f, enemyLayer);
        foreach (Collider2D hit in hits)
        {
            EnemyStatus enemy = hit.GetComponentInParent<EnemyStatus>();
            if (enemy != null) enemy.TakeDamage(attackDamage);
        }
    }
    //AttackSlash 애니메이션 이벤트
    public void AttackEnd()
    {
        IsAttack = false;
        controller.ResetAnimation();
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
