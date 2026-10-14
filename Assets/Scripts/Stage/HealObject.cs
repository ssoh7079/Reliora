using UnityEngine;
using UnityEngine.InputSystem;

public class HealObject : MonoBehaviour
{
    [SerializeField] private float healRate = 0.6f;

    [SerializeField] private GameObject interactionPrompt;

    [Header("사운드")]
    [SerializeField] private AudioClip interactionSFX;

    private HealRoom healRoom;
    private PlayerStatus playerStatus;
    private AudioSource playerAudioSource;

    private bool isPlayerInside;
    private bool isUsed;


    private void Awake()
    {
        healRoom = GetComponentInParent<HealRoom>();
        if (interactionPrompt != null) interactionPrompt.SetActive(false);
    }
    void Update()
    {
        if (!isPlayerInside || isUsed) return;
        if (Keyboard.current == null) return;
        if (Keyboard.current.fKey.wasPressedThisFrame) Heal();
    }

    private void Heal()
    {
        if (playerStatus == null) return;

        isUsed = true;
        if (playerAudioSource != null && interactionSFX != null)
        {
            playerAudioSource.PlayOneShot(interactionSFX);
        }
        int healAmount = Mathf.RoundToInt(playerStatus.MaxHp * healRate);
        playerStatus.Heal(healAmount);
        if (interactionPrompt != null) interactionPrompt.SetActive(false);
        healRoom.HealComplete();
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStatus status = other.GetComponentInParent<PlayerStatus>();
        if (status == null) return;
        playerStatus = status;
        playerAudioSource = status.GetComponent<AudioSource>();
        isPlayerInside = true;
        if (interactionPrompt != null) interactionPrompt.SetActive(true);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerStatus status = other.GetComponentInParent<PlayerStatus>();
        if (status == null) return;
        if (status != playerStatus) return;
        playerStatus = null;
        playerAudioSource = null;
        isPlayerInside = false;
        if (interactionPrompt != null) interactionPrompt.SetActive(false);
    }
}
