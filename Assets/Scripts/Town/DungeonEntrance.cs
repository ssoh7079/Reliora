using UnityEngine;
using UnityEngine.InputSystem;

public class DungeonEntrance : MonoBehaviour
{
    private bool isPlayerInside;

    void Update()
    {
        if (!isPlayerInside) return;
        if (Keyboard.current.fKey.wasPressedThisFrame) GameManager.Instance.EnterDungeon();
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
