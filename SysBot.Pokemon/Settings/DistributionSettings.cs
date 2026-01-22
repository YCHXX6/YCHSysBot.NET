using PKHeX.Core;
using SysBot.Base;
using System.ComponentModel;

namespace SysBot.Pokemon;

public class DistributionSettings : ISynchronizationSetting
{
    private const string Distribute = "分发";
    private const string Synchronize = "同步";
    public override string ToString() => "分发 交易 设置";

    // Distribute

    [Category(Distribute), DisplayName("空闲时分发"), Description("启用时，空闲的 LinkTrade 机器人会从分发文件夹中随机分发 PKM 文件。")]
    public bool DistributeWhileIdle { get; set; } = true;

    [Category(Distribute), DisplayName("分发文件夹随机化"), Description("启用时，分发文件夹将随机产出而不是按序列。")]
    public bool Shuffled { get; set; }

    [Category(Distribute), DisplayName("Ledy 限定物种"), Description("当设置为非 None 时，随机交易将要求匹配此物种以及昵称匹配。")]
    public Species LedySpecies { get; set; } = Species.None;

    [Category(Distribute), DisplayName("Ledy 无匹配时退出"), Description("当设置为 true 时，随机 Ledy 昵称交换交易在无匹配时将退出，而不是从池中交易随机实体。")]
    public bool LedyQuitIfNoMatch { get; set; }

    [Category(Distribute), DisplayName("分发交易联机码"), Description("分发交易的联机码。")]
    public int TradeCode { get; set; } = 7196;

    [Category(Distribute), DisplayName("使用随机联机码范围"), Description("分发交易的联机码将使用最小和最大范围而不是固定联机码。")]
    public bool RandomCode { get; set; }

    [Category(Distribute), DisplayName("BDSP 保持在联机室"), Description("对于 BDSP，分发机器人将进入特定房间并停留直到机器人停止。")]
    public bool RemainInUnionRoomBDSP { get; set; } = true;

    // Synchronize

    [Category(Synchronize), DisplayName("同步分发机器人"), Description("Link Trade：使用多个分发机器人 —— 所有机器人将同时确认联机码。在本地（Local）模式下，当所有机器人到达屏障时继续。在远程（Remote）模式下，需由外部信号使机器人继续。")]
    public BotSyncOption SynchronizeBots { get; set; } = BotSyncOption.LocalSync;

    [Category(Synchronize), DisplayName("同步延迟 (毫秒)"), Description("Link Trade：使用多个分发机器人 —— 当所有机器人准备好确认联机码后，Hub 将等待 X 毫秒再释放所有机器人。")]
    public int SynchronizeDelayBarrier { get; set; }

    [Category(Synchronize), DisplayName("同步超时 (秒)"), Description("Link Trade：使用多个分发机器人 —— 在继续前，机器人等待同步的最长时间（秒）。")]
    public double SynchronizeTimeout { get; set; } = 90;
}
