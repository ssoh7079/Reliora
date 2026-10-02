using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("데이터")]
    [SerializeField] private BossData bossData;

    [Header("공격")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask playerLayer;

    [Header("패턴")]
    [SerializeField] private GameObject warningIcon;

    [Header("레퍼런스")]
    [SerializeField] private Transform character;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer bodyRenderer;

    private Transform player;
    private PlayerController playerController;

    private Rigidbody2D rb;
    private Collider2D bossCollider;

    private BossState state = BossState.Intro;

    private float characterScaleX;

    private float stateTimer;
    private float attackCoolTimer;
    private float patternCoolTimer;

    private int patternPhase;

    private bool isAttack;
    private bool isPatternAttack;
    private bool isVanished;

    private float vanishY;

    public BossState State => state;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bossCollider = GetComponent<Collider2D>();

        characterScaleX = Mathf.Abs(character.localScale.x);

        if (warningIcon != null) warningIcon.SetActive(false);
    }
    private void Update()
    {
        if (state == BossState.Dead || state == BossState.Intro) return;
        if (attackCoolTimer > 0.0f) attackCoolTimer -= Time.deltaTime;
        if (patternCoolTimer > 0.0f) patternCoolTimer -= Time.deltaTime;
        if (stateTimer > 0.0f) stateTimer -= Time.deltaTime;

        switch (state)
        {
            case BossState.Ready:
                ReadyState();
                break;
            case BossState.Trace:
                TraceState();
                break;
            case BossState.Pattern1:
                Pattern1State();
                break;
        }
    }
    private void FixedUpdate()
    {
        if (state == BossState.Dead || state == BossState.Intro)
        {
            Stop();
            return;
        }
        if (isVanished) return;
        if (state == BossState.Trace)
        {
            MoveToPlayer(bossData.MoveSpeed);
            return;
        }
        if (state == BossState.Pattern1 && patternPhase == 1)
        {
            MoveToPlayer(bossData.PatternRunSpeed);
            return;
        }
        Stop();
    }

    public void SetPlayer(Transform target)
    {
        player = target;
        playerController = target.GetComponent<PlayerController>();
    }
    public void StartIntro()
    {
        state = BossState.Intro;
        Stop();
        animator.SetBool("Run", false);
        animator.SetBool("Ready", true);
        Flip();
    }
    public void StartBattle()
    {
        patternCoolTimer = bossData.PatternCoolTime;
        EnterReady();
    }
    private void EnterReady()
    {
        if (state == BossState.Dead) return;

        state = BossState.Ready;
        stateTimer = bossData.ReadyTime;

        isAttack = false;
        isPatternAttack = false;

        Stop();
        animator.SetBool("Run", false);
        animator.SetBool("Ready", true);
        Flip();
    }
    private void ReadyState()
    {
        if (stateTimer > 0.0f) return;
        if (patternCoolTimer <= 0.0f)
        {
            int pattern = Random.Range(0, 2);
            if (pattern == 0)
            {
                EnterPattern1();
            }
            else
            {
                EnterPattern2();
            }
            return;
        }

        state = BossState.Trace;

        animator.SetBool("Ready", false);
        animator.SetBool("Run", true);
    }
    private void TraceState()
    {
        if (player == null) return;

        Flip();

        float distance = Mathf.Abs(player.position.x - transform.position.x);
        if (distance > bossData.AttackRange) return;
        if (attackCoolTimer > 0.0f) return;

        StartAttack(false);
    }
    private void StartAttack(bool patternAttack)
    {
        state = BossState.Attack;

        isAttack = true;
        isPatternAttack = patternAttack;

        Stop();

        Flip();

        animator.SetBool("Ready", false);
        animator.SetBool("Run", false);

        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");
    }
    public void AttackHit()
    {
        if (state != BossState.Attack || !isAttack) return;

        int dir = character.localScale.x >= 0.0f ? 1 : -1;
        Vector2 center = (Vector2)attackPoint.position + Vector2.right * dir * (bossData.AttackSize.x * 0.5f);
        Collider2D hit = Physics2D.OverlapBox( center, bossData.AttackSize, 0.0f, playerLayer);
        if (hit == null) return;

        PlayerStatus playerStatus = hit.GetComponentInParent<PlayerStatus>();
        if (playerStatus != null) playerStatus.TakeDamage(bossData.AttackDamage);
    }
    public void AttackEnd()
    {
        if (state != BossState.Attack || !isAttack) return;

        isAttack = false;

        attackCoolTimer = bossData.AttackCoolTime;
        if (isPatternAttack) patternCoolTimer = bossData.PatternCoolTime;

        isPatternAttack = false;

        EnterReady();
    }
    private void EnterPattern1()
    {
        state = BossState.Pattern1;
        patternPhase = 0;

        stateTimer = bossData.PatternReadyTime;

        Stop();

        animator.SetBool("Run", false);
        animator.SetBool("Ready", true);

        if (warningIcon != null) warningIcon.SetActive(true);

        Flip();
    }
    private void Pattern1State()
    {
        if (patternPhase == 0)
        {
            if (stateTimer > 0.0f) return;

            patternPhase = 1;

            if (warningIcon != null) warningIcon.SetActive(false);

            animator.SetBool("Ready", false);
            animator.SetBool("Run", true);
            return;
        }
        if (player == null) return;

        Flip();

        float distance = Mathf.Abs(player.position.x - transform.position.x);
        if (distance > bossData.AttackRange) return;

        StartAttack(true);
    }
    private void EnterPattern2()
    {
        if (state == BossState.Dead) return;

        state = BossState.Pattern2;

        Stop();

        animator.SetBool("Run", false);
        animator.SetBool("Ready", true);

        StartCoroutine(Pattern2());
    }
    private IEnumerator Pattern2()
    {
        if (warningIcon != null) warningIcon.SetActive(true);

        yield return new WaitForSeconds(bossData.PatternReadyTime);

        if (state == BossState.Dead) yield break;
        if (warningIcon != null) warningIcon.SetActive(false);

        animator.SetBool("Ready", false);

        float effectTime = 0.18f;
        float currentTime = 0.0f;
        float ghostTimer = 0.0f;

        while (currentTime < effectTime)
        {
            if (state == BossState.Dead) yield break;

            currentTime += Time.deltaTime;
            ghostTimer -= Time.deltaTime;
            if (ghostTimer <= 0.0f)
            {
                SpawnGhost();
                ghostTimer = 0.06f;
            }
            yield return null;
        }

        isVanished = true;
        //현재 바닥 높이 저장
        vanishY = transform.position.y;
        //보스 외형 숨김
        character.gameObject.SetActive(false);
        //Collider만 끄면 중력으로 떨어지므로
        //패턴 동안 Rigidbody2D 자체를 정지
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        yield return new WaitForSeconds(bossData.VanishTime);

        if (state == BossState.Dead) yield break;

        MoveBehindPlayer();

        rb.simulated = true;
        rb.linearVelocity = Vector2.zero;

        character.gameObject.SetActive(true);

        isVanished = false;

        Flip();

        animator.SetBool("Ready", false);
        animator.SetBool("Run", false);

        StartAttack(true);
    }
    private void MoveBehindPlayer()
    {
        if (player == null) return;

        int facingDir = 1;
        if (playerController != null) facingDir = playerController.FacingDir;

        float appearX = player.position.x - facingDir * bossData.AppearDistance;
        transform.position = new Vector3(appearX, vanishY, transform.position.z);
    }
    private void SpawnGhost()
    {
        if (bodyRenderer == null || bodyRenderer.sprite == null) return;

        GameObject ghost = new GameObject("BossGhost");
        BossGhost bossGhost = ghost.AddComponent<BossGhost>();
        bossGhost.Initialize(bodyRenderer);
    }
    public void Hit()
    {
        if (state == BossState.Dead || state == BossState.Intro) return;
        if (isVanished) return;
        //공격 애니메이션의 Animation Event가
        //중간에 끊기는 것을 방지
        if (state == BossState.Attack) return;

        animator.SetTrigger("Hit");
    }
    public void Die()
    {
        if (state == BossState.Dead) return;

        StopAllCoroutines();

        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }
        if (character != null)
        {
            character.gameObject.SetActive(true);
        }

        state = BossState.Dead;

        isAttack = false;
        isPatternAttack = false;
        isVanished = false;

        Stop();

        if (warningIcon != null) warningIcon.SetActive(false);

        animator.ResetTrigger("Attack");
        animator.SetBool("Ready", false);
        animator.SetBool("Run", false);
        animator.SetBool("Die", true);
    }
    public void DieEnd()
    {
        BossRoom bossRoom = GetComponentInParent<BossRoom>();
        if (bossRoom != null) bossRoom.BossDead();

        Destroy(gameObject);
    }
    private void MoveToPlayer(float speed)
    {
        if (player == null) return;

        float dir = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
    }
    private void Flip()
    {
        if (player == null) return;

        float dir = player.position.x - transform.position.x;
        Vector3 scale = character.localScale;
        scale.x = dir >= 0.0f ? characterScaleX : -characterScaleX;
        character.localScale = scale;
    }
    private void Stop()
    {
        if (rb == null || !rb.simulated) return;

        rb.linearVelocity = new Vector2(0.0f, rb.linearVelocity.y);
    }



    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null || bossData == null) return;

        int dir = 1;
        if (character != null && character.localScale.x < 0.0f) dir = -1;

        Vector2 center = (Vector2)attackPoint.position + Vector2.right * dir * (bossData.AttackSize.x * 0.5f);
        Gizmos.DrawWireCube(center, bossData.AttackSize);
    }
}