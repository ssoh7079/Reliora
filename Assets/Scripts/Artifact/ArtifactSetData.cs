using UnityEngine;

[CreateAssetMenu(fileName = "Artifact Set Data", menuName = "Artifact/Artifact Set Data")]
public class ArtifactSetData : ScriptableObject
{
    [Header("세트")]
    [SerializeField] private ArtifactSetType setType;
    [SerializeField] private string setName;

    [Header("필요 아티팩트")]
    [SerializeField] private ArtifactData firstArtifact;
    [SerializeField] private ArtifactData secondArtifact;

    [Header("스킬")]
    [SerializeField] private ArtifactSkillType skillType;
    [SerializeField] private string skillName;
    [SerializeField] private Sprite skillIcon;

    [TextArea]
    [SerializeField] private string skillDescription;

    [SerializeField] private int skillDamage;
    [SerializeField] private float skillCoolTime;


    public ArtifactSetType SetType => setType;
    public string SetName => setName;

    public ArtifactData FirstArtifact => firstArtifact;
    public ArtifactData SecondArtifact => secondArtifact;

    public ArtifactSkillType SkillType => skillType;
    public string SkillName => skillName;
    public Sprite SkillIcon => skillIcon;
    public string SkillDescription => skillDescription;

    public int SkillDamage => skillDamage;
    public float SkillCoolTime => skillCoolTime;
}
