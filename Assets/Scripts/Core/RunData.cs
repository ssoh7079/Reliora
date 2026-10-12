public class RunData
{
    private const int RoomReward = 5;
    private const int BossReward = 20;

    public bool IsRunning { get; private set; }
    public bool IsFinished { get; private set; }

    public int CombatRoomClearCount { get; private set; }
    public bool BossCleared { get; private set; }

    public int EarnedCurrency { get; private set; }


    public void StartRun()
    {
        Reset();
        IsRunning = true;
    }
    public void AddCombatRoomClear()
    {
        if (!IsRunning) return;
        if (IsFinished) return;
        CombatRoomClearCount++;
        EarnedCurrency += RoomReward;
    }
    public int FinishRun(bool bossCleared)
    {
        if (!IsRunning) return 0;
        if (IsFinished) return 0;

        BossCleared = bossCleared;
        if (bossCleared) EarnedCurrency += BossReward;

        IsRunning = false;
        IsFinished = true;
        return EarnedCurrency;
    }
    public void Reset()
    {
        IsRunning = false;
        IsFinished = false;

        CombatRoomClearCount = 0;
        BossCleared = false;

        EarnedCurrency = 0;
    }
}
