using System;
using System.Collections;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("기본 능력치")]
    [SerializeField] private int maxHp = 100;
    [SerializeField] private int defense = 0;

    [Header("피격")]
    [SerializeField] private float hitDuration = 0.35f;
    [SerializeField] private Animator animator;

    private PlayerController controller;
    private PlayerCombat combat;

    private int hp;

    private int artifactDefenseBonus;
    private int artifactMaxHpBonus;

    public int Hp => hp;
    public int MaxHp => maxHp + artifactMaxHpBonus;
    public int Defense => defense + artifactDefenseBonus;

    public bool IsHit {  get; private set; }
    public bool IsDead {  get; private set; }

    public event Action OnDead;


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

        int finalDamage = Mathf.Max(damage - Defense, 1);
        hp -= finalDamage;
        hp = Mathf.Max(hp, 0);

        //확인용
        Debug.Log($"Player HP : {hp} / {maxHp}" + $"Damage : {finalDamage}" + $"Defense : {Defense}");

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
        OnDead?.Invoke();
    }
    public void Heal(int amount)
    {
        if (IsDead) return;
        if (amount <= 0) return;

        hp += amount;
        hp = Mathf.Min(hp, maxHp);

        //확인용
        Debug.Log($"Player Heal : {hp} / {maxHp}");
    }
    public void SetArtifactStats(int defenseBonus, int maxHpBonus)
    {
        artifactDefenseBonus = Mathf.Max(defenseBonus, 0);
        artifactMaxHpBonus = Mathf.Max(maxHpBonus, 0);

        hp = Mathf.Min(hp, MaxHp);
        
        //확인용
        Debug.Log($"Artifact Status : HP {hp}/{MaxHp}, DEF {Defense}");
    }


    //테스트용
    [ContextMenu("Test Damage")]
    private void TestDamage()
    {
        TakeDamage(20);
    }
}
