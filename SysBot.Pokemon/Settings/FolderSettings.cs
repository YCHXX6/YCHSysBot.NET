using System.ComponentModel;
using System.IO;

namespace SysBot.Pokemon;

public class FolderSettings : IDumper
{
    private const string FeatureToggle = "功能切换";
    private const string Files = "文件";
    public override string ToString() => "文件 / 导出 设置";

    [Category(FeatureToggle), DisplayName("导出接收的 PKM 文件"), Description("启用时，会把收到的 PKM 文件（交易结果）导出到导出文件夹。")]
    public bool Dump { get; set; }

    [Category(Files), DisplayName("来源文件夹"), Description("来源文件夹：从此处选择要分发的 PKM 文件。")]
    public string DistributeFolder { get; set; } = string.Empty;

    [Category(Files), DisplayName("导出目标文件夹"), Description("目标文件夹：收到的所有 PKM 文件将导出到此处。")]
    public string DumpFolder { get; set; } = string.Empty;

    public void CreateDefaults(string path)
    {
        var dump = Path.Combine(path, "dump");
        Directory.CreateDirectory(dump);
        DumpFolder = dump;
        Dump = true;

        var distribute = Path.Combine(path, "distribute");
        Directory.CreateDirectory(distribute);
        DistributeFolder = distribute;
    }
}
