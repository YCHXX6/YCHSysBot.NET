using PKHeX.Core;
using SysBot.Base;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace SysBot.Pokemon;

public class TradeSettings : IBotStateSettings, ICountSettings
{
    private const string TradeCode = "交易码";
    private const string TradeConfig = "交易配置";
    private const string Dumping = "导出";
    private const string Counts = "计数";
    public override string ToString() => "交易机器人 设置";

    [Category(TradeConfig), DisplayName("等待交易伙伴时间 (秒)"), Description("等待交易伙伴的时间（秒）。")]
    public int TradeWaitTime { get; set; } = 30;

    [Category(TradeConfig), DisplayName("最长确认时间 (秒)"), Description("按 A 等待交易处理的最长时间（秒）。")]
    public int MaxTradeConfirmTime { get; set; } = 25;

    [Category(TradeCode), DisplayName("最小联机码"), Description("最小联机码。")]
    public int MinTradeCode { get; set; } = 0;

    [Category(TradeCode), DisplayName("最大联机码"), Description("最大联机码。")]
    public int MaxTradeCode { get; set; } = 9999_9999;

    [Category(Dumping), DisplayName("单用户最大导出次数"), Description("导出操作：当单个用户的导出次数达到此最大值时，导出流程将停止。")]
    public int MaxDumpsPerTrade { get; set; } = 20;

    [Category(Dumping), DisplayName("导出交易最大时间 (秒)"), Description("导出操作：导出流程在交易中花费超过此秒数后将停止。")]
    public int MaxDumpTradeTime { get; set; } = 180;

    [Category(Dumping), DisplayName("导出时输出合法性检查"), Description("导出操作：启用时，会向用户输出合法性检查信息。")]
    public bool DumpTradeLegalityCheck { get; set; } = true;

    [Category(TradeConfig), DisplayName("关闭屏幕"), Description("启用时，正常机器人循环运行期间会关闭屏幕以节省电力。")]
    public bool ScreenOff { get; set; }

    [Category(TradeConfig), DisplayName("单次交易最大宝可梦数量"), Description("单次交易的最大宝可梦数量。若此配置小于 1，则批处理模式将被关闭。")]
    public int MaxPkmsPerTrade { get; set; } = 1;
    
    [Category(TradeConfig), DisplayName("禁止请求非原生宝可梦"), Description("启用时，禁止请求来自非原始来源的宝可梦。")]
    public bool DisallowNonNatives { get; set; } = true;

    [Category(TradeConfig), DisplayName("禁止请求有 HOME 追踪的宝可梦"), Description("启用时，若宝可梦具有 HOME 追踪器则禁止请求。")]
    public bool DisallowTracked { get; set; } = true;

    [Category(TradeConfig), DisplayName("拒绝会进化的宝可梦"), Description("启用时，若交易中提供的宝可梦会进化，机器人将自动取消交易。")]
    public bool DisallowTradeEvolve { get; set; } = true;

    /// <summary>
    /// Gets a random trade code based on the range settings.
    /// </summary>
    public int GetRandomTradeCode() => Util.Rand.Next(MinTradeCode, MaxTradeCode + 1);

    private int _completedSurprise;
    private int _completedDistribution;
    private int _completedTrades;
    private int _completedSeedChecks;
    private int _completedClones;
    private int _completedDumps;

    [Category(Counts), DisplayName("已完成的惊喜交易"), Description("已完成的惊喜交易次数。")]
    public int CompletedSurprise
    {
        get => _completedSurprise;
        set => _completedSurprise = value;
    }

    [Category(Counts), DisplayName("已完成的分发交易"), Description("已完成的分发交易次数。")]
    public int CompletedDistribution
    {
        get => _completedDistribution;
        set => _completedDistribution = value;
    }

    [Category(Counts), DisplayName("已完成的连线交易 (特定用户)"), Description("已完成的连线交易（特定用户）。")]
    public int CompletedTrades
    {
        get => _completedTrades;
        set => _completedTrades = value;
    }

    [Category(Counts), DisplayName("已完成的种子检查交易"), Description("已完成的种子检查交易次数。")]
    public int CompletedSeedChecks
    {
        get => _completedSeedChecks;
        set => _completedSeedChecks = value;
    }

    [Category(Counts), DisplayName("已完成的克隆交易 (特定用户)"), Description("已完成的克隆交易（特定用户）。")]
    public int CompletedClones
    {
        get => _completedClones;
        set => _completedClones = value;
    }

    [Category(Counts), DisplayName("已完成的导出交易 (特定用户)"), Description("已完成的导出交易（特定用户）。")]
    public int CompletedDumps
    {
        get => _completedDumps;
        set => _completedDumps = value;
    }

    [Category(Counts), DisplayName("状态检查时发送计数"), Description("启用时，当请求状态检查时会输出这些计数。")]
    public bool EmitCountsOnStatusCheck { get; set; }

    public void AddCompletedTrade() => Interlocked.Increment(ref _completedTrades);
    public void AddCompletedSeedCheck() => Interlocked.Increment(ref _completedSeedChecks);
    public void AddCompletedSurprise() => Interlocked.Increment(ref _completedSurprise);
    public void AddCompletedDistribution() => Interlocked.Increment(ref _completedDistribution);
    public void AddCompletedDumps() => Interlocked.Increment(ref _completedDumps);
    public void AddCompletedClones() => Interlocked.Increment(ref _completedClones);

    public IEnumerable<string> GetNonZeroCounts()
    {
        if (!EmitCountsOnStatusCheck)
            yield break;
        if (CompletedSeedChecks != 0)
            yield return $"种子检查交易: {CompletedSeedChecks}";
        if (CompletedClones != 0)
            yield return $"克隆交易: {CompletedClones}";
        if (CompletedDumps != 0)
            yield return $"导出交易: {CompletedDumps}";
        if (CompletedTrades != 0)
            yield return $"连线交易: {CompletedTrades}";
        if (CompletedDistribution != 0)
            yield return $"分发交易: {CompletedDistribution}";
        if (CompletedSurprise != 0)
            yield return $"惊喜交易: {CompletedSurprise}";
    }
}
