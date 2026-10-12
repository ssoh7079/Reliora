using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;

    [Header("결과 제목")]
    [SerializeField] private TMP_Text resultText;

    [Header("런 결과")]
    [SerializeField] private TMP_Text runResultText;

    [SerializeField] private int totalCombatRoomCount = 4;


    private void Awake()
    {
        resultPanel.SetActive(false);
    }

    public void ShowClear()
    {
        resultText.text = "!!STAGE CLEAR!!";
        RefreshRunResult();
        resultPanel.SetActive(true);
    }
    public void ShowDead()
    {
        resultText.text = "YOU DIED";
        RefreshRunResult();
        resultPanel.SetActive(true);
    }
    private void RefreshRunResult()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentRun == null)
        {
            runResultText.text =
                "전투방 클리어 : 0 / 4\n\n" + "보스 처치 : 실패\n\n" + "획득 크리스탈 : + 0";
            return;
        }
        RunData run = GameManager.Instance.CurrentRun;
        string bossResult = run.BossCleared ? "완료" : "실패";
        runResultText.text =
            $"전투방 클리어 : {run.CombatRoomClearCount} / {totalCombatRoomCount}\n\n" +
            $"보스 처치 : {bossResult}\n\n" +
            $"획득 크리스탈 : + {run.EarnedCurrency}";
    }
    public void ReturnToTown()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToTown();
            return;
        }
        //Stage01Scene을 실행해서 테스트했을 경우
        SceneManager.LoadScene("TownScene");
    }
}
