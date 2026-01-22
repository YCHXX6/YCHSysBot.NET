using System.ComponentModel;

namespace SysBot.Pokemon;

public class FossilSettings
{
    private const string Fossil = "化石";
    private const string Counts = "计数";
    public override string ToString() => "化石机器人 设置";

    [Category(Fossil), DisplayName("目标化石种类"), Description("要寻找的化石宝可梦的种类。")]
    public FossilSpecies Species { get; set; } = FossilSpecies.Dracozolt;

    /// <summary>
    /// Toggle for injecting fossil pieces.
    /// </summary>
    [Category(Fossil), DisplayName("缺失时注入化石碎片"), Description("当化石位置为空时，是否注入化石碎片。")]
    public bool InjectWhenEmpty { get; set; }
}
