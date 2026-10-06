using UnityEngine;

public class ArtifactSkillEffect : MonoBehaviour
{
    private ArtifactSkillController controller;

    private bool isHit;
    private bool isEnded;

    public ArtifactSetData SkillData { get; private set; }
    public Component Target { get; private set; }

    public Vector2 HitCenter { get; private set; }
    public Vector2 HitSize { get; private set; }
    public Vector2 CastOrigin { get; private set; }


    public void Initialize(
        ArtifactSkillController owner, ArtifactSetData skillData, 
        Component target, Vector2 hitCenter, Vector2 hitSize, Vector2 castOrigin)
    {
        controller = owner;

        SkillData = skillData;
        Target = target;

        HitCenter = hitCenter;
        HitSize = hitSize;
        CastOrigin = castOrigin;
    }

    public void SkillHit()
    {
        if (isHit) return;

        isHit = true;

        if (controller != null) controller.ResolveSkillHit(this);
    }
    public void SkillEnd()
    {
        if (isEnded) return;

        isEnded = true;
        if (controller != null) controller.EndSkillEffect(this);

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        //예상치 못하게 VFX가 먼저 삭제돼도 조작 잠금이 남아 있지 않게 처리
        if (isEnded) return;

        isEnded = true;
        if (controller != null) controller.EndSkillEffect(this);
    }
}
