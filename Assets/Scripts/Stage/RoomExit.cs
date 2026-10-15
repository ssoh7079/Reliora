using UnityEngine;
using UnityEngine.InputSystem;

public class RoomExit : MonoBehaviour
{
    [SerializeField] private StageController stageController;

    [SerializeField] private GameObject interactionPrompt;

    [Header("사운드")]
    [SerializeField] private AudioClip interactionSFX;

    private bool isPlayerInside;
    private AudioSource playerAudioSource;


    private void Awake()
    {
        if (interactionPrompt != null) interactionPrompt.SetActive(false);
    }
    void Update()
    {
        if (Time.timeScale == 0.0f) return;
        if (!isPlayerInside) return;
        if (Keyboard.current == null) return;
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (playerAudioSource != null && interactionSFX != null)
            {
                playerAudioSource.PlayOneShot(interactionSFX);
            }
            stageController.EnterNextRoom();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null) return;

        isPlayerInside = true;
        playerAudioSource = player.GetComponent<AudioSource>();
        if (interactionPrompt != null) interactionPrompt.SetActive(true);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null) return;

        isPlayerInside = false;
        playerAudioSource = null;
        if (interactionPrompt != null) interactionPrompt.SetActive(false);
    }
}
