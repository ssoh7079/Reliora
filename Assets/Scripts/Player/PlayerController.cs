using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float jumpPower = 12.0f;

    [SerializeField] private Transform character;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private Camera mainCam;
    private PlayerStatus status;

    private float moveX;
    private float characterScaleX;
    private bool isGround;

    private string currentAnim;

    private const string IdleAnim = "Idle";
    private const string RunAnim = "Run";
    private const string JumpAnim = "Jump";

    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
        status = GetComponent<PlayerStatus>();

        characterScaleX = Mathf.Abs(character.localScale.x);
    }
    void Update()
    {
        if (status.IsDead)
        {
            moveX = 0.0f;
            return;
        }
        if (status.IsHit)
        {
            moveX = 0.0f;
            return;
        }

        MoveInput();
        CheckGround();
        Jump();
        Flip();
        AnimationState();
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
        if (!Keyboard.current.spaceKey.wasPressedThisFrame || !isGround) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
    }
    private void CheckGround()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
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
