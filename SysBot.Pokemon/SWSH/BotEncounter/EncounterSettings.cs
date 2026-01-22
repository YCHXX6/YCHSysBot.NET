using SysBot.Base;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace SysBot.Pokemon;

public class EncounterSettings : IBotStateSettings, ICountSettings
{
    private const string Counts = "计数";
    private const string Encounter = "遭遇";
    private const string Settings = "设置";
    public override string ToString() => "遭遇机器人 (SWSH) 设置";

    [Category(Encounter), DisplayName("遭遇方式"), Description("线路与重置机器人用于遭遇宝可梦的方法。")]
    public EncounterMode EncounteringType { get; set; } = EncounterMode.VerticalLine;

    [Category(Settings), DisplayName("化石设置")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public FossilSettings Fossil { get; set; } = new();

    [Category(Encounter), DisplayName("匹配后继续"), Description("启用后，找到合适匹配时机器人将继续运行。")]
    public ContinueAfterMatch ContinueAfterMatch { get; set; } = ContinueAfterMatch.StopExit;

    [Category(Encounter), DisplayName("关闭屏幕"), Description("启用时，正常机器人循环运行期间会关闭屏幕以节省电力。")]
    public bool ScreenOff { get; set; }

    private int _completedWild;
    private int _completedLegend;
    private int _completedEggs;
    private int _completedFossils;

    [Category(Counts), DisplayName("已遭遇（野生）"), Description("遭遇到的野生宝可梦数量。")]
    public int CompletedEncounters
    {
        get => _completedWild;
        set => _completedWild = value;
    }

    [Category(Counts), DisplayName("已遭遇（传说）"), Description("遭遇到的传说宝可梦数量。")]
    public int CompletedLegends
    {
        get => _completedLegend;
        set => _completedLegend = value;
    }

    [Category(Counts), DisplayName("已获得的蛋"), Description("已获得的蛋的数量。")]
    public int CompletedEggs
    {
        get => _completedEggs;
        set => _completedEggs = value;
    }

    [Category(Counts), DisplayName("已复活化石"), Description("已复活的化石宝可梦数量。")]
    public int CompletedFossils
    {
        get => _completedFossils;
        set => _completedFossils = value;
    }

    [Category(Counts), DisplayName("状态检查时发送计数"), Description("启用时，当请求状态检查时会输出这些计数。")]
    public bool EmitCountsOnStatusCheck { get; set; }

    public int AddCompletedEncounters() => Interlocked.Increment(ref _completedWild);
    public int AddCompletedLegends() => Interlocked.Increment(ref _completedLegend);
    public int AddCompletedEggs() => Interlocked.Increment(ref _completedEggs);
    public int AddCompletedFossils() => Interlocked.Increment(ref _completedFossils);

    public IEnumerable<string> GetNonZeroCounts()
    {
        if (!EmitCountsOnStatusCheck)
            yield break;
        if (CompletedEncounters != 0)
            yield return $"野外遭遇: {CompletedEncounters}";
        if (CompletedLegends != 0)
            yield return $"传说遭遇: {CompletedLegends}";
        if (CompletedEggs != 0)
            yield return $"已获得的蛋: {CompletedEggs}";
        if (CompletedFossils != 0)
            yield return $"已复活化石: {CompletedFossils}";
    }
}
