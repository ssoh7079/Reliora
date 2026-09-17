using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("기본 능력치")]
    [SerializeField] private int maxHp = 30;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float moveSpeed = 2.0f;

    [Header("전투 설정")]
    [SerializeField] private Vector2 attackSize = new Vector2(1.2f, 1.0f);
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float detectRange = 6.0f;
    [SerializeField] private float attackCoolTime = 1.0f;

    public int MaxHp => maxHp;
    public int AttackDamage => attackDamage;
    public float MoveSpeed => moveSpeed;

    public Vector2 AttackSize => attackSize;
    public float AttackRange => attackRange;
    public float DetectRange => detectRange;
    public float AttackCoolTime => attackCoolTime;
}
