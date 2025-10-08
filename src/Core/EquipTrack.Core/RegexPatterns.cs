namespace EquipTrack.Core;

using System.Text.RegularExpressions;

public static partial class RegexPatterns
{
    public static readonly Regex EmailIsValid = EmailRegexPatternAttr();
    public static readonly Regex PhoneNumberIsValid = PhoneNumberRegexPatternAttr();
    public static readonly Regex AssetCodeIsValid = AssetCodeRegexPatternAttr();
    public static readonly Regex WorkOrderIdIsValid = WorkOrderIdRegexPatternAttr();
    public static readonly Regex DateIsValid = DateRegexPatternAttr();
    public static readonly Regex EmployeeIdIsValid = EmployeeIdRegexPatternAttr();
    public static readonly Regex PartNumberIsValid = PartNumberRegexPatternAttr();

    [GeneratedRegex(@"^([0-9a-zA-Z]([+\-_.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,17})$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex EmailRegexPatternAttr();

    [GeneratedRegex(@"^\+?\d{10,15}$",
        RegexOptions.Compiled)]
    private static partial Regex PhoneNumberRegexPatternAttr();

    [GeneratedRegex(@"^ASSET-\d{4,10}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex AssetCodeRegexPatternAttr();

    [GeneratedRegex(@"^WO-\d{4}-\d{3,5}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex WorkOrderIdRegexPatternAttr();

    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2}$",
        RegexOptions.Compiled)]
    private static partial Regex DateRegexPatternAttr();

    [GeneratedRegex(@"^EMP-\d{3,6}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex EmployeeIdRegexPatternAttr();

    [GeneratedRegex(@"^PART-[A-Z]{2,5}-\d{3,6}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex PartNumberRegexPatternAttr();
}
