using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHpUI : MonoBehaviour
{
    [SerializeField] private Image hpFill;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private CanvasGroup canvasGroup;

    private BossStatus bossStatus;


    void Start()
    {
        Hide();
    }
    void Update()
    {
        if (bossStatus == null)
        {
            bossStatus = FindFirstObjectByType<BossStatus>();
            if (bossStatus == null)
            {
                Hide();
                return;
            }
            Show();
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        float ratio = (float)bossStatus.CurrentHp / bossStatus.MaxHp;
        hpFill.fillAmount = ratio;
        hpText.text = $"보스 {bossStatus.CurrentHp} / {bossStatus.MaxHp}";
    }
    private void Show()
    {
        canvasGroup.alpha = 1.0f;
    }
    private void Hide()
    {
        canvasGroup.alpha = 0.0f;
    }
}
