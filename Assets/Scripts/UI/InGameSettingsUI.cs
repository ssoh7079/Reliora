using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InGameSettingsUI : MonoBehaviour
{
    [SerializeField] private GameObject settingsOverlay;

    [Header("다른 UI")]
    [SerializeField] private ArtifactInventoryUI inventoryUI;
    [SerializeField] private TraitUI traitUI;


    private void Awake()
    {
        Time.timeScale = 1.0f;
        inventoryUI = FindFirstObjectByType<ArtifactInventoryUI>();
        traitUI = FindFirstObjectByType<TraitUI>();
        if (settingsOverlay != null) settingsOverlay.SetActive(false);
    }
    private void Update()
    {
        if (Keyboard.current == null) return;
        if (!Keyboard.current.escapeKey.wasPressedThisFrame) return;
        //설정창이 열려 있으면 닫기
        if (settingsOverlay != null && settingsOverlay.activeSelf)
        {
            CloseSettings();
            return;
        }
        //가방이 열려 있으면 가방 닫기
        if (inventoryUI != null && inventoryUI.IsOpen)
        {
            inventoryUI.CloseInventory();
            return;
        }
        //특성창이 열려 있으면 특성창 닫기
        if (traitUI != null && traitUI.IsOpen)
        {
            traitUI.Close();
            return;
        }
        //아무 UI도 없을 때만 설정창 열기
        OpenSettings();
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
    public void ReturnToTitle()
    {
        if (GameManager.Instance == null) return;
        ResumeGame();
        GameManager.Instance.ReturnToTitle();
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
