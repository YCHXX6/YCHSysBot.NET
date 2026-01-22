using System.ComponentModel;

namespace SysBot.Pokemon;

/// <summary>
/// Console agnostic settings
/// </summary>
public abstract class BaseConfig
{
    protected const string FeatureToggle = "功能切换";
    protected const string Operation = "操作";
    private const string Debug = "调试";

    [Category(FeatureToggle), DisplayName("防止待机 (Anti-Idle)"), Description("启用时，当机器人空闲时会偶尔按 B 按钮以防止进入睡眠。")]
    public bool AntiIdle { get; set; }

    [Category(FeatureToggle), DisplayName("启用日志"), Description("启用文本日志。更改后需重启以生效。")]
    public bool LoggingEnabled { get; set; } = true;

    [Category(FeatureToggle), DisplayName("保留的日志归档数量"), Description("保留的旧文本日志文件的最大数量。设置为 <= 0 可禁用日志清理。更改后需重启以生效。")]
    public int MaxArchiveFiles { get; set; } = 14;

    [Category(Debug), DisplayName("跳过控制台机器人创建"), Description("启动程序时跳过创建机器人；便于测试集成。")]
    public bool SkipConsoleBotCreation { get; set; }

    [Category(Operation)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public LegalitySettings Legality { get; set; } = new();

    [Category(Operation)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public FolderSettings Folder { get; set; } = new();

    public abstract bool Shuffled { get; }
}
