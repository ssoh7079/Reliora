using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private PlayerCombat combat;
    private PlayerController controller;

    private void Awake()
    {
        combat = GetComponentInParent<PlayerCombat>();
        controller = GetComponentInParent<PlayerController>();
    }

    public void AttackHit()
    {
        combat.AttackHit();
    }
    public void AttackEnd()
    {
        combat.AttackEnd();
    }
    public void Footstep()
    {
        controller?.PlayFootstepSFX();
    }
}
