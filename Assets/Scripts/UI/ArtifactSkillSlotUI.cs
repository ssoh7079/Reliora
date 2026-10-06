using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactSkillSlotUI : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button button;

    [Header("스킬 정보")]
    [SerializeField] private Image skillIcon;
    [SerializeField] private TMP_Text skillNameText;
    [SerializeField] private TMP_Text keyText;

    [Header("쿨타임")]
    [SerializeField] private Image cooldownMask;
    [SerializeField] private TMP_Text cooldownText;

    private const float SlotHeight = 100.0f;


    public void Setup(ArtifactSetData data, ArtifactSkillController controller, int index)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => controller.UseSkill(index));

        bool hasIcon = data.SkillIcon != null;

        skillIcon.sprite = data.SkillIcon;
        skillIcon.enabled = hasIcon;

        skillNameText.text = data.SkillName;
        skillNameText.gameObject.SetActive(!hasIcon);

        keyText.text = (index + 1).ToString();

        cooldownMask.gameObject.SetActive(false);
        cooldownText.text = "";
    }
    public void RefreshCooldown(float remain, float total)
    {
        if (remain <= 0.0f || total <= 0.0f)
        {
            cooldownMask.gameObject.SetActive(false);
            cooldownText.text = "";
            return;
        }

        cooldownMask.gameObject.SetActive(true);
        float ratio = Mathf.Clamp01(remain / total);
        cooldownMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, SlotHeight * ratio);
        cooldownText.text = Mathf.CeilToInt(remain).ToString();
    }
}
