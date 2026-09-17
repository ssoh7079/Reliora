using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("데이터")]
    [SerializeField] private EnemyData enemyData;

    [Header("공격 관련")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask playerLayer;

    [Header("레퍼런스??(이름 변경해야 할 듯)")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform character;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;

    private bool isAttack;
    private float attackCoolTimer;
    private float characterScaleX;

    private EnemyState state = EnemyState.Idle;

    public EnemyState State => state;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        characterScaleX = Mathf.Abs(character.localScale.x);
    }
    void Start()
    {
        SetMoveAnimation(false);
    }
    void Update()
    {
        if (state == EnemyState.Dead) return;
        if (attackCoolTimer > 0.0f) attackCoolTimer -= Time.deltaTime;

        float distance = Mathf.Abs(player.position.x - transform.position.x);

        switch (state)
        {
            case EnemyState.Idle:
                IdleState(distance);
                break;
            case EnemyState.Trace:
                TraceState(distance);
                break;
            case EnemyState.Attack:
                AttackState(distance);
                break;
        }
    }
    private void FixedUpdate()
    {
        if (state != EnemyState.Trace) return;

        float dir = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(dir * enemyData.MoveSpeed, rb.linearVelocity.y);
    }

    private void IdleState(float distance)
    {
        Stop();
        SetMoveAnimation(false);

        if (distance <= enemyData.DetectRange) state = EnemyState.Trace;
    }
    private void TraceState(float distance)
    {
        if (distance > enemyData.DetectRange)
        {
            state = EnemyState.Idle;
            return;
        }
        if (distance <= enemyData.AttackRange)
        {
            Stop();
            SetMoveAnimation(false);
            state = EnemyState.Attack;
            return;
        }

        Flip();
        SetMoveAnimation(true);
    }
    private void AttackState(float distance)
    {
        Stop();
        SetMoveAnimation(false);

        if (isAttack) return;
        if (distance > enemyData.AttackRange)
        {
            state = EnemyState.Trace;
            return;
        }
        if (attackCoolTimer > 0.0f) return;

        Flip();
        isAttack = true;
        animator.SetTrigger("Attack");
    }
    private void Flip()
    {
        float dir = player.position.x - transform.position.x;
        Vector3 scale = character.localScale;
        scale.x = dir >= 0f ? characterScaleX : -characterScaleX;
        character.localScale = scale;
    }
    private void Stop()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }
    private void SetMoveAnimation(bool isMove)
    {
        animator.SetBool("Idle", !isMove);
        animator.SetBool("Walk", isMove);
    }
    public void AttackHit()
    {
        if (state != EnemyState.Attack) return;

        int dir = character.localScale.x >= 0f ? 1 : -1;
        Vector2 center = (Vector2)attackPoint.position + Vector2.right * dir * (enemyData.AttackSize.x * 0.5f);
        Collider2D hit = Physics2D.OverlapBox(center, enemyData.AttackSize, 0f, playerLayer);
        if (hit == null) return;

        PlayerStatus playerStatus = hit.GetComponentInParent<PlayerStatus>();
        if (playerStatus != null) playerStatus.TakeDamage(enemyData.AttackDamage);
    }
    public void AttackEnd()
    {
        if (state != EnemyState.Attack) return;

        isAttack = false;
        attackCoolTimer = enemyData.AttackCoolTime;

        float distance = Mathf.Abs(player.position.x - transform.position.x);
        if (distance <= enemyData.DetectRange)
        {
            state = EnemyState.Trace;
        }
        else
        {
            state = EnemyState.Idle;
        }
    }
    public void DieEnd()
    {
        Destroy(gameObject);
    }
    public void Hit()
    {
        if (state == EnemyState.Dead) return;

        isAttack = false;
        attackCoolTimer = enemyData.AttackCoolTime;

        Stop();
        animator.SetTrigger("Hit");
        state = EnemyState.Trace;
    }
    public void Die()
    {
        if (state == EnemyState.Dead) return;

        state = EnemyState.Dead;
        isAttack = false;
        Stop();

        animator.SetBool("Idle", false);
        animator.SetBool("Walk", false);
        animator.SetBool("Die", true);
    }



    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null || enemyData == null) return;

        int dir = 1;
        if (character != null && character.localScale.x < 0f) dir = -1;

        Vector2 center = (Vector2)attackPoint.position + Vector2.right * dir * (enemyData.AttackSize.x * 0.5f);
        Gizmos.DrawWireCube(center, enemyData.AttackSize);
    }
}
