using UnityEngine;
using UnityEngine.InputSystem;

public class RoomExit : MonoBehaviour
{
    [SerializeField] private StageController stageController;

    private bool isPlayerInside;

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
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() == null) return;
        isPlayerInside = false;
    }
}
