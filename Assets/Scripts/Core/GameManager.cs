using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public RunData CurrentRun {  get; private set; }
    public MetaProgressData MetaProgress { get; private set; }

    private const string TownScene = "TownScene";
    private const string StageScene = "Stage01Scene";


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        CurrentRun = new RunData();
        MetaProgress = new MetaProgressData();
        MetaProgress.Load();
    }
    private void Update()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.escapeKey.wasPressedThisFrame) Application.Quit();
    }

    public void EnterDungeon()
    {
        CurrentRun.StartRun();

        //확인용
        Debug.Log($"Run Start : {CurrentRun.IsRunning}");

        SceneManager.LoadScene(StageScene);
    }
    public void FinishRun(bool bossCleared)
    {
        int reward = CurrentRun.FinishRun(bossCleared);
        //이미 정산된 런이면 중복 지급하지 않음
        if (reward <= 0) return;
        MetaProgress.AddCurrency(reward);
        //확인용
        Debug.Log(
            $"Run End / " +
            $"Room : {CurrentRun.CombatRoomClearCount} / " +
            $"Boss : {CurrentRun.BossCleared} / " +
            $"Reward : {CurrentRun.EarnedCurrency} / " +
            $"Total Currency : {MetaProgress.Currency}");
    }
    public void ReturnToTown()
    {
        CurrentRun.Reset();
        SceneManager.LoadScene(TownScene);
    }


    [ContextMenu("Reset Meta Progress")]
    private void ResetMetaProgress()
    {
        if (MetaProgress == null) return;
        MetaProgress.ResetData();
    }
}
