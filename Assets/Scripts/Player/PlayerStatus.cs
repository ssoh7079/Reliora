using System.Collections;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] private int maxHp = 100;
    [SerializeField] private float hitDuration = 0.6f;
    [SerializeField] private Animator animator;

    private PlayerController controller;

    private int hp;

    public int Hp => hp;
    public int MaxHp => maxHp;

    public bool IsHit {  get; private set; }
    public bool IsDead {  get; private set; }


    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        hp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead || IsHit) return;

        hp -= damage;
        hp = Mathf.Max(hp, 0);

        //확인용
        Debug.Log($"Player HP : {hp} / {maxHp}");

        if (hp <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(Hit());
    }
    private IEnumerator Hit()
    {
        IsHit = true;
        animator.SetTrigger("Hit");
        yield return new WaitForSeconds(hitDuration);
        IsHit = false;
        controller.ResetAnimation();
    }
    private void Die()
    {
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
