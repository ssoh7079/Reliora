using UnityEngine;

public class EnemyAnimationEvent : MonoBehaviour
{
    private EnemyController controller;

    private void Awake()
    {
        controller = GetComponentInParent<EnemyController>();
    }

    public void AttackHit()
    {
        controller.AttackHit();
    }
    public void AttackEnd()
    {
        controller.AttackEnd();
    }
    public void DieEnd()
    {
        controller.DieEnd();
    }
}
