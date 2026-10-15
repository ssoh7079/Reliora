using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpUI : MonoBehaviour
{
    [SerializeField] private Image hpFill;
    [SerializeField] private TMP_Text hpText;

    private PlayerStatus playerStatus;


    void Start()
    {
        playerStatus = FindFirstObjectByType<PlayerStatus>();
    }
    void Update()
    {
        if (playerStatus == null)
        {
            playerStatus = FindFirstObjectByType<PlayerStatus>();
            if (playerStatus == null) return;
        }

        float ratio = (float)playerStatus.Hp / playerStatus.MaxHp;
        hpFill.fillAmount = ratio;
        hpText.text = $"{playerStatus.Hp} / {playerStatus.MaxHp}";
    }
}
