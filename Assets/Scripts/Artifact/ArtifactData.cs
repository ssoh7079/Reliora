using UnityEngine;

[CreateAssetMenu(fileName = "Artifact Data", menuName = "Artifact/Artifact Data")]
public class ArtifactData : ScriptableObject
{
    [Header("기본 정보")]
    [SerializeField] private string artifactName;
    [SerializeField] private Sprite icon;

    [TextArea]
    [SerializeField] private string description;

    [Header("고유")]
    [SerializeField] private bool isUnique = true;

    [Header("세트")]
    [SerializeField] private ArtifactSetType setType;

    [Header("능력치")]
    [SerializeField] private int attackBonus;
    [SerializeField] private int defenseBonus;
    [SerializeField] private int maxHpBonus;
    //0.15 = 15%
    [SerializeField] private float attackSpeedBonus;


    public string ArtifactName => artifactName;
    public Sprite Icon => icon;
    public string Description => description;

    public bool IsUnique => isUnique;

    public ArtifactSetType SetType => setType;

    public int AttackBonus => attackBonus;
    public int DefenseBonus => defenseBonus;
    public int MaxHpBonus => maxHpBonus;
    public float AttackSpeedBonus => attackSpeedBonus;
}
