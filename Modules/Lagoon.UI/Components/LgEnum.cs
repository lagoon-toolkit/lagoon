namespace Lagoon.UI.Components;

/// <summary>
/// Message level.
/// </summary>
public enum Level
{
    /// <summary>
    /// Error message.
    /// </summary>
    Error = 1,
    /// <summary>
    /// Information message.
    /// </summary>
    Info,
    /// <summary>
    /// Warning message.
    /// </summary>
    Warning,
    /// <summary>
    /// Success message.
    /// </summary>
    Success
}

/// <summary>
/// Position.
/// </summary>
public enum Position
{
    /// <summary>
    /// Top left
    /// </summary>
    TopLeft = 1,
    /// <summary>
    /// Top right
    /// </summary>
    TopRight,
    /// <summary>
    /// Bottom left
    /// </summary>
    BottomLeft,
    /// <summary>
    /// Bottom right
    /// </summary>
    BottomRight,
    /// <summary>
    /// Top center
    /// </summary>
    TopCenter,
    /// <summary>
    /// Bottom center
    /// </summary>
    BottomCenter,

}

/// <summary>
/// Kind of component button render (from boostrap) 
/// </summary>
public enum ButtonKind
{
    /// <summary>
    /// Primary kind.
    /// </summary>
    Primary = 1,
    /// <summary>
    /// Secondary kind.
    /// </summary>
    Secondary,
    /// <summary>
    /// Success kind.
    /// </summary>
    Success,
    /// <summary>
    /// Error kind.
    /// </summary>
    Error,
    /// <summary>
    /// Warning kind.
    /// </summary>
    Warning,
    /// <summary>
    /// Info kind.
    /// </summary>
    Info,
    /// <summary>
    /// Link kind.
    /// </summary>
    Link,
    /// <summary>
    /// Ghost kind.
    /// </summary>
    Ghost,
    /// <summary>
    /// Custom kind.
    /// </summary>
    Custom
}

/// <summary>
/// Kind of component toolbar button group render (from boostrap)
/// </summary>
public enum ToolbarGroupKind
{
    /// <summary>
    /// Primary kind.
    /// </summary>
    Primary = 1,
    /// <summary>
    /// Secondary kind.
    /// </summary>
    Secondary,
    /// <summary>
    /// Success kind.
    /// </summary>
    Success,
    /// <summary>
    /// Error kind.
    /// </summary>
    Error,
    /// <summary>
    /// Warning kind.
    /// </summary>
    Warning,
    /// <summary>
    /// Info kind.
    /// </summary>
    Info,
    /// <summary>
    /// Ghost kind.
    /// </summary>
    Ghost
}

/// <summary>
/// Kind of component render (from boostrap)
/// </summary>
public enum Kind
{
    /// <summary>
    /// Primary kind.
    /// </summary>
    Primary = 1,
    /// <summary>
    /// Default kind.
    /// </summary>
    Default,
    /// <summary>
    /// Secondary kind.
    /// </summary>
    Secondary,
    /// <summary>
    /// Success kind.
    /// </summary>
    Success,
    /// <summary>
    /// Error kind.
    /// </summary>
    Error,
    /// <summary>
    /// Warning kind.
    /// </summary>
    Warning,
    /// <summary>
    /// Information kind.
    /// </summary>
    Info
}

/// <summary>
/// Input mask lind.
/// </summary>
public enum InputMaskKind
{
    /// <summary>
    /// None.
    /// </summary>
    None,
    /// <summary>
    /// Mask using 9 : numeric, a : alphabetical, * : alphanumeric.
    /// </summary>
    Mask,
    /// <summary>
    /// Mask using regular expression.
    /// </summary>
    Regex,
    /// <summary>
    /// Email mask.
    /// </summary>
    Email,
}

/// <summary>
/// Represents the different display types of a textbox component.
/// </summary>
public enum TextBoxMode
{
    /// <summary>
    /// The textbox is a textarea
    /// </summary>
    Multiline = 0,
    /// <summary>
    /// Password.
    /// </summary>
    Password = 1
}

/// <summary>
/// Represents the different display types of a checkbox component.
/// </summary>
public enum CheckBoxKind
{
    /// <summary>
    /// The component will appear as a typical checkbox.
    /// </summary>
    Check,

    /// <summary>
    /// The component will appear as a toogle button.
    /// </summary>
    Toggle
}

/// <summary>
/// Represents the different position types for labels.
/// </summary>
public enum CheckBoxTextPosition
{
    /// <summary>
    /// The label will be displayed to the left.
    /// </summary>
    Left,

