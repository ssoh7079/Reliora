using TMPro;
using UnityEngine;

public class GrowthCurrencyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text currencyText;

    private MetaProgressData metaProgress;

    void Start()
    {
        if (GameManager.Instance == null) return;
        metaProgress = GameManager.Instance.MetaProgress;
        if (metaProgress == null) return;
        metaProgress.OnChanged += Refresh;
        Refresh();
    }
    private void OnDestroy()
    {
        if (metaProgress != null) metaProgress.OnChanged -= Refresh;
    }

    private void Refresh()
    {
        if (metaProgress == null) return;
        currencyText.text = metaProgress.Currency.ToString();
    }
}
