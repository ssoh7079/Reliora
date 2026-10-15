using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArtifactChest : MonoBehaviour
{
    [Header("보상 목록")]
    [SerializeField] private ArtifactData[] rewardPool;

    [Header("상호작용")]
    [SerializeField] private GameObject interactionPrompt;

    [Header("사운드")]
    [SerializeField] private AudioClip interactionSFX;

    private CombatRoom combatRoom;
    private RewardRoom rewardRoom;

    private ArtifactInventory inventory;
    private AudioSource playerAudioSource;

    private bool isPlayerInside;
    private bool isTaken;


    private void Awake()
    {
        rewardRoom = GetComponentInParent<RewardRoom>();
        if (interactionPrompt != null) interactionPrompt.SetActive(false);
    }
    public void SetRoom(CombatRoom room)
    {
        combatRoom = room;
    }

    void Update()
    {
        if (Time.timeScale == 0.0f) return;
        if (!isPlayerInside || isTaken) return;
        if (Keyboard.current == null) return;
        if (Keyboard.current.fKey.wasPressedThisFrame) TakeReward();
    }

    private void TakeReward()
    {
        if (inventory == null) return;

        ArtifactData artifact = GetRandomArtifact();
        //이미 6개를 전부 가지고 있다면, 방 진행만 가능하게 처리
        if (artifact == null)
        {
            PlayInteractionSFX();
            CompleteReward();
            return;
        }
        if (!inventory.AddArtifact(artifact)) return;
        PlayInteractionSFX();
        CompleteReward();
    }
    private void PlayInteractionSFX()
    {
        if (playerAudioSource == null || interactionSFX == null) return;
        playerAudioSource.PlayOneShot(interactionSFX);
    }
    private ArtifactData GetRandomArtifact()
    {
        if (rewardPool == null || rewardPool.Length == 0) return null;

        List<ArtifactData> candidates = new List<ArtifactData>();

        foreach (ArtifactData artifact in rewardPool)
        {
            if (artifact == null) continue;
            //고유 아이템이고 현재 보유 중이면 제외
            if (artifact.IsUnique && inventory.HasArtifact(artifact)) continue;
            candidates.Add(artifact);
        }
        if (candidates.Count == 0) return null;

        int index = Random.Range(0, candidates.Count);
        return candidates[index];
    }
    private void CompleteReward()
    {
        isTaken = true;

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
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
        ArtifactInventory playerInventory = other.GetComponentInParent<ArtifactInventory>();
        if (playerInventory == null) return;
        
        inventory = playerInventory;
        playerAudioSource = playerInventory.GetComponent<AudioSource>();
        isPlayerInside = true;

        if (interactionPrompt != null) interactionPrompt.SetActive(true);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        ArtifactInventory playerInventory = other.GetComponentInParent<ArtifactInventory>();
        if (playerInventory == null || playerInventory != inventory) return;

        inventory = null;
        playerAudioSource = null;
        isPlayerInside = false;

        if (interactionPrompt != null) interactionPrompt.SetActive(false);
    }
}