    /// <summary>
    /// The label will be displayed to the right.
    /// </summary>
    Right
}

/// <summary>
/// Represents the different position types for labels.
/// </summary>
public enum ProgressLabelPosition
{
    /// <summary>
    /// The label will be displayed to the left.
    /// </summary>
    Left,

    /// <summary>
    /// The label will be displayed on the center.
    /// </summary>
    Center,

    /// <summary>
    /// The label will be displayed to the right.
    /// </summary>
    Right
}

/// <summary>
/// Enum display orientation (vertical, horizontal)
/// </summary>
public enum DisplayOrientation
{
    /// <summary>
    /// Vertical orientation.
    /// </summary>
    Vertical,
    /// <summary>
    /// Horizontal orientation.
    /// </summary>
    Horizontal
}

/// <summary>
/// Enum display kind
/// </summary>
public enum RadioButtonDisplayKind
{
    /// <summary>
    /// A check box is shown beside the text.
    /// </summary>
    Classic,
    /// <summary>
    /// Text style change without check box.
    /// </summary>
    Modern
}

/// <summary>
/// EditForm toastr configuration for the ValidationSummary
/// </summary>
[Flags]
public enum EditFormErrorsDisplayOptions
{

    /// <summary>
    /// Don't display any message when validation occur on LgEditForm
    /// </summary>
    None = 0,

    /// <summary>
    /// Show only a generic message in toastr when validation errors occur on LgEditForm
    /// </summary>
    /// <remark>
    /// This is a default option
    /// </remark>
    ToastrGenericMessage = 1,

    /// <summary>
    /// Show all validation message in toastr
    /// </summary>
    ToastrAllMessages = 2,

    /// <summary>
    /// Show validation message in a toastr after input change
    /// </summary>
    ToastrOnBlur = 4,

    /// <summary>
    /// Show all validation message in a summary at the beginning of the form
    /// </summary>
    Summary = 8,

    /// <summary>
    /// Show erros message under input
    /// </summary>
    /// <remark>
    /// This is a default option
    /// </remark>
    InlineValidation = 16
}

/// <summary>
/// Enum display title level
/// </summary>
public enum TitleLevel
{
    /// <summary>
    /// h1
    /// </summary>
    Level1,
    /// <summary>
    /// h2
    /// </summary>
    Level2
}

/// <summary>
/// Enum menu position 
/// </summary>
[Flags]
public enum MenuPosition
{
    /// <summary>
    /// None.
    /// </summary>
    None = 0,
    /// <summary>
    /// Main horizontal menu
    /// </summary>
    MenuTop = 1,
    /// <summary>
    /// Main toolbar menu
    /// </summary>
    MenuToolbar = 2,
    /// <summary>
    /// Left menu
    /// </summary>
    MenuSidebar = 4,
    /// <summary>
    /// Menu on the main nav tab bar
    /// </summary>
    MenuToolbarTabContainer = 8,
}

/// <summary>
/// Indicate if it's simple text or icon from library.
/// </summary>
public enum InputLabelType
{
    /// <summary>
    /// Simple text.
    /// </summary>
    Text,
    /// <summary>
    /// Icon's name from icon library.
    /// </summary>
    IconName
}

/// <summary>
/// Value to customize the display of required field.
/// </summary>
public enum RequiredInputDisplayMode
{
    /// <summary>
    /// No indication added to required or optional fields
    /// </summary>
    None,
    /// <summary>
    /// Indication added to optional fields
    /// </summary>
    OptionalOnly,
    /// <summary>
    /// Indication added to required fields
    /// </summary>
    MandatoryOnly,
    /// <summary>
    /// Indication added to required and optional fields
    /// </summary>
    Both
}

/// <summary>
/// Enum types of Media type (mobile, classic) 
/// </summary>
public enum MediaType
{
    /// <summary>
    /// Desktop size
    /// </summary>
    Desktop,
    /// <summary>
    /// Mobile size
    /// </summary>
    Mobile
}

/// <summary>
/// Enum media orientation (vertical, horizontal)
/// </summary>
public enum MediaOrientation
{
    /// <summary>
    /// Device is in vertical position
    /// </summary>
    Portrait,
    /// <summary>
    /// Device is in horizontal position
    /// </summary>
    Landscape
}

