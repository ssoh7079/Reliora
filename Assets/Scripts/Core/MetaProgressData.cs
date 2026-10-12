using System;
using UnityEngine;

public class MetaProgressData
{
    private const int MaxLevel = 5;

    private readonly int[] upgradeCosts = { 10, 20, 35, 55, 80 };

    private const string CurrencyKey = "Meta_Currency";
    private const string HpKey = "Meta_Hp";
    private const string AttackKey = "Meta_Attack";
    private const string DefenseKey = "Meta_Defense";
    private const string DashKey = "Meta_Dash";
    private const string SkillCoolKey = "Meta_SkillCool";

    private int currency;

    private int hpLevel;
    private int attackLevel;
    private int defenseLevel;
    private int dashLevel;
    private int skillCoolLevel;

    public int Currency => currency;

    public int MaxHpBonus => hpLevel * 10;
    public int AttackBonus => attackLevel;
    public int DefenseBonus => defenseLevel;
    public float DashCoolReduction => dashLevel * 0.08f;
    public float SkillCoolMultiplier => Mathf.Clamp(1.0f - skillCoolLevel * 0.1f, 0.5f, 1.0f);

    public int TraitMaxLevel => MaxLevel;

    public event Action OnChanged;

    public void Load()
    {
        currency = PlayerPrefs.GetInt(CurrencyKey, 0);
        hpLevel = PlayerPrefs.GetInt(HpKey, 0);
        attackLevel = PlayerPrefs.GetInt(AttackKey, 0);
        defenseLevel = PlayerPrefs.GetInt(DefenseKey, 0);
        dashLevel = PlayerPrefs.GetInt(DashKey, 0);
        skillCoolLevel = PlayerPrefs.GetInt(SkillCoolKey, 0);
    }
    public void AddCurrency(int amount)
    {
        if (amount <= 0) return;
        currency += amount;
        Save();
        OnChanged?.Invoke();
    }
    public int GetLevel(TraitType type)
    {
        switch (type)
        {
            case TraitType.MaxHp:
                return hpLevel;
            case TraitType.Attack:
                return attackLevel;
            case TraitType.Defense:
                return defenseLevel;
            case TraitType.DashCoolTime:
                return dashLevel;
            case TraitType.SkillCoolTime:
                return skillCoolLevel;
        }
        return 0;
    }
    public int GetUpgradeCost(TraitType type)
    {
        int level = GetLevel(type);
        if (level >= MaxLevel) return 0;
        return upgradeCosts[level];
    }
    public bool Upgrade(TraitType type)
    {
        int level = GetLevel(type);
        if (level >= MaxLevel) return false;

        int cost = upgradeCosts[level];
        if (currency < cost) return false;

        currency -= cost;
        SetLevel(type, level + 1);

        Save();
        OnChanged?.Invoke();
        return true;
    }
    public bool Downgrade(TraitType type)
    {
        int level = GetLevel(type);
        if (level <= 0) return false;
        //현재 레벨을 올릴 때 사용했던 비용 반환
        int refund = upgradeCosts[level - 1];
        currency += refund;
        SetLevel(type, level - 1);

        Save();
        OnChanged?.Invoke();
        return true;
    }
    private void SetLevel(TraitType type, int level)
    {
        level = Mathf.Clamp(level, 0, MaxLevel);
        switch (type)
        {
            case TraitType.MaxHp:
                hpLevel = level;
                break;
            case TraitType.Attack:
                attackLevel = level;
                break;
            case TraitType.Defense:
                defenseLevel = level;
                break;
            case TraitType.DashCoolTime:
                dashLevel = level;
                break;
            case TraitType.SkillCoolTime:
                skillCoolLevel = level;
                break;
        }
    }
    private void Save()
    {
        PlayerPrefs.SetInt(CurrencyKey, currency);
        PlayerPrefs.SetInt(HpKey, hpLevel);
        PlayerPrefs.SetInt(AttackKey, attackLevel);
        PlayerPrefs.SetInt(DefenseKey, defenseLevel);
        PlayerPrefs.SetInt(DashKey, dashLevel);
        PlayerPrefs.SetInt(SkillCoolKey, skillCoolLevel);
        PlayerPrefs.Save();
    }


    public void ResetData()
    {
        currency = 0;

        hpLevel = 0;
        attackLevel = 0;
        defenseLevel = 0;
        dashLevel = 0;
        skillCoolLevel = 0;

        PlayerPrefs.DeleteKey(CurrencyKey);
        PlayerPrefs.DeleteKey(HpKey);
        PlayerPrefs.DeleteKey(AttackKey);
        PlayerPrefs.DeleteKey(DefenseKey);
        PlayerPrefs.DeleteKey(DashKey);
        PlayerPrefs.DeleteKey(SkillCoolKey);

        PlayerPrefs.Save();

        OnChanged?.Invoke();

        Debug.Log("영구 성장 데이터 초기화");
    }
}
