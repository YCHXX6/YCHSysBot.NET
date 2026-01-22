using System;
using System.ComponentModel;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace SysBot.Pokemon;

public class QueueSettings
{
    private const string FeatureToggle = "功能切换";
    private const string UserBias = "用户偏好";
    private const string TimeBias = "时间偏好";
    private const string QueueToggle = "队列切换";
    public override string ToString() => "队列加入 设置";

    // General

    [Category(FeatureToggle), DisplayName("允许加入队列"), Description("是否允许用户加入队列。")]
    public bool CanQueue { get; set; } = true;

    [Category(FeatureToggle), DisplayName("最大队列人数"), Description("如果队列中已有该数量的用户，则阻止添加更多用户。")]
    public int MaxQueueCount { get; set; } = 999;

    [Category(FeatureToggle), DisplayName("允许处理中出队"), Description("允许正在交易的用户出队。")]
    public bool CanDequeueIfProcessing { get; set; }

    [Category(FeatureToggle), DisplayName("Flex 模式处理方式"), Description("决定 Flex 模式如何处理队列。")]
    public FlexYieldMode FlexMode { get; set; } = FlexYieldMode.Weighted;

    [Category(FeatureToggle), DisplayName("队列开关模式"), Description("决定何时打开或关闭队列。")]
    public QueueOpening QueueToggleMode { get; set; } = QueueOpening.Threshold;

    // Queue Toggle

    [Category(QueueToggle), DisplayName("阈值解锁人数"), Description("阈值模式：触发队列开启的用户数量。")]
    public int ThresholdUnlock { get; set; }

    [Category(QueueToggle), DisplayName("阈值锁定人数"), Description("阈值模式：触发队列关闭的用户数量。")]
    public int ThresholdLock { get; set; } = 30;

    [Category(QueueToggle), DisplayName("计划模式：开启持续秒数"), Description("计划模式：队列在被锁定前开放的秒数。")]
    public int IntervalOpenFor { get; set; } = 5 * 60;

    [Category(QueueToggle), DisplayName("计划模式：关闭持续秒数"), Description("计划模式：队列在解锁前关闭的秒数。")]
    public int IntervalCloseFor { get; set; } = 15 * 60;

    // Flex Users

    [Category(UserBias), Description("Biases the Trade Queue's weight based on how many users are in the queue.")]
    public int YieldMultCountTrade { get; set; } = 100;

    [Category(UserBias), Description("Biases the Seed Check Queue's weight based on how many users are in the queue.")]
    public int YieldMultCountSeedCheck { get; set; } = 100;

    [Category(UserBias), Description("Biases the Clone Queue's weight based on how many users are in the queue.")]
    public int YieldMultCountClone { get; set; } = 100;

    [Category(UserBias), Description("Biases the Dump Queue's weight based on how many users are in the queue.")]
    public int YieldMultCountDump { get; set; } = 100;

    // Flex Time

    [Category(TimeBias), Description("Determines whether the weight should be added or multiplied to the total weight.")]
    public FlexBiasMode YieldMultWait { get; set; } = FlexBiasMode.Multiply;

    [Category(TimeBias), Description("Checks time elapsed since the user joined the Trade queue, and increases the queue's weight accordingly.")]
    public int YieldMultWaitTrade { get; set; } = 1;

    [Category(TimeBias), Description("Checks time elapsed since the user joined the Seed Check queue, and increases the queue's weight accordingly.")]
    public int YieldMultWaitSeedCheck { get; set; } = 1;

    [Category(TimeBias), Description("Checks time elapsed since the user joined the Clone queue, and increases the queue's weight accordingly.")]
    public int YieldMultWaitClone { get; set; } = 1;

    [Category(TimeBias), Description("Checks time elapsed since the user joined the Dump queue, and increases the queue's weight accordingly.")]
    public int YieldMultWaitDump { get; set; } = 1;

    [Category(TimeBias), Description("Multiplies the amount of users in queue to give an estimate of how much time it will take until the user is processed.")]
    public float EstimatedDelayFactor { get; set; } = 1.1f;

    private int GetCountBias(PokeTradeType type) => type switch
    {
        PokeTradeType.Seed => YieldMultCountSeedCheck,
        PokeTradeType.Clone => YieldMultCountClone,
        PokeTradeType.Dump => YieldMultCountDump,
        _ => YieldMultCountTrade,
    };

    private int GetTimeBias(PokeTradeType type) => type switch
    {
        PokeTradeType.Seed => YieldMultWaitSeedCheck,
        PokeTradeType.Clone => YieldMultWaitClone,
        PokeTradeType.Dump => YieldMultWaitDump,
        _ => YieldMultWaitTrade,
    };

    /// <summary>
    /// Gets the weight of a <see cref="PokeTradeType"/> based on the count of users in the queue and time users have waited.
    /// </summary>
    /// <param name="count">Count of users for <see cref="type"/></param>
    /// <param name="time">Next-to-be-processed user's time joining the queue</param>
    /// <param name="type">Queue type</param>
    /// <returns>Effective weight for the trade type.</returns>
    public long GetWeight(int count, DateTime time, PokeTradeType type)
    {
        var now = DateTime.Now;
        var seconds = (now - time).Seconds;

        var cb = GetCountBias(type) * count;
        var tb = GetTimeBias(type) * seconds;

        return YieldMultWait switch
        {
            FlexBiasMode.Multiply => cb * tb,
            _ => cb + tb,
        };
    }

    /// <summary>
    /// Estimates the amount of time (minutes) until the user will be processed.
    /// </summary>
    /// <param name="position">Position in the queue</param>
    /// <param name="botct">Amount of bots processing requests</param>
    /// <returns>Estimated time in Minutes</returns>
    public float EstimateDelay(int position, int botct) => (EstimatedDelayFactor * position) / botct;
}

public enum FlexBiasMode
{
    Add,
    Multiply,
}

public enum FlexYieldMode
{
    LessCheatyFirst,
    Weighted,
}

public enum QueueOpening
{
    Manual,
    Threshold,
    Interval,
}
