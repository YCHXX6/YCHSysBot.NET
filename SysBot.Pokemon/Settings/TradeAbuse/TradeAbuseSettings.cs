using System.ComponentModel;

namespace SysBot.Pokemon;

public class TradeAbuseSettings
{
    private const string Monitoring = "监控";
    public override string ToString() => "交易滥用 监控 设置";

    [Category(Monitoring), DisplayName("交易冷却 (分钟)"), Description("当某人在小于此设置值（分钟）内再次出现时，将发送通知。")]
    public double TradeCooldown { get; set; }

    [Category(Monitoring), DisplayName("回显 Nintendo ID (冷却)"), Description("当某人忽略交易冷却时，回显消息将包含其 Nintendo 账户 ID。")]
    public bool EchoNintendoOnlineIDCooldown { get; set; } = true;

    [Category(Monitoring), DisplayName("冷却滥用回显提及"), Description("若不为空，当用户违反交易冷却时，指定的字符串将追加到回显通知中以提醒指定对象。在 Discord 中使用 <@userIDnumber> 提及用户。")]
    public string CooldownAbuseEchoMention { get; set; } = string.Empty;

    [Category(Monitoring), DisplayName("交易滥用过期 (分钟)"), Description("当某人在小于此设置值（分钟）内以不同的 Discord/Twitch 账户出现时，将发送通知。")]
    public double TradeAbuseExpiration { get; set; } = 120;

    [Category(Monitoring), DisplayName("回显 Nintendo ID (多账户)"), Description("当检测到某人使用多个 Discord/Twitch 账户时，回显消息将包含其 Nintendo 账户 ID。")]
    public bool EchoNintendoOnlineIDMulti { get; set; } = true;

    [Category(Monitoring), DisplayName("回显 Nintendo ID (多接收者)"), Description("当检测到某人发送给多个游戏内账户时，回显消息将包含其 Nintendo 账户 ID。")]
    public bool EchoNintendoOnlineIDMultiRecipients { get; set; } = true;

    [Category(Monitoring), DisplayName("检测到多账户时的动作"), Description("当检测到使用多个 Discord/Twitch 账户的人时，将采取此操作。")]
    public TradeAbuseAction TradeAbuseAction { get; set; } = TradeAbuseAction.Quit;

    [Category(Monitoring), DisplayName("被封禁 ID 时阻止并加入黑名单"), Description("当某人在游戏中因多个账户被封禁时，其在线 ID 将被加入 BannedIDs。")]
    public bool BanIDWhenBlockingUser { get; set; } = true;

    [Category(Monitoring), DisplayName("多账户滥用回显提及"), Description("若不为空，当检测到用户使用多个账户时，指定的字符串将追加到回显通知中。在 Discord 中使用 <@userIDnumber> 提及用户。")]
    public string MultiAbuseEchoMention { get; set; } = string.Empty;

    [Category(Monitoring), DisplayName("多接收者回显提及"), Description("若不为空，当检测到用户向多个游戏内玩家发送时，指定的字符串将追加到回显通知中。在 Discord 中使用 <@userIDnumber> 提及用户。")]
    public string MultiRecipientEchoMention { get; set; } = string.Empty;

    [Category(Monitoring), DisplayName("被封禁的在线 ID 列表"), Description("被封禁的在线 ID 列表，这些 ID 会触发交易退出或游戏内封禁。")]
    public RemoteControlAccessList BannedIDs { get; set; } = new();

    [Category(Monitoring), DisplayName("检测到被封禁的用户时阻止"), Description("当检测到具有被封禁 ID 的人时，在退出交易前先在游戏内封禁他们。")]
    public bool BlockDetectedBannedUser { get; set; } = true;

    [Category(Monitoring), DisplayName("被封禁 ID 匹配回显提及"), Description("若不为空，当用户匹配被封禁 ID 时，指定的字符串将追加到回显通知中。在 Discord 中使用 <@userIDnumber> 提及用户。")]
    public string BannedIDMatchEchoMention { get; set; } = string.Empty;

    [Category(Monitoring), DisplayName("回显 Nintendo ID (Ledy)"), Description("当检测到使用 Ledy 昵称交换滥用时，回显消息将包含其 Nintendo 账户 ID。")]
    public bool EchoNintendoOnlineIDLedy { get; set; } = true;

    [Category(Monitoring), DisplayName("Ledy 滥用回显提及"), Description("若不为空，当用户违反 Ledy 交易规则时，指定的字符串将追加到回显通知中。在 Discord 中使用 <@userIDnumber> 提及用户。")]
    public string LedyAbuseEchoMention { get; set; } = string.Empty;
}
