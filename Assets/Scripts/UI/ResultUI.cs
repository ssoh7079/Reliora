using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text resultText;


    private void Awake()
    {
        resultPanel.SetActive(false);
    }

    public void ShowClear()
    {
        resultText.text = "STAGE CLEAR!!!!!";
        resultPanel.SetActive(true);
    }
    public void ShowDead()
    {
        resultText.text = "YOU DIED";
        resultPanel.SetActive(true);
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
