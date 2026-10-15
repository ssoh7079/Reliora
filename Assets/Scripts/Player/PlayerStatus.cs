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

    [Header("사운드")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSFX;

    private PlayerController controller;
    private PlayerCombat combat;
    private MetaProgressData metaProgress;

    private int hp;
    private int previousMaxHp;

    private int artifactDefenseBonus;
    private int artifactMaxHpBonus;

    public int Hp => hp;
    public int MaxHp => maxHp + MetaMaxHpBonus + artifactMaxHpBonus;
    public int Defense => defense + MetaDefenseBonus + artifactDefenseBonus;

    public bool IsHit {  get; private set; }
    public bool IsDead {  get; private set; }

    public event Action OnDead;

    private int MetaMaxHpBonus
    {
        get
        {
            if (GameManager.Instance == null || GameManager.Instance.MetaProgress == null) return 0;
            return GameManager.Instance.MetaProgress.MaxHpBonus;
        }
    }
    private int MetaDefenseBonus
    {
        get
        {
            if (GameManager.Instance == null || GameManager.Instance.MetaProgress == null) return 0;
            return GameManager.Instance.MetaProgress.DefenseBonus;
        }
    }


    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        combat = GetComponent<PlayerCombat>();
        hp = MaxHp;
        previousMaxHp = MaxHp;
    }
    private void Start()
    {
        if (GameManager.Instance == null) return;

        metaProgress = GameManager.Instance.MetaProgress;
        if (metaProgress == null) return;

        metaProgress.OnChanged += RefreshMetaStats;
        RefreshMetaStats();
    }
    private void OnDestroy()
    {
        if (metaProgress != null)
        {
            metaProgress.OnChanged -= RefreshMetaStats;
        }
    }

    private void RefreshMetaStats()
    {
        int newMaxHp = MaxHp;
        int difference = newMaxHp - previousMaxHp;
        //최대 체력이 증가하면 현재 체력도 증가량만큼 같이 올림
        if (difference > 0) hp += difference;
        //최대 체력이 내려갔을 경우 초과 체력 방지
        hp = Mathf.Min(hp, newMaxHp);
        previousMaxHp = newMaxHp;
    }
    public void TakeDamage(int damage)
    {
        if (IsDead || IsHit) return;
        //공격 중이었다면 강제 종료
        combat.CancelAttack();

        int finalDamage = Mathf.Max(damage - Defense, 1);
        hp -= finalDamage;
        hp = Mathf.Max(hp, 0);

        if (audioSource != null && hitSFX != null) audioSource.PlayOneShot(hitSFX);

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
        if (GameManager.Instance != null) GameManager.Instance.FinishRun(false);
        OnDead?.Invoke();
    }
    public void Heal(int amount)
    {
        if (IsDead) return;
        if (amount <= 0) return;

        hp += amount;
        hp = Mathf.Min(hp, MaxHp);

        //확인용
        Debug.Log($"Player Heal : {hp} / {MaxHp}");
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
