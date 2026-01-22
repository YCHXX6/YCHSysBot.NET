using System.ComponentModel;

namespace SysBot.Pokemon;

public class SeedCheckSettings
{
    private const string FeatureToggle = "功能切换";
    public override string ToString() => "种子检查 设置";

    [Category(FeatureToggle), DisplayName("显示所有 Z3 结果"), Description("启用时，种子检查将返回所有可能的种子结果，而不是第一个有效匹配。")]
    public bool ShowAllZ3Results { get; set; }

    [Category(FeatureToggle), DisplayName("结果显示模式"), Description("决定返回最近的光闪帧、首个星形与方形光闪帧，或前三个光闪帧。")]
    public SeedCheckResults ResultDisplayMode { get; set; }
}

public enum SeedCheckResults
{
    ClosestOnly,            // Only gets the first shiny
    FirstStarAndSquare,     // Gets the first star shiny and first square shiny
    FirstThree,             // Gets the first three frames
}
