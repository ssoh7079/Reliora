using UnityEngine;

public class ArtifactSkillAnimationEvent : MonoBehaviour
{
    private ArtifactSkillEffect effect;

    public void Initialize(ArtifactSkillEffect skillEffect)
    {
        effect = skillEffect;
    }

    public void SkillHit()
    {
        if (effect == null) return;
        effect.SkillHit();
    }
    public void SkillEnd()
    {
        if (effect == null) return;
        effect.SkillEnd();
    }
}
