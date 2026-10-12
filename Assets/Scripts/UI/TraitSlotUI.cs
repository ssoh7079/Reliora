using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TraitSlotUI : MonoBehaviour
{
    [SerializeField] private TraitType traitType;

    [Header("텍스트")]
    [SerializeField] private TMP_Text effectText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text levelText;

    [Header("버튼")]
    [SerializeField] private Button minusButton;
    [SerializeField] private Button plusButton;

    private MetaProgressData metaProgress;

    void Start()
    {
        if (GameManager.Instance == null) return;

        metaProgress = GameManager.Instance.MetaProgress;
        if (metaProgress == null) return;

        minusButton.onClick.AddListener(Downgrade);
        plusButton.onClick.AddListener(Upgrade);

        metaProgress.OnChanged += Refresh;
        Refresh();
    }
    private void OnDestroy()
    {
        if (metaProgress != null) metaProgress.OnChanged -= Refresh;
    }

    private void Upgrade()
    {
        if (metaProgress == null) return;
        metaProgress.Upgrade(traitType);
    }
    private void Downgrade()
    {
        if (metaProgress == null) return;
        metaProgress.Downgrade(traitType);
    }
    private void Refresh()
    {
        if (metaProgress == null) return;

        int level = metaProgress.GetLevel(traitType);
        levelText.text = level.ToString();
        minusButton.interactable = level > 0;
        if (level >= metaProgress.TraitMaxLevel)
        {
            costText.text = "MAX";
            plusButton.interactable = false;
        }
        else
        {
            int cost = metaProgress.GetUpgradeCost(traitType);
            costText.text = $"비용 : {cost}";
            plusButton.interactable = metaProgress.Currency >= cost;
        }
        RefreshEffect(level);
    }
    private void RefreshEffect(int level)
    {
        switch (traitType)
        {
            case TraitType.MaxHp:
                effectText.text = $"최대 체력 + {level * 10}";
                break;
            case TraitType.Attack:
                effectText.text = $"공격력 + {level}";
                break;
            case TraitType.Defense:
                effectText.text = $"방어력 + {level}";
                break;
            case TraitType.DashCoolTime:
                effectText.text = $"대시 쿨타임 - {level * 0.2f:0.0}초";
                break;
            case TraitType.SkillCoolTime:
                effectText.text = $"스킬 쿨타임 - {level * 10}%";
                break;
        }
    }
}
