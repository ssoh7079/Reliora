using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ArtifactInventoryUI : MonoBehaviour
{
    [Header("인벤토리")]
    [SerializeField] private ArtifactInventory inventory;

    [Header("패널")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject detailPanel;

    [Header("바깥 클릭 영역")]
    [SerializeField] private Button inventoryBackdrop;
    [SerializeField] private Button detailBackdrop;

    [Header("슬롯")]
    [SerializeField] private Transform slotRoot;
    [SerializeField] private ArtifactSlotUI slotPrefab;

    [Header("플레이어 스탯")]
    [SerializeField] private TMP_Text playerStatText;

    [Header("아티팩트 상세")]
    [SerializeField] private Image artifactIcon;
    [SerializeField] private TMP_Text artifactNameText;
    [SerializeField] private TMP_Text artifactStatText;
    [SerializeField] private TMP_Text setText;
    [SerializeField] private TMP_Text skillText;

    [Header("버튼")]
    [SerializeField] private Button equipButton;
    [SerializeField] private TMP_Text equipButtonText;
    [SerializeField] private Button dropButton;

    private PlayerStatus status;
    private PlayerCombat combat;

    private ArtifactData selectedArtifact;
    private ArtifactSlotUI[] slots;

    private int closeFrame = -1;

    // 창이 닫힌 프레임에도 공격 입력이 넘어가지 않게
    public bool BlocksAttack => inventoryPanel.activeSelf || closeFrame == Time.frameCount;


    private void Awake()
    {
        status = inventory.GetComponent<PlayerStatus>();
        combat = inventory.GetComponent<PlayerCombat>();

        CreateSlots();

        inventoryPanel.SetActive(false);
        detailPanel.SetActive(false);
        inventoryBackdrop.gameObject.SetActive(false);
        detailBackdrop.gameObject.SetActive(false);

        inventoryBackdrop.onClick.AddListener(CloseInventory);
        detailBackdrop.onClick.AddListener(CloseDetail);

        equipButton.onClick.AddListener(EquipOrUnequip);
        dropButton.onClick.AddListener(DropSelected);
    }
    private void OnEnable()
    {
        inventory.OnInventoryChanged += Refresh;
        inventory.OnEquipmentChanged += Refresh;
    }
    private void OnDisable()
    {
        inventory.OnInventoryChanged -= Refresh;
        inventory.OnEquipmentChanged -= Refresh;
    }
    private void Update()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.bKey.wasPressedThisFrame) ToggleInventory();
    }

    public void ToggleInventory()
    {
        // 상세창이 열려 있으면 상세창부터 닫기
        if (detailPanel.activeSelf)
        {
            CloseDetail();
            return;
        }
        if (inventoryPanel.activeSelf)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }
    private void OpenInventory()
    {
        inventoryBackdrop.gameObject.SetActive(true);
        inventoryPanel.SetActive(true);

        Refresh();
    }
    public void CloseInventory()
    {
        closeFrame = Time.frameCount;

        CloseDetail();

        inventoryPanel.SetActive(false);
        inventoryBackdrop.gameObject.SetActive(false);
    }
    public void CloseDetail()
    {
        detailPanel.SetActive(false);
        detailBackdrop.gameObject.SetActive(false);

        selectedArtifact = null;
    }
    private void CreateSlots()
    {
        slots = new ArtifactSlotUI[inventory.Capacity];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = Instantiate(slotPrefab, slotRoot);
        }
    }
    private void Refresh()
    {
        RefreshSlots();
        RefreshPlayerStats();

        if (selectedArtifact == null) return;
        if (!inventory.HasArtifact(selectedArtifact))
        {
            CloseDetail();
            return;
        }

        ShowDetail();
    }
    private void RefreshSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            ArtifactData artifact = null;
            if (i < inventory.Artifacts.Count) artifact = inventory.Artifacts[i];
            slots[i].Setup(artifact, inventory, this);
        }
    }
    private void RefreshPlayerStats()
    {
        playerStatText.text =
            $"체력: {status.Hp} / {status.MaxHp}\n\n" +
            $"공격력: {combat.AttackDamage}\n\n" +
            $"방어력: {status.Defense}\n\n" +
            $"공격속도: {combat.AttackSpeed:F2}\n\n" +
            $"착용 장비: {inventory.EquippedCount} / {inventory.MaxEquipCount}";
    }
    public void SelectArtifact(ArtifactData artifact)
    {
        if (artifact == null) return;

        selectedArtifact = artifact;
        ShowDetail();
    }
    private void ShowDetail()
    {
        if (selectedArtifact == null) return;

        detailBackdrop.gameObject.SetActive(true);
        detailPanel.SetActive(true);

        artifactNameText.text = selectedArtifact.IsUnique ? $"[고유] {selectedArtifact.ArtifactName}" : selectedArtifact.ArtifactName;

        if (selectedArtifact.Icon != null)
        {
            artifactIcon.sprite = selectedArtifact.Icon;
            artifactIcon.enabled = true;
        }
        else
        {
            artifactIcon.sprite = null;
            artifactIcon.enabled = false;
        }

        artifactStatText.text = MakeStatText(selectedArtifact);

        RefreshSetText();

        bool equipped = inventory.IsEquipped(selectedArtifact);
        equipButtonText.text = equipped ? "해제" : "장착";
        equipButton.interactable = equipped || inventory.CanEquip(selectedArtifact);
        // 고유 아이템도 버리기 가능
        dropButton.interactable = true;
    }
    private void RefreshSetText()
    {
        ArtifactSetData setData = inventory.GetSetData(selectedArtifact);
        if (setData == null)
        {
            setText.text = "";
            skillText.text = "";
            return;
        }

        bool firstOwned = inventory.HasArtifact(setData.FirstArtifact);
        bool secondOwned = inventory.HasArtifact(setData.SecondArtifact);
        bool setActive = inventory.IsSetActive(setData);
        
        string firstColor = firstOwned ? "#FFFFFF" : "#555555";
        string secondColor = secondOwned ? "#FFFFFF" : "#555555";
        // 두 아이템 모두 장착해야 세트 이름이 밝아짐
        string setColor = setActive ? "#FFFFFF" : "#555555";

        setText.text =
            $"<color={firstColor}>{setData.FirstArtifact.ArtifactName}</color>" +
            " + " +
            $"<color={secondColor}>{setData.SecondArtifact.ArtifactName}</color>" +
            " = " +
            $"<color={setColor}>{setData.SetName}</color>";

        skillText.text =
            $"세트 스킬 : {setData.SkillName}\n" +
            $"데미지 : {setData.SkillDamage}\n" +
            $"쿨타임 : {setData.SkillCoolTime:0.#}초";
    }
    private string MakeStatText(ArtifactData artifact)
    {
        StringBuilder text = new StringBuilder();
        if (artifact.AttackBonus != 0)
        {
            text.AppendLine($"공격력 +{artifact.AttackBonus}");
        }
        if (artifact.DefenseBonus != 0)
        {
            text.AppendLine($"방어력 +{artifact.DefenseBonus}");
        }
        if (artifact.MaxHpBonus != 0)
        {
            text.AppendLine($"최대 체력 +{artifact.MaxHpBonus}");
        }
        if (artifact.AttackSpeedBonus != 0.0f)
        {
            float percent = artifact.AttackSpeedBonus * 100.0f;
            text.AppendLine($"공격속도 +{percent:0}%");
        }
        return text.ToString();
    }
    private void EquipOrUnequip()
    {
        if (selectedArtifact == null) return;
        if (inventory.IsEquipped(selectedArtifact))
        {
            inventory.UnequipArtifact(selectedArtifact);
        }
        else
        {
            inventory.EquipArtifact(selectedArtifact);
        }
    }
    private void DropSelected()
    {
        if (selectedArtifact == null) return;

        ArtifactData dropArtifact = selectedArtifact;

        CloseDetail();

        inventory.DropArtifact(dropArtifact);
    }
}