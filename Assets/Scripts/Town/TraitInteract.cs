using UnityEngine;
using UnityEngine.InputSystem;

public class TraitInteract : MonoBehaviour
{
    [SerializeField] private GameObject prompt;
    [SerializeField] private TraitUI traitUI;

    [Header("사운드")]
    [SerializeField] private AudioClip interactionSFX;

    private PlayerController player;
    private AudioSource playerAudioSource;

    private void Awake()
    {
        if (prompt != null) prompt.SetActive(false);
    }
    void Update()
    {
        UpdatePrompt();
        if (player == null) return;
        if (Keyboard.current == null) return;
        if (!Keyboard.current.fKey.wasPressedThisFrame) return;

        PlayInteractionSFX();

        if (traitUI.IsOpen)
        {
            traitUI.Close();
        }
        else
        {
            traitUI.Open(player);
        }
    }

    private void PlayInteractionSFX()
    {
        if (playerAudioSource == null || interactionSFX == null) return;
        playerAudioSource.PlayOneShot(interactionSFX);
    }
    private void UpdatePrompt()
    {
        if (prompt == null) return;

        bool show = player != null && !traitUI.IsOpen;
        if (prompt.activeSelf != show) prompt.SetActive(show);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController target = other.GetComponentInParent<PlayerController>();
        if (target == null) return;

        player = target;
        playerAudioSource = target.GetComponent<AudioSource>();
        UpdatePrompt();
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController target = other.GetComponentInParent<PlayerController>();
        if (target == null || target != player) return;

        player = null;
        playerAudioSource = null;
        UpdatePrompt();
    }
}
