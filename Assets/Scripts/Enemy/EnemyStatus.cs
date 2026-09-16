using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    [SerializeField] private int maxHp = 30;

    private int hp;

    private void Awake()
    {
        hp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        hp = Mathf.Max(hp, 0);

        //확인용
        Debug.Log($"Enemy HP : {hp} / {maxHp}");

        if (hp <= 0) Destroy(gameObject);
    }
}
