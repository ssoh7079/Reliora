using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("데이터")]
    [SerializeField] private EnemyData enemyData;

    [Header("공격 관련")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask playerLayer;

    [Header("레퍼런스")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform character;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;

    private bool isAttack;
    private float attackCoolTimer;

    private float characterScaleX;

    private float startX;
    private float patrolDir;
    private float patrolWaitTimer;

    private EnemyState state = EnemyState.Idle;

    public EnemyState State => state;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        characterScaleX = Mathf.Abs(character.localScale.x);
        startX = transform.position.x;
    }
    void Start()
    {
        SetMoveAnimation(false);
        patrolWaitTimer = enemyData.PatrolWaitTime;
    }
    void Update()
    {
        if (state == EnemyState.Dead) return;
        if (attackCoolTimer > 0.0f) attackCoolTimer -= Time.deltaTime;

        float xDistance = Mathf.Abs(player.position.x - transform.position.x);
        float yDistance = Mathf.Abs(player.position.y - transform.position.y);

        switch (state)
        {
            case EnemyState.Idle:
                IdleState(xDistance);
                break;
            case EnemyState.Patrol:
                PatrolState(xDistance);
                break;
            case EnemyState.Trace:
                TraceState(xDistance, yDistance);
                break;
            case EnemyState.Attack:
                AttackState(xDistance, yDistance);
                break;
        }
    }
    private void FixedUpdate()
    {
        if (state == EnemyState.Patrol)
        {
            rb.linearVelocity = new Vector2(patrolDir * enemyData.PatrolSpeed, rb.linearVelocity.y);
            return;
        }
        if (state != EnemyState.Trace) return;

        float dir = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(dir * enemyData.MoveSpeed, rb.linearVelocity.y);
    }

    private void IdleState(float distance)
    {
        Stop();
        SetMoveAnimation(false);

        if (distance <= enemyData.DetectRange)
        {
            state = EnemyState.Trace;
            return;
        }

        patrolWaitTimer -= Time.deltaTime;
        if (patrolWaitTimer > 0.0f) return;

        patrolDir = Random.Range(0, 2) == 0 ? -1.0f : 1.0f;
        Flip(patrolDir);
        state = EnemyState.Patrol;
    }
    private void PatrolState(float distance)
    {
        if (distance <= enemyData.DetectRange)
        {
            state = EnemyState.Trace;
            return;
        }

        SetMoveAnimation(true);

        float moveDistance = Mathf.Abs(transform.position.x - startX);
        if (moveDistance >= enemyData.PatrolDistance)
        {
            Stop();
            patrolWaitTimer = enemyData.PatrolWaitTime;
            state = EnemyState.Idle;
        }
    }
    private void TraceState(float xDistance, float yDistance)
    {
        if (xDistance > enemyData.DetectRange)
        {
            Stop();
            patrolWaitTimer = enemyData.PatrolWaitTime;
            state = EnemyState.Idle;
            return;
        }
        if (IsInAttackRange(xDistance, yDistance))
        {
            Stop();
            SetMoveAnimation(false);
            state = EnemyState.Attack;
            return;
        }

        Flip(player.position.x - transform.position.x);
        SetMoveAnimation(true);
    }
    private void AttackState(float xDistance, float yDistance)
    {
        Stop();
        SetMoveAnimation(false);

        if (isAttack) return;
        if (!IsInAttackRange(xDistance, yDistance))
        {
            state = EnemyState.Trace;
            return;
        }
        if (attackCoolTimer > 0.0f) return;

        Flip(player.position.x - transform.position.x);
        isAttack = true;
        animator.SetTrigger("Attack");
    }
    private bool IsInAttackRange(float xDistance, float yDistance)
    {
        return xDistance <= enemyData.AttackRange && yDistance <= enemyData.AttackHeightRange;
    }
    private void Flip(float dir)
    {
        Vector3 scale = character.localScale;
        scale.x = dir >= 0.0f ? characterScaleX : -characterScaleX;
        character.localScale = scale;
    }
    private void Stop()
    {
        rb.linearVelocity = new Vector2(0.0f, rb.linearVelocity.y);
    }
    private void SetMoveAnimation(bool isMove)
    {
        animator.SetBool("Idle", !isMove);
        animator.SetBool("Walk", isMove);
    }
    public void AttackHit()
    {
        if (state != EnemyState.Attack) return;

        int dir = character.localScale.x >= 0.0f ? 1 : -1;
        Vector2 center = (Vector2)attackPoint.position + Vector2.right * dir * (enemyData.AttackSize.x * 0.5f);
        Collider2D hit = Physics2D.OverlapBox(center, enemyData.AttackSize, 0.0f, playerLayer);
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
            patrolWaitTimer = enemyData.PatrolWaitTime;
            state = EnemyState.Idle;
        }
    }
    public void Hit()
    {
        if (state == EnemyState.Dead) return;

        bool wasAttack = isAttack;
        isAttack = false;
        //공격 도중 끊긴 경우에만 쿨타임 시작
        if (wasAttack)
        {
            attackCoolTimer = enemyData.AttackCoolTime;
        }

        Stop();
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Hit");
        state = EnemyState.Trace;
    }
    public void Die()
    {
        if (state == EnemyState.Dead) return;

        state = EnemyState.Dead;
        isAttack = false;
        Stop();

        animator.ResetTrigger("Attack");
        animator.SetBool("Idle", false);
        animator.SetBool("Walk", false);
        animator.SetBool("Die", true);
    }
    public void DieEnd()
    {
        Destroy(gameObject);
    }

    public void SetPlayer(Transform target)
    {
        player = target;
    }


    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null || enemyData == null) return;

        int dir = 1;
        if (character != null && character.localScale.x < 0.0f) dir = -1;

        Vector2 center = (Vector2)attackPoint.position + Vector2.right * dir * (enemyData.AttackSize.x * 0.5f);
        Gizmos.DrawWireCube(center, enemyData.AttackSize);
    }
}
