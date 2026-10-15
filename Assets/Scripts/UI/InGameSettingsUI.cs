using UnityEngine;
using UnityEngine.EventSystems;

public class InGameSettingsUI : MonoBehaviour
{
    [SerializeField] private GameObject settingsOverlay;


    private void Awake()
    {
        Time.timeScale = 1.0f;
        if (settingsOverlay != null) settingsOverlay.SetActive(false);
    }

    public void OpenSettings()
    {
        if (settingsOverlay == null) return;
        settingsOverlay.SetActive(true);
        Time.timeScale = 0.0f;
        if (EventSystem.current != null)
        {
            EventSystem.current.sendNavigationEvents = false;
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    public void CloseSettings()
    {
        if (settingsOverlay == null) return;
        settingsOverlay.SetActive(false);
        ResumeGame();
    }
    public void ReturnToTown()
    {
        if (GameManager.Instance == null) return;
        ResumeGame();
        GameManager.Instance.ReturnToTown();
    }
    public void QuitGame()
    {
        ResumeGame();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    private void ResumeGame()
    {
        Time.timeScale = 1.0f;
        if (EventSystem.current != null)
        {
            EventSystem.current.sendNavigationEvents = true;
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    private void OnDisable()
    {
        ResumeGame();
    }
}
