using System.Collections.Generic;
using UnityEngine;

public class ArtifactSkillBarUI : MonoBehaviour
{
    [SerializeField] private ArtifactSkillController skillController;
    [SerializeField] private Transform skillSlotRoot;
    [SerializeField] private ArtifactSkillSlotUI skillSlotPrefab;

    private readonly List<ArtifactSkillSlotUI> slots = new List<ArtifactSkillSlotUI>();

    private void OnEnable()
    {
        skillController.OnSkillsChanged += RefreshSkills;
    }
    private void Start()
    {
        RefreshSkills();
    }
    private void OnDisable()
    {
        skillController.OnSkillsChanged -= RefreshSkills;
    }
    private void Update()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            ArtifactSetData skill = skillController.ActiveSkills[i];
            slots[i].RefreshCooldown(skillController.GetRemainCoolTime(i), skill.SkillCoolTime);
        }
    }

    private void RefreshSkills()
    {
        foreach (ArtifactSkillSlotUI slot in slots)
        {
            if (slot != null) Destroy(slot.gameObject);
        }

        slots.Clear();

        int count = skillController.ActiveSkills.Count;
        skillSlotRoot.gameObject.SetActive(count > 0);
        for (int i = 0; i < count; i++)
        {
            ArtifactSkillSlotUI slot = Instantiate( skillSlotPrefab, skillSlotRoot);
            slot.Setup(skillController.ActiveSkills[i], skillController, i);
            slots.Add(slot);
        }
    }
}