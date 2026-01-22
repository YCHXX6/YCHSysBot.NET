namespace SysBot.Pokemon;

using System.ComponentModel;

public enum EncounterMode
{
    [Description("垂直线路 - 在垂直直线路往返移动以遭遇宝可梦")]
    VerticalLine,

    [Description("水平线路 - 在水平直线路往返移动以遭遇宝可梦")]
    HorizontalLine,

    [Description("软重置 Eternatus")]
    Eternatus,

    [Description("领取礼物并检查盒子 1 插槽 1")]
    Gift,

    [Description("检查野外遭遇然后重置游戏")]
    Reset,

    [Description("重置 Regigigas")]
    Regigigas,

    [Description("重置 Motostoke 竞技场的遭遇")]
    MotostokeGym,
}
