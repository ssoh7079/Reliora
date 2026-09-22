using UnityEngine;
using UnityEngine.InputSystem;

public class ArtifactChest : MonoBehaviour
{
    private CombatRoom combatRoom;
    private RewardRoom rewardRoom;

    private bool isPlayerInside;
    private bool isTaken;


    private void Awake()
    {
        rewardRoom = GetComponentInParent<RewardRoom>();
    }
    public void SetRoom(CombatRoom room)
    {
        combatRoom = room;
    }

    void Update()
    {
        if (!isPlayerInside || isTaken) return;
        if (Keyboard.current.fKey.wasPressedThisFrame) TakeReward();
    }

    private void TakeReward()
    {
        isTaken = true;
        //확인용
        Debug.Log("아티팩트 획득");
        if (combatRoom != null)
        {
            combatRoom.RewardTaken();
        }
        else if (rewardRoom != null)
        {
            rewardRoom.RewardTaken();
        }
            Destroy(gameObject);
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
