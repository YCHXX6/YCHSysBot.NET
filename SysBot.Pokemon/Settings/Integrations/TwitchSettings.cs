using System;
using System.ComponentModel;
using System.Linq;

namespace SysBot.Pokemon;

public class TwitchSettings
{
    private const string Startup = "启动";
    private const string Operation = "操作";
    private const string Messages = "消息";
    public override string ToString() => "Twitch 集成 设置";

    // Startup

    [Category(Startup), DisplayName("Bot 登录令牌"), Description("Bot 登录令牌。")]
    public string Token { get; set; } = string.Empty;

    [Category(Startup), DisplayName("Bot 用户名"), Description("Bot 用户名。")]
    public string Username { get; set; } = string.Empty;

    [Category(Startup), DisplayName("发送消息的频道"), Description("发送消息的频道。")]
    public string Channel { get; set; } = string.Empty;

    [Category(Startup), DisplayName("Bot 指令前缀"), Description("Bot 命令前缀。")]
    public char CommandPrefix { get; set; } = '$';

    [Category(Operation), DisplayName("屏障释放消息"), Description("当屏障被释放时发送的消息。")]
    public string MessageStart { get; set; } = string.Empty;

    // Messaging

    [Category(Operation), Description("Throttle the bot from sending messages if X messages have been sent in the past Y seconds.")]
    public int ThrottleMessages { get; set; } = 100;

    [Category(Operation), Description("Throttle the bot from sending messages if X messages have been sent in the past Y seconds.")]
    public double ThrottleSeconds { get; set; } = 30;

    [Category(Operation), Description("Throttle the bot from sending whispers if X messages have been sent in the past Y seconds.")]
    public int ThrottleWhispers { get; set; } = 100;

    [Category(Operation), Description("Throttle the bot from sending whispers if X messages have been sent in the past Y seconds.")]
    public double ThrottleWhispersSeconds { get; set; } = 60;

    // Operation

    [Category(Operation), DisplayName("Sudo 用户名列表"), Description("拥有 sudo 权限的用户名列表。")]
    public string SudoList { get; set; } = string.Empty;

    [Category(Operation), DisplayName("用户黑名单"), Description("具有这些用户名的用户不能使用机器人。")]
    public string UserBlacklist { get; set; } = string.Empty;

    [Category(Operation), DisplayName("允许通过频道发送命令"), Description("启用时，机器人将处理发送到频道的命令。")]
    public bool AllowCommandsViaChannel { get; set; } = true;

    [Category(Operation), DisplayName("允许通过私信发送命令"), Description("启用时，机器人允许用户通过私信发送命令（绕过慢速模式）。")]
    public bool AllowCommandsViaWhisper { get; set; }

    // Message Destinations

    [Category(Messages), DisplayName("普通通知发送位置"), Description("确定普通通知发送到何处。")]
    public TwitchMessageDestination NotifyDestination { get; set; }

    [Category(Messages), DisplayName("交易开始通知发送位置"), Description("确定交易开始通知发送到何处。")]
    public TwitchMessageDestination TradeStartDestination { get; set; } = TwitchMessageDestination.Channel;

    [Category(Messages), DisplayName("交易搜索通知发送位置"), Description("确定交易搜索通知发送到何处。")]
    public TwitchMessageDestination TradeSearchDestination { get; set; }

    [Category(Messages), DisplayName("交易完成通知发送位置"), Description("确定交易完成通知发送到何处。")]
    public TwitchMessageDestination TradeFinishDestination { get; set; }

    [Category(Messages), DisplayName("交易取消通知发送位置"), Description("确定交易取消通知发送到何处。")]
    public TwitchMessageDestination TradeCanceledDestination { get; set; } = TwitchMessageDestination.Channel;

    [Category(Messages), DisplayName("分发交易倒计时"), Description("切换分发交易在开始前是否倒计时。")]
    public bool DistributionCountDown { get; set; } = true;

    public bool IsSudo(string username)
    {
        var sudos = SudoList.Split([ ",", ", ", " " ], StringSplitOptions.RemoveEmptyEntries);
        return sudos.Contains(username);
    }
}

public enum TwitchMessageDestination
{
    Disabled,
    Channel,
    Whisper,
}
