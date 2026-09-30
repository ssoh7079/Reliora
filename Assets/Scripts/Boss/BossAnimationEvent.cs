using UnityEngine;

public class BossAnimationEvent : MonoBehaviour
{
    private BossController controller;


    private void Awake()
    {
        controller = GetComponentInParent<BossController>();
    }
    public void AttackHit()
    {
        if (controller == null) return;
        controller.AttackHit();
    }
    public void AttackEnd()
    {
        if (controller == null) return;
        controller.AttackEnd();
    }
    public void DieEnd()
    {
        if (controller == null) return;
        controller.DieEnd();
    }
}
