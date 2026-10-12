using UnityEngine;
using UnityEngine.InputSystem;

public class RoomExit : MonoBehaviour
{
    [SerializeField] private StageController stageController;

    [SerializeField] private GameObject interactionPrompt;

    private bool isPlayerInside;


    private void Awake()
    {
        if (interactionPrompt != null) interactionPrompt.SetActive(false);
    }
    void Update()
    {
        if (!isPlayerInside) return;
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            stageController.EnterNextRoom();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() == null) return;
        isPlayerInside = true;
        if (interactionPrompt != null) interactionPrompt.SetActive(true);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() == null) return;
        isPlayerInside = false;
        if (interactionPrompt != null) interactionPrompt.SetActive(false);
    }
}
