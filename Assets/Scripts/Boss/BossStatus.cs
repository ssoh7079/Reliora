using System;
using UnityEngine;

public class BossStatus : MonoBehaviour
{
    [SerializeField] private BossData bossData;

    private BossController controller;
    private int currentHp;

    public int CurrentHp => currentHp;
    public int MaxHp => bossData.MaxHp;


    private void Awake()
    {
        controller = GetComponent<BossController>();
        currentHp = bossData.MaxHp;
    }

    public void TakeDamage(int damage)
    {
        if (controller.State == BossState.Dead) return;

        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);

        //확인용
        Debug.Log($"Boss HP : {currentHp} / {bossData.MaxHp}");

        if (currentHp <= 0)
        {
            controller.Die();
            return;
        }

        controller.Hit();
    }
}
