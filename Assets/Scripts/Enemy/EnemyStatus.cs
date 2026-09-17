using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;

    private int currentHp;
    private EnemyController controller;

    public int CurrentHp => currentHp;
    public int MaxHp => enemyData.MaxHp;

    private void Awake()
    {
        controller = GetComponent<EnemyController>();
        currentHp = enemyData.MaxHp;
    }

    public void TakeDamage(int damage)
    {
        if (controller.State == EnemyState.Dead) return;

        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);

        //확인용
        Debug.Log($"Enemy HP : {currentHp} / {enemyData.MaxHp}");

        if (currentHp <= 0)
        {
            controller.Die();
            return;
        }
        controller.Hit();
    }
}
