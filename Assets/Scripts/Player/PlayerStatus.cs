using System.Collections;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] private int maxHp = 100;
    [SerializeField] private float hitDuration = 0.35f;
    [SerializeField] private Animator animator;

    private PlayerController controller;
    private PlayerCombat combat;

    private int hp;

    public int Hp => hp;
    public int MaxHp => maxHp;

    public bool IsHit {  get; private set; }
    public bool IsDead {  get; private set; }


    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        combat = GetComponent<PlayerCombat>();
        hp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead || IsHit) return;
        //공격 중이었다면 강제 종료
        combat.CancelAttack();

        hp -= damage;
        hp = Mathf.Max(hp, 0);

        //확인용
        Debug.Log($"Player HP : {hp} / {maxHp}");

        if (hp <= 0)
        {
            Die();
            return;
        }

        IsHit = true;
        animator.SetTrigger("Hit");
        StartCoroutine(Hit());
    }
    private IEnumerator Hit()
    {
        yield return new WaitForSeconds(hitDuration);
        IsHit = false;
        controller.ResetAnimation();
    }
    private void Die()
    {
        combat.CancelAttack();
        IsDead = true;
        animator.SetTrigger("Death");
        //확인용
        Debug.Log("Player Dead");
    }


    //테스트용
    [ContextMenu("Test Damage")]
    private void TestDamage()
    {
        TakeDamage(20);
    }
}
