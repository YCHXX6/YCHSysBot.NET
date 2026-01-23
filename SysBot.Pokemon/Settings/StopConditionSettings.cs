using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SysBot.Pokemon;

public class StopConditionSettings
{
    private const string StopConditions = "停止条件";
    public override string ToString() => "停止条件 设置";

    [Category(StopConditions), DisplayName("停止物种"), Description("仅在指定物种时停止。若设为 \"None\" 则不限制。")]
    public Species StopOnSpecies { get; set; }

    [Category(StopConditions), DisplayName("指定形态 ID"), Description("仅在具有此形态 ID 时停止。留空则不限制。")]
    public int? StopOnForm { get; set; }

    [Category(StopConditions), DisplayName("目标性格"), Description("仅在具有指定性格时停止。")]
    public Nature TargetNature { get; set; } = Nature.Random;

    [Category(StopConditions), Description("Minimum accepted IVs in the format HP/Atk/Def/SpA/SpD/Spe. Use \"x\" for unchecked IVs, \"s\" to match 0 or 31, and \"/\" as a separator.")]
    public string TargetMinIVs { get; set; } = "";

    [Category(StopConditions), Description("Maximum accepted IVs in the format HP/Atk/Def/SpA/SpD/Spe. Use \"x\" for unchecked IVs, \"s\" to match 0 or 31 and \"/\" as a separator.")]
    public string TargetMaxIVs { get; set; } = "";

    [Category(StopConditions), DisplayName("光闪目标"), Description("选择要停止的光闪类型。")]
    public TargetShinyType ShinyTarget { get; set; } = TargetShinyType.DisableOption;

    [Category(StopConditions), DisplayName("身高目标"), Description("允许根据最小或最大身高进行筛选。")]
    public TargetHeightType HeightTarget { get; set; } = TargetHeightType.DisableOption;

    [Category(StopConditions), DisplayName("仅有标记"), Description("仅在宝可梦具有标记时停止。")]
    public bool MarkOnly { get; set; }

    [Category(StopConditions), DisplayName("忽略的标记列表"), Description("用逗号分隔要忽略的标记列表。使用完整名称，例如：\"Uncommon Mark, Dawn Mark, Prideful Mark\"。")]
    public string UnwantedMarks { get; set; } = "";

    [Category(StopConditions), DisplayName("录制视频片段"), Description("当 EncounterBot 或 Fossilbot 找到匹配宝可梦时，按住 Capture 按钮以录制 30 秒片段。")]
    public bool CaptureVideoClip { get; set; }

    [Category(StopConditions), DisplayName("录制前额外等待 (毫秒)"), Description("在匹配到遭遇后，按下 Capture 之前额外等待的时间（毫秒）。用于 EncounterBot 或 Fossilbot。")]
    public int ExtraTimeWaitCaptureVideo { get; set; } = 10000;

    [Category(StopConditions), DisplayName("同时匹配光闪与 IV"), Description("若为 TRUE，则同时匹配 ShinyTarget 与 TargetIVs 设置。否则，只需满足任一条件即可匹配。")]
    public bool MatchShinyAndIV { get; set; } = true;

    [Category(StopConditions), DisplayName("匹配到时回显提及前缀"), Description("若不为空，指定的字符串会被加在匹配结果日志消息前以回显通知给指定对象。在 Discord 中使用 <@userIDnumber> 提及用户。")]
    public string MatchFoundEchoMention { get; set; } = string.Empty;

    public static bool EncounterFound<T>(T pk, int[] targetminIVs, int[] targetmaxIVs, StopConditionSettings settings, IReadOnlyList<string>? marklist) where T : PKM
    {
        // Match Nature and Species if they were specified.
        if (settings.StopOnSpecies != Species.None && settings.StopOnSpecies != (Species)pk.Species)
            return false;

        if (settings.StopOnForm.HasValue && settings.StopOnForm != pk.Form)
            return false;

        if (settings.TargetNature != Nature.Random && settings.TargetNature != pk.Nature)
            return false;

        // Return if it doesn't have a mark, or it has an unwanted mark.
        var unmarked = pk is IRibbonIndex m && !HasMark(m);
        var unwanted = marklist is not null && pk is IRibbonIndex m2 && settings.IsUnwantedMark(GetMarkName(m2), marklist);
        if (settings.MarkOnly && (unmarked || unwanted))
            return false;

        if (settings.ShinyTarget != TargetShinyType.DisableOption)
        {
            bool shinymatch = settings.ShinyTarget switch
            {
                TargetShinyType.AnyShiny => pk.IsShiny,
                TargetShinyType.NonShiny => !pk.IsShiny,
                TargetShinyType.StarOnly => pk.IsShiny && pk.ShinyXor != 0,
                TargetShinyType.SquareOnly => pk.ShinyXor == 0,
                TargetShinyType.DisableOption => true,
                _ => throw new ArgumentException(nameof(TargetShinyType)),
            };

            // If we only needed to match one of the criteria and it shiny match'd, return true.
            // If we needed to match both criteria, and it didn't shiny match, return false.
            if (!settings.MatchShinyAndIV && shinymatch)
                return true;
            if (settings.MatchShinyAndIV && !shinymatch)
                return false;
        }

        if (settings.HeightTarget != TargetHeightType.DisableOption && pk is PK8 p)
        {
            var value = p.HeightScalar;
            bool heightmatch = settings.HeightTarget switch
            {
                TargetHeightType.MinOnly => value is 0,
                TargetHeightType.MaxOnly => value is 255,
                TargetHeightType.MinOrMax => value is 0 or 255,
                _ => throw new ArgumentException(nameof(TargetHeightType)),
            };

            if (!heightmatch)
                return false;
        }

        // Reorder the speed to be last.
        Span<int> pkIVList = stackalloc int[6];
        pk.GetIVs(pkIVList);
        (pkIVList[5], pkIVList[3], pkIVList[4]) = (pkIVList[3], pkIVList[4], pkIVList[5]);
        return MatchesTargetIVs(pkIVList, targetminIVs, targetmaxIVs);
    }

