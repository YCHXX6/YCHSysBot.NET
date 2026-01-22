using System.ComponentModel;

namespace SysBot.Pokemon;

public enum ContinueAfterMatch
{
    [Description("继续")]
    Continue,

    [Description("暂停并等待确认")]
    PauseWaitAcknowledge,

    [Description("停止并退出")]
    StopExit,
}
