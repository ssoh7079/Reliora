using UnityEngine;

public class TraitUI : MonoBehaviour
{
    [SerializeField] private GameObject traitPanel;

    private PlayerController playerController;
    private PlayerCombat playerCombat;
    private Rigidbody2D playerRb;

    public bool IsOpen => traitPanel.activeSelf;

    private void Awake()
    {
        traitPanel.SetActive(false);
    }
    public void Open(PlayerController player)
    {
        if (player == null || IsOpen) return;

        playerController = player;
        playerCombat = player.GetComponent<PlayerCombat>();
        playerRb = player.GetComponent<Rigidbody2D>();
        //창을 열 때 진행 중인 행동 정리
        if (playerCombat != null) playerCombat.CancelAttack();
        if (playerController.IsDash) playerController.CancelDash();
        if (playerRb != null) playerRb.linearVelocity = new Vector2(0.0f, playerRb.linearVelocity.y);

        playerController.SetIdle();
        playerController.enabled = false;
        if (playerCombat != null) playerCombat.enabled = false;
        traitPanel.SetActive(true);
    }
    public void Close()
    {
        if (!IsOpen) return;
        traitPanel.SetActive(false);
        if (playerController != null) playerController.enabled = true;
        if (playerCombat != null) playerCombat.enabled = true;
    }
}
