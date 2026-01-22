using PKHeX.Core;
using SysBot.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysBot.Pokemon.WinForms;

public sealed partial class Main : Form
{
    private readonly List<PokeBotState> Bots = [];
    private readonly IPokeBotRunner RunningEnvironment;
    private readonly ProgramConfig Config = Program.Config;

    public Main()
    {
        InitializeComponent();

        RunningEnvironment = GetRunner(Config);
        {
            foreach (var bot in Config.Bots)
            {
                bot.Initialize();
                AddBot(bot);
            }
        }

        RTB_Logs.MaxLength = 32_767; // character length
        LoadControls();
        Text = $"{Text} ({Config.Mode})";
        Task.Run(BotMonitor);

        InitUtil.InitializeStubs(Config.Mode);

        if (Config.DarkMode)
        {
            foreach (TabPage tab in TC_Main.TabPages)
                tab.UseVisualStyleBackColor = false;
        }

        if (Config is not { Width: 0, Height: 0 })
        {
            Width = Config.Width;
            Height = Config.Height;
        }

        B_New.Height = CB_Protocol.Height;
        FLP_BotCreator.Height = B_New.Height + B_New.Margin.Vertical;
    }

    protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
    {
        base.ScaleControl(factor, specified);
        TC_Main.ItemSize = new((int)(TC_Main.ItemSize.Width * factor.Width), (int)(TC_Main.ItemSize.Height * factor.Height));
    }

    private static IPokeBotRunner GetRunner(ProgramConfig cfg) => cfg.Mode switch
    {
        ProgramMode.SWSH => new PokeBotRunnerImpl<PK8>(cfg.Hub, new BotFactory8SWSH()),
        ProgramMode.BDSP => new PokeBotRunnerImpl<PB8>(cfg.Hub, new BotFactory8BS()),
        ProgramMode.LA   => new PokeBotRunnerImpl<PA8>(cfg.Hub, new BotFactory8LA()),
        ProgramMode.SV   => new PokeBotRunnerImpl<PK9>(cfg.Hub, new BotFactory9SV()),
        ProgramMode.LZA  => new PokeBotRunnerImpl<PA9>(cfg.Hub, new BotFactory9LZA()),
        _ => throw new IndexOutOfRangeException("Unsupported mode."),
    };

    private async Task BotMonitor()
    {
        while (!Disposing)
        {
            try
            {
                foreach (var c in FLP_Bots.Controls.OfType<BotController>())
                    c.ReadState();
            }
            catch
            {
                // Updating the collection by adding/removing bots will change the iterator
                // Can try a for-loop or ToArray, but those still don't prevent concurrent mutations of the array.
                // Just try, and if failed, ignore. Next loop will be fine. Locks on the collection are kinda overkill, since this task is not critical.
            }
            await Task.Delay(2_000).ConfigureAwait(false);
        }
    }

    private void LoadControls()
    {
        PG_Hub.SelectedObject = RunningEnvironment.Config;
        var routines = Enum.GetValues<PokeRoutineType>().Where(z => RunningEnvironment.SupportsRoutine(z));
        static string GetRoutineDisplayName(PokeRoutineType z) => z switch
        {
            PokeRoutineType.Idle => "空闲",
            PokeRoutineType.SurpriseTrade => "惊喜交易",
            PokeRoutineType.FlexTrade => "灵活交易",
            PokeRoutineType.LinkTrade => "连线交易",
            PokeRoutineType.SeedCheck => "种子检查",
            PokeRoutineType.Clone => "克隆",
            PokeRoutineType.Dump => "导出",
            PokeRoutineType.RaidBot => "突袭机器人",
            PokeRoutineType.EncounterLine => "遭遇线路",
            PokeRoutineType.Reset => "重置遭遇",
            PokeRoutineType.DogBot => "箱体传说遭遇",
            PokeRoutineType.EggFetch => "拾蛋",
            PokeRoutineType.FossilBot => "化石机器人",
            PokeRoutineType.RemoteControl => "远程控制",
            _ => z.ToString(),
        };

        var list = routines.Select(z => new ComboItem(GetRoutineDisplayName(z), (int)z)).ToArray();
        CB_Routine.DisplayMember = nameof(ComboItem.Text);
        CB_Routine.ValueMember = nameof(ComboItem.Value);
        CB_Routine.DataSource = list;
        CB_Routine.SelectedValue = (int)PokeRoutineType.FlexTrade; // default option

        var protocols = Enum.GetValues<SwitchProtocol>();
        static string GetProtocolDisplayName(SwitchProtocol p) => p switch
        {
            SwitchProtocol.WiFi => "WiFi (无线)",
            SwitchProtocol.USB => "USB (有线)",
            _ => p.ToString(),
        };
        var listP = protocols.Select(z => new ComboItem(GetProtocolDisplayName(z), (int)z)).ToArray();
        CB_Protocol.DisplayMember = nameof(ComboItem.Text);
        CB_Protocol.ValueMember = nameof(ComboItem.Value);
        CB_Protocol.DataSource = listP;
        CB_Protocol.SelectedIndex = (int)SwitchProtocol.WiFi; // default option

        LogUtil.Forwarders.Add(new TextBoxForwarder(RTB_Logs));
    }

    private ProgramConfig GetCurrentConfiguration()
    {
        Config.Bots = [.. Bots];
        return Config;
    }

