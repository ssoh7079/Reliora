using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public RunData CurrentRun {  get; private set; }

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
    }

    public void EnterDungeon()
    {
        CurrentRun.StartRun();

        //확인용
        Debug.Log($"Run Start : {CurrentRun.IsRunning}");

        SceneManager.LoadScene(StageScene);
    }
    public void ReturnToTown()
    {
        CurrentRun.Reset();
        SceneManager.LoadScene(TownScene);
    }
}
