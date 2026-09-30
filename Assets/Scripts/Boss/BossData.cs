using UnityEngine;

[CreateAssetMenu(fileName = "Boss Data", menuName = "Boss/Boss Data")]
public class BossData : ScriptableObject
{
    [Header("기본 능력치")]
    [SerializeField] private int maxHp = 200;
    [SerializeField] private int attackDamage = 15;

    [Header("이동")]
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float patternRunSpeed = 7.0f;

    [Header("공격")]
    [SerializeField] private Vector2 attackSize = new Vector2(3.5f, 2.5f);
    [SerializeField] private float attackRange = 1.8f;
    [SerializeField] private float attackCoolTime = 1.0f;

    [Header("상태")]
    [SerializeField] private float readyTime = 0.6f;

    [Header("패턴")]
    [SerializeField] private float patternCoolTime = 4.0f;
    [SerializeField] private float patternReadyTime = 1.2f;
    [SerializeField] private float vanishTime = 1.2f;
    [SerializeField] private float appearDistance = 1.5f;

    public int MaxHp => maxHp;
    public int AttackDamage => attackDamage;

    public float MoveSpeed => moveSpeed;
    public float PatternRunSpeed => patternRunSpeed;

    public Vector2 AttackSize => attackSize;
    public float AttackRange => attackRange;
    public float AttackCoolTime => attackCoolTime;

    public float ReadyTime => readyTime;

    public float PatternCoolTime => patternCoolTime;
    public float PatternReadyTime => patternReadyTime;
    public float VanishTime => vanishTime;
    public float AppearDistance => appearDistance;
}