    private void Main_FormClosing(object sender, FormClosingEventArgs e)
    {
        SaveCurrentConfig();
        var bots = RunningEnvironment;
        if (!bots.IsRunning)
            return;

        async Task WaitUntilNotRunning()
        {
            while (bots.IsRunning)
                await Task.Delay(10).ConfigureAwait(false);
        }

        // Try to let all bots hard-stop before ending execution of the entire program.
        WindowState = FormWindowState.Minimized;
        ShowInTaskbar = false;
        bots.StopAll();
        Task.WhenAny(WaitUntilNotRunning(), Task.Delay(5_000)).ConfigureAwait(true).GetAwaiter().GetResult();
    }

    private void SaveCurrentConfig()
    {
        var cfg = GetCurrentConfiguration();
        cfg.Width = Width;
        cfg.Height = Height;
        ConfigLoader.Save(cfg);
    }

    private void B_Start_Click(object sender, EventArgs e)
    {
        SaveCurrentConfig();

        LogUtil.LogInfo("正在启动所有机器人...", "Form");
        RunningEnvironment.InitializeStart();
        SendAll(BotControlCommand.Start);
        Tab_Logs.Select();

        if (Bots.Count == 0)
            WinFormsUtil.Alert("未配置任何机器人，但所有支持服务已启动。");
    }

    private void SendAll(BotControlCommand cmd)
    {
        foreach (var c in FLP_Bots.Controls.OfType<BotController>())
            c.SendCommand(cmd, false);

        EchoUtil.Echo($"已向所有机器人发送 {cmd} 命令。");
    }

    private void B_Stop_Click(object sender, EventArgs e)
    {
        var env = RunningEnvironment;
        if (!env.IsRunning && (ModifierKeys & Keys.Alt) == 0)
        {
            WinFormsUtil.Alert("当前没有运行的任务。");
            return;
        }

        var cmd = BotControlCommand.Stop;

        if ((ModifierKeys & Keys.Control) != 0 || (ModifierKeys & Keys.Shift) != 0) // either, because remembering which can be hard
        {
                if (env.IsRunning)
            {
                WinFormsUtil.Alert("正在将所有机器人置为空闲。", "按 Stop（不按修饰键）可强制停止并解锁控制；再次按带修饰键的 Stop 可恢复。");
                cmd = BotControlCommand.Idle;
            }
            else
            {
                WinFormsUtil.Alert("正在命令所有机器人恢复其原始任务。", "按 Stop（不按修饰键）可强制停止并解锁控制。");
                cmd = BotControlCommand.Resume;
            }
        }
        SendAll(cmd);
    }

    private void B_New_Click(object sender, EventArgs e)
    {
        var cfg = CreateNewBotConfig();
        if (!AddBot(cfg))
        {
            WinFormsUtil.Alert("无法添加机器人；请确保信息有效且不与已存在的机器人重复。");
            return;
        }
        System.Media.SystemSounds.Asterisk.Play();
    }

    private bool AddBot(PokeBotState cfg)
    {
        if (!cfg.IsValid())
            return false;

        if (Bots.Any(z => z.Connection.Equals(cfg.Connection)))
            return false;

        PokeRoutineExecutorBase newBot;
        try
        {
            Console.WriteLine($"Current Mode ({Config.Mode}) does not support this type of bot ({cfg.CurrentRoutineType}).");
            newBot = RunningEnvironment.CreateBotFromConfig(cfg);
        }
        catch
        {
            return false;
        }

        try
        {
            RunningEnvironment.Add(newBot);
        }
        catch (ArgumentException ex)
        {
            WinFormsUtil.Error(ex.Message);
            return false;
        }

        AddBotControl(cfg);
        Bots.Add(cfg);
        return true;
    }

    private void AddBotControl(PokeBotState cfg)
    {
        var row = new BotController { Width = FLP_Bots.Width, Anchor = AnchorStyles.Left | AnchorStyles.Right };
        row.Initialize(RunningEnvironment, cfg);
        FLP_Bots.Controls.Add(row);
        FLP_Bots.SetFlowBreak(row, true);
        row.AddClickHandler(() =>
        {
            var details = cfg.Connection;
            TB_IP.Text = details.IP;
            NUD_Port.Text = details.Port.ToString();
            CB_Protocol.SelectedIndex = (int)details.Protocol;
            CB_Routine.SelectedValue = (int)cfg.InitialRoutine;
        });

        row.Remove += (s, e) =>
        {
            Bots.Remove(row.State);
            RunningEnvironment.Remove(row.State, !RunningEnvironment.Config.SkipConsoleBotCreation);
            FLP_Bots.Controls.Remove(row);
        };
    }

    private PokeBotState CreateNewBotConfig()
    {
        var ip = TB_IP.Text;
        var port = int.TryParse(NUD_Port.Text, out var p) ? p : 6000;
        var cfg = BotConfigUtil.GetConfig<SwitchConnectionConfig>(ip, port);
        cfg.Protocol = (SwitchProtocol)WinFormsUtil.GetIndex(CB_Protocol);

        var pk = new PokeBotState { Connection = cfg };
        var type = (PokeRoutineType)WinFormsUtil.GetIndex(CB_Routine);
        pk.Initialize(type);
        return pk;
    }

    private void FLP_Bots_Resize(object sender, EventArgs e)
    {
        foreach (var c in FLP_Bots.Controls.OfType<BotController>())
            c.Width = FLP_Bots.Width;
    }

    private void CB_Protocol_SelectedIndexChanged(object sender, EventArgs e)
    {
        var isWifi = CB_Protocol.SelectedIndex == 0;
        TB_IP.Visible = isWifi;
        NUD_Port.ReadOnly = isWifi;

        if (isWifi)
            NUD_Port.Text = "6000";
    }
}
