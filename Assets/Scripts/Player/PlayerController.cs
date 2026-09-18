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
    
    [Header("레퍼런스?")]
    [SerializeField] private Transform character;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private Camera mainCam;
    private PlayerStatus status;
    private PlayerCombat combat;

    private float moveX;
    private float characterScaleX;
    
    private bool isGround;
    private int jumpCount;

    private string currentAnim;

    private const string IdleAnim = "Idle";
    private const string RunAnim = "Run";
    private const string JumpAnim = "Jump";

    public int FacingDir => character.localScale.x >= 0.0f ? 1 : -1;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
        status = GetComponent<PlayerStatus>();
        combat = GetComponent<PlayerCombat>();

        characterScaleX = Mathf.Abs(character.localScale.x);
    }
    void Update()
    {
        if (status.IsDead || status.IsHit)
        {
            moveX = 0.0f;
            return;
        }

        MoveInput();
        CheckGround();
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
        isGround = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        //착지한 경우에만 점프 횟수 초기화
        if (isGround && rb.linearVelocity.y <= 0.0f) jumpCount = 0;
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
