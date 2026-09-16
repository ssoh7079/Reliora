using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Animator animator;
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
        if (Mouse.current.leftButton.wasPressedThisFrame) StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        IsAttack = true;
        animator.SetTrigger("AttackSlash");
        //Animator가 AttackSlash 상태로 전환될 때까지 대기
        yield return null;

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("AttackSlash"))
        {
            yield return null;
        }
        //AttackSlash 애니메이션이 끝날 때까지 대기
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        IsAttack = false;

        controller.ResetAnimation();
    }
}
