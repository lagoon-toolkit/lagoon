namespace Lagoon.UI.Demo.Pages;

public class PrefixSuffix
{

    public bool PrefixEnabled { get; set; } = true;

    public bool SuffixEnabled { get; set; } = true;

    public string Prefix => !PrefixEnabled ? null : PrefixType == InputLabelType.IconName ? IconPrefix : TextPrefix;

    public InputLabelType PrefixType { get; set; } = InputLabelType.IconName;

    public string IconPrefix { get; set; } = IconNames.Info;

    public string TextPrefix { get; set; } = "$";

    public string Suffix => !SuffixEnabled ? null : SuffixType == InputLabelType.IconName ? IconSuffix : TextSuffix;

    public InputLabelType SuffixType { get; set; } = InputLabelType.Text;

    public string IconSuffix { get; set; } = IconNames.All.Search;

    public string TextSuffix { get; set; } = " €";

}