/// <summary>
/// Enum button size
/// </summary>
public enum ButtonSize
{
    /// <summary>
    /// Regular size for button.
    /// </summary>
    Medium,
    /// <summary>
    /// Smaller size for button.
    /// </summary>
    Small,
    /// <summary>
    /// Bigger size for button.
    /// </summary>
    Large
}

/// <summary>
/// Default accept header for HttpClient
/// </summary>
public enum DefaultAcceptHeader
{
    /// <summary>
    /// Use application/x-msgpack
    /// </summary>
    MessagePack,
    /// <summary>
    /// Use application/json
    /// </summary>
    Json
}

/// <summary>
/// Enum modal size 
/// </summary>
public enum ModalSize
{
    /// <summary>
    /// 300px
    /// </summary>
    Small,
    /// <summary>
    /// 500px
    /// </summary>
    Medium,
    /// <summary>
    /// 800px
    /// </summary>
    Large,
    /// <summary>
    /// 1140px
    /// </summary>
    ExtraLarge,
    /// <summary>
    /// 1600px
    /// </summary>
    ExtraExtraLarge
}

/// <summary>
/// Tooltip postion.
/// </summary>
public enum TooltipPosition
{
    /// <summary>
    /// Undefined.
    /// </summary>
    None, 
    /// <summary>
    /// Top
    /// </summary>
    Top,
    /// <summary>
    /// Right
    /// </summary>
    Right,
    /// <summary>
    /// Bottom
    /// </summary>
    Bottom,
    /// <summary>
    /// Left
    /// </summary>
    Left
}

/// <summary>
/// Post logout mode
/// </summary>
public enum PostLogoutMode
{
    /// <summary>
    /// Navigate to the PostLogoutUri 
    /// </summary>
    Navigation,
    /// <summary>
    /// Stay on application logout page and send an XHR to the specified PostLogoutUri
    /// </summary>
    XHR
}

/// <summary>
/// Event source of a closing event.
/// </summary>
public enum PageClosingSourceEvent
{
    /// <summary>
    /// User close the page.
    /// </summary>
    PageClose,
    /// <summary>
    /// User reload the page.
    /// </summary>
    PageReload,
    /// <summary>
    /// User quit application.
    /// </summary>
    ApplicationQuit
}

/// <summary>
/// Progress type.
/// </summary>
public enum ProgressType
{
    /// <summary>
    /// Progress bar.
    /// </summary>
    Bar,
    /// <summary>
    /// Circle progress.
    /// </summary>
    Circle
}

/// <summary>
/// Upload mode supported by the LgInputFile component
/// </summary>
public enum UploadMode
{
    /// <summary>
    /// Upload all selected file directly after selection
    /// </summary>
    Automatic,

    /// <summary>
    /// Show a button inside the LgInputFile component to fire the upload event
    /// </summary>
    Button,

    /// <summary>
    /// You have to call manually the Upload method on LgInputFile
    /// </summary>
    Manual
}

/// <summary>
/// Saving methods for opened tabs
/// </summary>
public enum TabSavingMethod
{
    /// <summary>
    /// Do  not save user tabs
    /// </summary>
    None,
    /// <summary>
    /// Save locally user tabs opened into IndexedDb
    /// </summary>
    Local,
    /// <summary>
    /// Save into remote database opened tabs
    /// </summary>
    Remote
}

/// <summary>
/// GridView profile storage mode
/// </summary>
public enum GridViewProfileStorage
{
    /// <summary>
    /// Store in the LocalStorage only.
    /// </summary>
    Local,
    /// <summary>
    /// Store in the remote database.
    /// </summary>
    Remote
}

/// <summary>
/// GridView profile save mode
/// </summary>
public enum GridViewProfileSave
{
    /// <summary>
    /// Profile saved automatically.
    /// </summary>
    Auto,
    /// <summary>
    /// Profile save with button.
    /// </summary>
    Button
}

/// <summary>
/// Datepicker mode
/// </summary>
public enum DateBoxKind
{
    /// <summary>
    /// Displays the days on the calendar 
    /// </summary>
    Day = 0,
    /// <summary>
    /// Displays the month on the calendar
    /// </summary>
    Month = 1,
    /// <summary>
    /// Displays the days on the calendar but choose only Monday is selected
    /// </summary>
    Week = 3
}

/// <summary>
/// Represents the different position types for toggle label.
/// </summary>
public enum ToggleTextPosition
{
    /// <summary>
    /// The label will be displayed to the left.
    /// </summary>
    Left,

    /// <summary>
    /// The label will be displayed to the right.
    /// </summary>
    Right
}
