namespace Lagoon.UI.Components.Input.Internal;

/// <summary>
/// input common properties.
/// </summary>
public interface IInputCommonProperties
{

    /// <summary>
    /// Title zone
    /// </summary>
    string Label { get; set; }

    /// <summary>
    /// Optionnal - To allow full title customisation with a RenderFragment
    /// </summary>
    RenderFragment LabelContent { get; set; }


}
