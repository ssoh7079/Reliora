using UnityEngine;
using UnityEngine.InputSystem;

public class HealObject : MonoBehaviour
{
    [SerializeField] private float healRate = 0.6f;

    private HealRoom healRoom;
    private PlayerStatus playerStatus;

    private bool isPlayerInside;
    private bool isUsed;


    private void Awake()
    {
        healRoom = GetComponentInParent<HealRoom>();
    }
    void Update()
    {
        if (!isPlayerInside || isUsed) return;
        if (Keyboard.current.fKey.wasPressedThisFrame) Heal();
    }

    private void Heal()
    {
        if (playerStatus == null) return;

        isUsed = true;
        int healAmount = Mathf.RoundToInt(playerStatus.MaxHp * healRate);
        playerStatus.Heal(healAmount);
        healRoom.HealComplete();
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStatus status = other.GetComponentInParent<PlayerStatus>();
        if (status == null) return;
        playerStatus = status;
        isPlayerInside = true;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerStatus status = other.GetComponentInParent<PlayerStatus>();
        if (status == null) return;
        if (status != playerStatus) return;
        playerStatus = null;
        isPlayerInside = false;
    }
}