    public static void InitializeTargetIVs(PokeTradeHubConfig config, out int[] min, out int[] max)
    {
        min = ReadTargetIVs(config.StopConditions, true);
        max = ReadTargetIVs(config.StopConditions, false);
    }

    private static int[] ReadTargetIVs(StopConditionSettings settings, bool min)
    {
        int[] targetIVs = new int[6];
        char[] split = ['/'];

        string[] splitIVs = min
            ? settings.TargetMinIVs.Split(split, StringSplitOptions.RemoveEmptyEntries)
            : settings.TargetMaxIVs.Split(split, StringSplitOptions.RemoveEmptyEntries);

        // Only accept up to 6 values.  Fill it in with default values if they don't provide 6.
        // Anything that isn't an integer will be a wild card.
        for (int i = 0; i < 6; i++)
        {
            if (i < splitIVs.Length)
            {
                var str = splitIVs[i];
                // Special case where we are matching either 0 or 31.
                if (str.Equals("s", StringComparison.CurrentCultureIgnoreCase))
                {
                    targetIVs[i] = 99;
                    continue;
                }
                // Any other numerical IV value.
                else if (int.TryParse(str, out var val))
                {
                    targetIVs[i] = val;
                    continue;
                }
            }
            // If we get to here, set it to the min or max wild card value.
            targetIVs[i] = min ? 0 : 31;
        }
        return targetIVs;
    }

    private static bool MatchesTargetIVs(ReadOnlySpan<int> ivs, ReadOnlySpan<int> min, ReadOnlySpan<int> max)
    {
        for (int i = 0; i < 6; i++)
        {
            if (!MatchesTargetIVs(ivs[i], min[i], max[i]))
                return false;
        }
        return true;
    }

    private static bool MatchesTargetIVs(int value, int min, int max)
    {
        if (min is 99 || max is 99)
            return value is 0 or 31;
        return min <= value && value <= max;
    }

    private static bool HasMark(IRibbonIndex pk)
    {
        for (var mark = RibbonIndex.MarkLunchtime; mark <= RibbonIndex.MarkSlump; mark++)
        {
            if (pk.GetRibbon((int)mark))
                return true;
        }
        return false;
    }

    public static ReadOnlySpan<BattleTemplateToken> TokenOrder =>
    [
        BattleTemplateToken.FirstLine,
        BattleTemplateToken.Shiny,
        BattleTemplateToken.Nature,
        BattleTemplateToken.IVs,
    ];

    public static string GetPrintName(PKM pk)
    {
        const LanguageID lang = LanguageID.English;
        var settings = new BattleTemplateExportSettings(TokenOrder, lang);
        var set = ShowdownParsing.GetShowdownText(pk, settings);

        // Since we can match on Min/Max Height for transfer to future games, display it.
        if (pk is IScaledSize p)
            set += $"\nHeight: {p.HeightScalar}";

        // Add the mark if it has one.
        if (pk is IRibbonIndex r)
        {
            var rstring = GetMarkName(r);
            if (!string.IsNullOrEmpty(rstring))
                set += $"\nPokémon has the **{GetMarkName(r)}**!";
        }
        return set;
    }

    public static void ReadUnwantedMarks(StopConditionSettings settings, out IReadOnlyList<string> marks) =>
        marks = settings.UnwantedMarks.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();

    public virtual bool IsUnwantedMark(string mark, IReadOnlyList<string> marklist) => marklist.Contains(mark);

    public static string GetMarkName(IRibbonIndex pk)
    {
        for (var mark = RibbonIndex.MarkLunchtime; mark <= RibbonIndex.MarkSlump; mark++)
        {
            if (pk.GetRibbon((int)mark))
                return GameInfo.Strings.Ribbons.GetName($"Ribbon{mark}");
        }
        return "";
    }
}

public enum TargetShinyType
{
    DisableOption,  // Doesn't care
    NonShiny,       // Match nonshiny only
    AnyShiny,       // Match any shiny regardless of type
    StarOnly,       // Match star shiny only
    SquareOnly,     // Match square shiny only
}

public enum TargetHeightType
{
    DisableOption,  // Doesn't care
    MinOnly,        // 0 Height only
    MaxOnly,        // 255 Height only
    MinOrMax,       // 0 or 255 Height
}
