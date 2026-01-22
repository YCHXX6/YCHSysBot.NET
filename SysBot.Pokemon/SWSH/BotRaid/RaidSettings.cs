using PKHeX.Core;
using SysBot.Base;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace SysBot.Pokemon;

public class RaidSettings : IBotStateSettings, ICountSettings
{
    private const string Hosting = "主机";
    private const string Counts = "计数";
    private const string FeatureToggle = "功能切换";
    public override string ToString() => "突袭机器人 设置";

    [Category(Hosting), DisplayName("等待时间 (秒)"), Description("尝试开始突袭前等待的秒数。范围 0 到 180 秒。")]
    public int TimeToWait { get; set; } = 90;

    [Category(Hosting), DisplayName("最小联机码"), Description("用于主办突袭的最小联机码。设置为 -1 则不使用联机码。")]
    public int MinRaidCode { get; set; } = 8180;

    [Category(Hosting), DisplayName("最大联机码"), Description("用于主办突袭的最大联机码。设置为 -1 则不使用联机码。")]
    public int MaxRaidCode { get; set; } = 8199;

    [Category(FeatureToggle), DisplayName("突袭描述"), Description("机器人主办的突袭的可选描述。留空时使用自动宝可梦检测。")]
    public string RaidDescription { get; set; } = string.Empty;

    [Category(FeatureToggle), DisplayName("回显队伍锁定"), Description("当每个队员锁定宝可梦时回显信息。")]
    public bool EchoPartyReady { get; set; }

    [Category(FeatureToggle), DisplayName("好友码"), Description("如果设置，允许机器人回显你的好友码。")]
    public string FriendCode { get; set; } = string.Empty;

    [Category(Hosting), DisplayName("每次接受好友数量"), Description("每次接受的好友请求数量。")]
    public int NumberFriendsToAdd { get; set; }

    [Category(Hosting), DisplayName("每次删除好友数量"), Description("每次删除的好友数量。")]
    public int NumberFriendsToDelete { get; set; }

    [Category(Hosting), DisplayName("首次要主办的突袭数量"), Description("在尝试添加/删除好友之前要主办的突袭次数。设置为 1 将先主办一次突袭，然后开始添加/删除好友。")]
    public int InitialRaidsToHost { get; set; }

    [Category(Hosting), DisplayName("尝试添加好友之间的突袭次数"), Description("在尝试添加好友之间要主办的突袭次数。")]
    public int RaidsBetweenAddFriends { get; set; }

    [Category(Hosting), DisplayName("尝试删除好友之间的突袭次数"), Description("在尝试删除好友之间要主办的突袭次数。")]
    public int RaidsBetweenDeleteFriends { get; set; }

    [Category(Hosting), DisplayName("开始添加好友的行号"), Description("开始尝试添加好友的行号。")]
    public int RowStartAddingFriends { get; set; } = 1;

    [Category(Hosting), DisplayName("开始删除好友的行号"), Description("开始尝试删除好友的行号。")]
    public int RowStartDeletingFriends { get; set; } = 1;

    [Category(Hosting), DisplayName("Switch 资料档编号"), Description("用于管理好友的 Nintendo Switch 资料档编号。例如，使用第二个资料档则设置为 2。")]
    public int ProfileNumber { get; set; } = 1;

    [Category(FeatureToggle), DisplayName("关闭屏幕"), Description("启用时，正常机器人循环运行期间会关闭屏幕以节省电力。")]
    public bool ScreenOff { get; set; }

    /// <summary>
    /// Gets a random trade code based on the range settings.
    /// </summary>
    public int GetRandomRaidCode() => Util.Rand.Next(MinRaidCode, MaxRaidCode + 1);

    private int _completedRaids;

    [Category(Counts), DisplayName("已开始的突袭"), Description("已启动的突袭数量。")]
    public int CompletedRaids
    {
        get => _completedRaids;
        set => _completedRaids = value;
    }

    [Category(Counts), DisplayName("状态检查时发送计数"), Description("启用时，当请求状态检查时会输出这些计数。")]
    public bool EmitCountsOnStatusCheck { get; set; }

    public int AddCompletedRaids() => Interlocked.Increment(ref _completedRaids);

    public IEnumerable<string> GetNonZeroCounts()
    {
        if (!EmitCountsOnStatusCheck)
            yield break;
        if (CompletedRaids != 0)
            yield return $"已开始的突袭: {CompletedRaids}";
    }
}
