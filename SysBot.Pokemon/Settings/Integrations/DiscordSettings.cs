using System.ComponentModel;

namespace SysBot.Pokemon;

public class DiscordSettings
{
    private const string Startup = "启动";
    private const string Operation = "操作";
    private const string Channels = "频道";
    private const string Roles = "角色";
    private const string Users = "用户";
    public override string ToString() => "Discord 集成 设置";

    // Startup

    [Category(Startup), DisplayName("Bot 登录令牌"), Description("机器人登录令牌。")]
    public string Token { get; set; } = string.Empty;

    [Category(Startup), DisplayName("指令前缀"), Description("机器人命令前缀。")]
    public string CommandPrefix { get; set; } = "$";

    [Category(Startup), DisplayName("模块黑名单"), Description("启动机器人时不会加载的模块列表（以逗号分隔）。")]
    public string ModuleBlacklist { get; set; } = string.Empty;

    [Category(Startup), DisplayName("异步处理指令"), Description("切换以异步或同步方式处理指令。")]
    public bool AsyncCommands { get; set; }

    [Category(Startup), DisplayName("Bot 游戏状态 (自定义)"), Description("用于显示的自定义正在玩的游戏状态。")]
    public string BotGameStatus { get; set; } = "SysBot.NET: Pokémon";

    [Category(Startup), DisplayName("仅对交易型机器人显示颜色状态"), Description("仅考虑交易类型的机器人来显示 Discord 在线状态颜色。")]
    public bool BotColorStatusTradeOnly { get; set; } = true;

    [Category(Operation), DisplayName("问候回复"), Description("当用户向机器人打招呼时，机器人会回复的自定义消息。使用字符串格式来在回复中提及用户。")]
    public string HelloResponse { get; set; } = "Hi {0}!";

    // Whitelists

    [Category(Roles), DisplayName("允许进入交易队列的角色"), Description("具有此角色的用户被允许进入交易队列。")]
    public RemoteControlAccessList RoleCanTrade { get; set; } = new() { AllowIfEmpty = false };

    [Category(Roles), DisplayName("允许进入种子检查队列的角色"), Description("具有此角色的用户被允许进入种子检查队列。")]
    public RemoteControlAccessList RoleCanSeedCheck { get; set; } = new() { AllowIfEmpty = false };

    [Category(Roles), DisplayName("允许进入克隆队列的角色"), Description("具有此角色的用户被允许进入克隆队列。")]
    public RemoteControlAccessList RoleCanClone { get; set; } = new() { AllowIfEmpty = false };

    [Category(Roles), DisplayName("允许进入导出队列的角色"), Description("具有此角色的用户被允许进入导出队列。")]
    public RemoteControlAccessList RoleCanDump { get; set; } = new() { AllowIfEmpty = false };

    [Category(Roles), Description("Users with this role are allowed to remotely control the console (if running as Remote Control Bot.")]
    public RemoteControlAccessList RoleRemoteControl { get; set; } = new() { AllowIfEmpty = false };

    [Category(Roles), DisplayName("允许绕过指令限制的角色"), Description("具有此角色的用户被允许绕过指令限制。")]
    public RemoteControlAccessList RoleSudo { get; set; } = new() { AllowIfEmpty = false };

    // Operation

    [Category(Roles), DisplayName("优先入队的角色"), Description("具有此角色的用户被允许以更好的位置加入队列。")]
    public RemoteControlAccessList RoleFavored { get; set; } = new() { AllowIfEmpty = false };

    [Category(Users), DisplayName("用户黑名单"), Description("具有这些用户 ID 的用户不能使用机器人。")]
    public RemoteControlAccessList UserBlacklist { get; set; } = new();

    [Category(Channels), DisplayName("频道白名单"), Description("具有这些 ID 的频道是机器人确认命令的唯一频道。")]
    public RemoteControlAccessList ChannelWhitelist { get; set; } = new();

    [Category(Users), DisplayName("全局 sudo 列表"), Description("以逗号分隔的 Discord 用户 ID 列表，这些用户将具有对 Bot Hub 的 sudo 访问权限。")]
    public RemoteControlAccessList GlobalSudoList { get; set; } = new();

    [Category(Users), DisplayName("允许全局 sudo"), Description("禁用此项将移除全局 sudo 支持。")]
    public bool AllowGlobalSudo { get; set; } = true;

    [Category(Channels), DisplayName("日志回显频道"), Description("将回显日志机器人数据的频道 ID 列表。")]
    public RemoteControlAccessList LoggingChannels { get; set; } = new();

    [Category(Channels), DisplayName("交易开始日志频道"), Description("记录交易开始消息的日志频道。")]
    public RemoteControlAccessList TradeStartingChannels { get; set; } = new();

    [Category(Channels), DisplayName("回显频道"), Description("回显特殊消息的频道。")]
    public RemoteControlAccessList EchoChannels { get; set; } = new();

    [Category(Operation), DisplayName("返回交易中显示的 PKM"), Description("将交易中显示的宝可梦的 PKM 返回给用户。")]
    public bool ReturnPKMs { get; set; } = true;

    [Category(Operation), DisplayName("回复不允许的命令"), Description("当用户在该频道不允许使用某个命令时是否回复。为 false 时，机器人将静默忽略。")]
    public bool ReplyCannotUseCommandInChannel { get; set; } = true;

    [Category(Operation), DisplayName("自动转换 PKM 为 ShowdownSet"), Description("机器人监听频道消息，当检测到附加 PKM 文件时（非命令），会回复对应的 ShowdownSet。")]
    public bool ConvertPKMToShowdownSet { get; set; } = true;

    [Category(Operation), DisplayName("在任意频道回复 PKM 转换"), Description("允许机器人在其能看到的任意频道回复 ShowdownSet，而不限于白名单频道。仅在需要机器人在非机器人频道提供更多工具时启用。")]
    public bool ConvertPKMReplyAnyChannel { get; set; }
}
