using UnityEngine;

public class InGameSettingsUI : MonoBehaviour
{
    [SerializeField] private GameObject settingsOverlay;


    private void Awake()
    {
        if (settingsOverlay != null) settingsOverlay.SetActive(false);
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
    public void ReturnToTown()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.ReturnToTown();
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
