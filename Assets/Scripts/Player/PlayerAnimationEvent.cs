using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private PlayerCombat combat;

    private void Awake()
    {
        combat = GetComponentInParent<PlayerCombat>();
    }

    public void AttackHit()
    {
        combat.AttackHit();
    }
    public void AttackEnd()
    {
        combat.AttackEnd();
    }
}
