using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactSlotUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private GameObject equippedMark;

    private ArtifactData artifact;
    private ArtifactInventory inventory;
    private ArtifactInventoryUI inventoryUI;

    public void Setup(ArtifactData data, ArtifactInventory targetInventory, ArtifactInventoryUI targetUI)
    {
        artifact = data;
        inventory = targetInventory;
        inventoryUI = targetUI;

        button.onClick.RemoveAllListeners();

        if (artifact == null)
        {
            button.interactable = false;
            icon.sprite = null;
            icon.enabled = false;
            nameText.text = "";
            nameText.gameObject.SetActive(false);
            if (equippedMark != null) equippedMark.SetActive(false);
            return;
        }

        button.interactable = true;
        button.onClick.AddListener(SelectArtifact);

        bool hasIcon = artifact.Icon != null;

        icon.sprite = artifact.Icon;
        icon.enabled = hasIcon;
        //아이콘이 있으면 이름이 그림을 가리지 않음
        nameText.gameObject.SetActive(!hasIcon);
        nameText.text = artifact.ArtifactName;

        if (equippedMark != null) equippedMark.SetActive(inventory.IsEquipped(artifact));
    }
    private void SelectArtifact()
    {
        if (artifact == null) return;
        inventoryUI.SelectArtifact(artifact);
    }
}