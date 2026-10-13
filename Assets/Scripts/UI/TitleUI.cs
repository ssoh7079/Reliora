using UnityEngine;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private GameObject settingsOverlay;


    private void Awake()
    {
        if (settingsOverlay != null) settingsOverlay.SetActive(false);
    }

    public void NewGame()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.MetaProgress == null) return;

        GameManager.Instance.MetaProgress.ResetData();
        GameManager.Instance.ReturnToTown();
    }
    public void LoadGame()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.MetaProgress == null) return;

        GameManager.Instance.MetaProgress.Load();
        GameManager.Instance.ReturnToTown();
    }
    public void OpenSettings()
    {
        if (settingsOverlay == null) return;
        settingsOverlay.SetActive(true);
    }
    public void CloseSettings()
    {
        if (settingsOverlay == null) return;
        settingsOverlay.SetActive(false);
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
