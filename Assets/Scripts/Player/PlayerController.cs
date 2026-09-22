using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("이동속도")]
    [SerializeField] private float moveSpeed = 5.0f;
    
    [Header("점프")]
    [SerializeField] private float jumpPower = 15.0f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("대시")]
    [SerializeField] private float dashSpeed = 15.0f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCoolTime = 0.5f;
    [SerializeField] private float ghostSpawnInterval = 0.06f;
    
    [Header("레퍼런스")]
    [SerializeField] private Transform character;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer bodyRenderer;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private Camera mainCam;

    private PlayerStatus status;
    private PlayerCombat combat;

    private float moveX;
    private float characterScaleX;

    private bool isGround;
    private bool isDropPlatform;
    private int jumpCount;
    
    private float dashTimer;
    private float dashCoolTimer;
    private float ghostTimer;
    private int dashDir;

    private string currentAnim;

    private const string IdleAnim = "Idle";
    private const string RunAnim = "Run";
    private const string JumpAnim = "Jump";

    public int FacingDir => character.localScale.x >= 0.0f ? 1 : -1;
    public bool IsDash { get; private set; }


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        mainCam = Camera.main;
        status = GetComponent<PlayerStatus>();
        combat = GetComponent<PlayerCombat>();

        characterScaleX = Mathf.Abs(character.localScale.x);
    }
    void Update()
    {
        if (dashCoolTimer > 0.0f) dashCoolTimer -= Time.deltaTime;
        if (status.IsDead || status.IsHit)
        {
            moveX = 0.0f;
            CancelDash();
            return;
        }
        if (IsDash)
        {
            DashUpdate();
            return;
        }

        MoveInput();
        CheckGround();
        if (DropPlatform()) return;
        Dash();
        if (IsDash) return;
        Jump();

        if (!combat.IsAttack)
        {
            Flip();
            AnimationState();
        }
    }
    private void FixedUpdate()
    {
        if (status.IsDead)
        {
            rb.linearVelocity = new Vector2(0.0f, rb.linearVelocity.y);
            return;
        }
        if (IsDash)
        {
            rb.linearVelocity = new Vector2(dashDir * dashSpeed, 0.0f);
            return;
        }

        Move();
    }


    private void MoveInput()
    {
        moveX = 0.0f;

        if (Keyboard.current.aKey.isPressed) moveX = -1.0f;
        if (Keyboard.current.dKey.isPressed) moveX = 1.0f;
    }
    private void Move()
    {
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);
    }
    private void Jump()
    {
        if (!Keyboard.current.spaceKey.wasPressedThisFrame) return;
        //점프를 누르지 않고, 발판에서 그냥 떨어졌을 경우, 공중에서 점프는 한 번만 가능
        if (!isGround && jumpCount == 0) jumpCount = 1;
        if (jumpCount >= 2) return;
        //2단 점프 시, 점프 애니메이션을 다시 재생
        if (jumpCount == 1) currentAnim = "";

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);

        jumpCount++;
    }
    private void CheckGround()
    {
        if (isDropPlatform)
        {
            isGround = false;
            return;
        }
        isGround = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        //착지한 경우에만 점프 횟수 초기화
        if (isGround && rb.linearVelocity.y <= 0.0f) jumpCount = 0;
    }
    private bool DropPlatform()
    {
        if (isDropPlatform) return false;
        if (!Keyboard.current.sKey.wasPressedThisFrame) return false;

        RaycastHit2D[] hits = Physics2D.CircleCastAll(groundCheck.position, groundRadius, Vector2.down, groundRadius, groundLayer);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null) continue;
            PlatformEffector2D effector = hit.collider.GetComponentInParent<PlatformEffector2D>();
            if (effector == null) continue;
            StartCoroutine(DropPlatformCoroutine(hit.collider, hit.point.y));
            return true;
        }
        
        return false;
    }
    private IEnumerator DropPlatformCoroutine(Collider2D platform, float platformY)
    {
        isDropPlatform = true;
        isGround = false;

        currentAnim = "";
        PlayAnimation(JumpAnim);
        Physics2D.IgnoreCollision(playerCollider, platform, true);
        yield return new WaitForFixedUpdate();

        while (platform != null && playerCollider.bounds.max.y > platformY) yield return new WaitForFixedUpdate();
        if (platform != null) Physics2D.IgnoreCollision(playerCollider, platform, false);
        
        isDropPlatform = false;
    }
    
    private void Dash()
    {
        if (!Keyboard.current.leftShiftKey.wasPressedThisFrame) return;
        if (dashCoolTimer > 0.0f) return;
        //공격 중 대시하면 공격 캔슬 후 Idle 자세로 변경
        if (combat.IsAttack)
        {
            combat.CancelAttack();
            animator.Play(IdleAnim, 0, 0.0f);
            currentAnim = IdleAnim;
        }

        dashDir = Mathf.Abs(moveX) > 0.01f ? (int)Mathf.Sign(moveX) : FacingDir;
        IsDash = true;
        dashTimer = dashDuration;
        ghostTimer = 0.0f;
        rb.linearVelocity = new Vector2(dashDir * dashSpeed, 0.0f);
    }
    private void DashUpdate()
    {
        dashTimer -= Time.deltaTime;
        ghostTimer -= Time.deltaTime;
        //대시 중에만 잔상 생성
        if (ghostTimer <= 0.0f)
        {
            SpawnGhost();
            ghostTimer = ghostSpawnInterval;
        }
        if (dashTimer <= 0.0f) EndDash();
    }
    private void EndDash()
    {
        if (!IsDash) return;

        IsDash = false;
        dashTimer = 0.0f;
        dashCoolTimer = dashCoolTime;
        rb.linearVelocity = new Vector2(0.0f, rb.linearVelocity.y);
        ResetAnimation();
    }
    public void CancelDash()
    {
        if (!IsDash) return;

        IsDash = false;
        dashTimer = 0.0f;
        dashCoolTimer = dashCoolTime;
        rb.linearVelocity = new Vector2(0.0f, rb.linearVelocity.y);
        ResetAnimation();
    }
    private void SpawnGhost()
    {
        if (bodyRenderer == null || !bodyRenderer.enabled || bodyRenderer.sprite == null) return;

        GameObject ghost = new GameObject("PlayerGhost");
        PlayerGhost playerGhost = ghost.AddComponent<PlayerGhost>();
        playerGhost.Initialize(bodyRenderer);
    }
    private void Flip()
    {
        if (Mouse.current == null || mainCam == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = mainCam.ScreenToWorldPoint(mousePos);
        Vector3 scale = character.localScale;

        scale.x = worldPos.x >= transform.position.x ? characterScaleX : -characterScaleX;
        character.localScale = scale;
    }

    private void AnimationState()
    {
        if (!isGround)
        {
            PlayAnimation(JumpAnim);
            return;
        }
        if (Mathf.Abs(moveX) > 0.01f)
        {
            PlayAnimation(RunAnim);
            return;
        }

        PlayAnimation(IdleAnim);
    }
    private void PlayAnimation(string anim)
    {
        if (currentAnim == anim) return;

        currentAnim = anim;
        animator.Play(anim);
    }

    public void ResetAnimation()
    {
        currentAnim = "";
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
