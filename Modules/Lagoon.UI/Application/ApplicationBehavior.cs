using Lagoon.UI.Application.Logging;

namespace Lagoon.UI.Application;


/// <summary>
/// Global Lagoon configuration
/// </summary>
public class ApplicationBehavior
{

    #region fields

    /// <summary>
    /// Color list
    /// </summary>
    private List<string> _colors;

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets the default button size for toolbar buttons in panel action.
    /// </summary>
    public ButtonSize ActionPanelToolbarButtonSize { get; set; } = ButtonSize.Small;

    /// <summary>
    /// Friendly message list to replace original messages.
    /// </summary>
    public AliasMessageCollection AliasMessages { get; } = new()
    {
        // DataValidation messages (https://github.com/dotnet/runtime/blob/main/src/libraries/System.ComponentModel.Annotations/src/Resources/Strings.resx)
        { AliasMessageComparisonOperator.FromFormat, "The {0} field is required.", "#RequiredAttribute_ValidationError", "#RequiredAttribute_ValidationError_Short" },
        { AliasMessageComparisonOperator.FromFormat, "The {0} field is not a valid e-mail address.", "#EmailAddressAttribute_Invalid", "#EmailAddressAttribute_Invalid_Short" }
    };

    /// <summary>
    /// Gets or sets the list of color available in the palette.
    /// </summary>
    public List<string> ColorPalette
    {
        get => _colors ?? GetDefaultColors();
        set => _colors = value;
    }

    /// <summary>
    /// Gets or sets the customer logo uri (use for about page)
    /// </summary>
    public string OwnerLogoUri { get; set; }

    /// <summary>
    /// List of uri logos 'developed by' (use for about page)
    /// </summary>
    public List<string> DevelopedByLogoUri { get; set; }

    /// <summary>
    /// Gets or sets LgEditForm behavior
    /// </summary>
    /// <returns>LgEditFrom defaults options</returns>
    public LgEditFormConfiguration EditForm { get; } = new();

    /// <summary>
    /// Define if the application should retrieve eula content from server (or locally in Dico.xml)
    /// </summary>
    public bool EulaFromServer { get; set; }

    /// <summary>
    /// Gets the default export provider manager.
    /// </summary>
    public ExportProviderManager ExportProviderManager { get; } = new(new CsvExportProvider());

    /// <summary>
    /// Gets or sets the default button size for toolbar buttons in frames.
    /// </summary>
    public ButtonSize FrameToolbarButtonSize { get; set; } = ButtonSize.Small;

    /// <summary>
    /// Define if the application is in tabbed navigation
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This parameter have been moved to LgApp component in \"App.razor\" file.", true)]
    public bool IsTabbed { get; set; }

    /// <summary>
    /// Logger configuration
    /// </summary>
    public LgClientLoggerOptions Logger { get; } = new();

    /// <summary>
    /// Specify the behavior of post logout
    /// </summary>
    [Obsolete("This property is no more use, define the \"App:PostLogoutUri\" value in the appSettings, on the server side application.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public PostLogoutMode PostLogoutMode { get; set; }

    /// <summary>
    /// Optional. Define the uri to call after successfully disconnected from the application.
    /// Can be usefull to disconnect from SSO when disconnecting from the application.
    /// </summary>
    [Obsolete("This property is no more use, define the \"App:PostLogoutUri\" value in the appSettings, on the server side application.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string PostLogoutUri { get; set; }

    /// <summary>
    /// Define if the application must require eula consent
    /// </summary>
    public bool RequireEulaConsent { get; set; }

    /// <summary>
    /// Define if the application is Rgaa
    /// </summary>
    public bool RgaaSupport { get; set; }

    /// <summary>
    /// Gets or sets the inputs configuration for reset button
    /// </summary>
    public LgInputConfiguration Input { get; } = new LgInputConfiguration();

    /// <summary>
    /// Gets or sets the LgSelect and LgSelectMultiple configuration
    /// </summary>
    public LgSelectConfiguration Select { get; } = new();

    /// <summary>
    /// Get or sets if the service worker auto install when application is launched.
    /// </summary>
    public bool ServiceWorkerAutoInstall { get; set; } = true;

    /// <summary>
    /// Gets or sets the service-worker activation state. Default is false 
    /// </summary>
    public bool ServiceWorkerEnabled { get; }

    /// <summary>
    /// Gets or sets the name of policy authorizing the management of shared profiles in the grids.
    /// </summary>
    [Obsolete("Use app.GridView(options => options.SharedProfileAdministratorPolicy = \"xxx\").")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string SharedProfileAdministratorPolicy { get; set; }

    /// <summary>
    /// Gets or sets the tab saving method on server database
    /// </summary>
    public TabSavingMethod TabSavingMethod { get; set; } = TabSavingMethod.None;

    /// <summary>
    /// Get or sets if the active tab content can be reload on click.
    /// </summary>
    public bool TabReloadEnabled { get; set; } = true;

    /// <summary>
    /// Toaster options
    /// </summary>
    public LgToastrOptions Toastr { get; set; } = new();

    /// <summary>
    /// Gets or sets the default button size for toolbar buttons.
    /// </summary>
    public ButtonSize ToolbarButtonSize { get; set; } = ButtonSize.Medium;

    /// <summary>
    /// Gets or sets a value which activate the dowload of "App" Node from appsettings.json (server side)
    /// </summary>
    /// <value><c>true</c> by default</value>
    public bool UseAppSettings { get; set; } = true;

    /// <summary>
    /// Gets or sets the service-worker activation state. Default is false 
    /// </summary>
    [Obsolete("Use now the \"ServiceWorkerEnabled\" property in the csproj file.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool UseServiceWorker { get; set; }

    /// <summary>
    /// Gets or sets the default accept header to specify if we want to use MessagePack or JsonSerializer.
    /// </summary>
    public DefaultAcceptHeader DefaultAcceptHeader { get; set; } = DefaultAcceptHeader.Json;

    /// <summary>
    /// Gets or sets filter text case and accent sensitivity.
    /// </summary>
    /// <remarks>By default depends of the collation defined on the main application DbContext; If none is defined, the default is case-sensitive.</remarks>
    public CollationType? TextFilterCollation { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// Initialize a new behavior configuration.
    /// </summary>
    /// <param name="serviceWorkerEnabled">The service-worker activation state.</param>
    public ApplicationBehavior(bool serviceWorkerEnabled)
    {
        ServiceWorkerEnabled = serviceWorkerEnabled;
    }

    #endregion

    #region methods

    /// <summary>
    /// Color list
    /// </summary>
    private static List<string> GetDefaultColors()
    {
        return new()
        {
            //Row 1
            "#000000",
            "#C00000",
            "#FF8000",
            "#FFC000",
            "#70AD47",
            "#918655",
            "#66B1CE",
            "#4472C4",
            "#44546A",
            "#9E5E9B",
            //Row 2
            "#FFFFFF",
            "#F8C6C6",
            "#FBE5D5",
            "#FFF2CC",
            "#E2EFD9",
            "#EAE7DB",
            "#E0EFF5",
            "#D9E2F3",
            "#D6DCE4",
            "#EBDEEB",
            //Row 3
            "#E0E0E0",
            "#F28E8D",
            "#F7CBAC",
            "#FEE599",
            "#C5E0B3",
            "#D5D0B8",
            "#C1DFEB",
            "#B4C6E7",
            "#ADB9CA",
            "#D8BED7",
            //Row 4
            "#C0C0C0",
            "#EC5654",
            "#F4B183",
            "#FFD965",
            "#A8D08D",
            "#C0B895",
            "#A3D0E1",
            "#8EAADB",
            "#8496B0",
            "#C59DC3",
            //Row 5
            "#808080",
            "#840F0E",
            "#C55A11",
            "#BF9000",
            "#538135",
            "#6C643F",
            "#388DAE",
            "#2F5496",
            "#323F4F",
            "#764674",
            //Row 6
            "#404040",
            "#580A09",
            "#833C0B",
            "#7F6000",
            "#375623",
            "#48432A",
            "#255E74",
            "#1F3864",
            "#222A35",
            "#4F2F4D",
            //Row 7
            "#FF0000",
            "#FFFF00",
            "#00FF00",
            "#008000",
            "#008080",
            "#00FFFF",
            "#0000FF",
            "#000080",
            "#800080",
            "#FF00FF",
        };
    }

    /// <summary>
    /// Get the legacy color list.
    /// </summary>
    public static List<string> GetLegacyDefaultColors()
    {
        return new()
        {
            "#FFEBED",
            "#FFCDD2",
            "#EE9A9A",
            "#E57373",
            "#EE534F",
            "#F44236",
            "#E53935",
            "#C9342D",
            "#C62827",
            "#B61C1C",
            "#FBE4EC",
            "#F9BBD0",
            "#F48FB1",
            "#F06292",
            "#EC407A",
            "#EA1E63",
            "#D81A60",
            "#C2175B",
            "#AD1457",
            "#890E4F",
            "#F3E5F6",
            "#E1BEE8",
            "#CF93D9",
            "#B968C7",
            "#AA47BC",
            "#9C28B1",
            "#8E24AA",
            "#7A1FA2",
            "#6A1B9A",
            "#4A148C",
            "#EEE8F6",
            "#D0C4E8",
            "#B39DDB",
            "#9675CE",
            "#7E57C2",
            "#673BB7",
            "#5D35B0",
            "#512DA7",
            "#45289F",
            "#301B92",
            "#E8EAF6",
            "#C5CAE8",
            "#9EA8DB",
            "#7986CC",
            "#5C6BC0",
            "#3F51B5",
            "#3949AB",
            "#303E9F",
            "#283593",
            "#1A237E",
            "#E4F2FD",
            "#BBDEFA",
            "#90CAF8",
            "#64B5F6",
            "#42A5F6",
            "#2196F3",
            "#1D89E4",
            "#1976D3",
            "#1564C0",
            "#0E47A1",
            "#E1F5FE",
            "#B3E5FC",
            "#81D5FA",
            "#4FC2F8",
            "#28B6F6",
            "#03A9F5",
            "#039BE6",
            "#0288D1",
            "#0277BD",
            "#00579C",
            "#DFF7F9",
            "#B2EBF2",
            "#80DEEA",
            "#4DD0E2",
            "#25C6DA",
            "#00BCD5",
            "#00ACC2",
            "#0098A6",
            "#00828F",
            "#016064",
            "#E0F2F2",
            "#B2DFDC",
            "#80CBC4",
            "#4CB6AC",
            "#26A59A",
            "#009788",
            "#00887A",
            "#00796A",
            "#00695B",
            "#004C3F",
            "#E8F6E9",
            "#C8E6CA",
            "#A5D6A7",
            "#80C783",
            "#66BB6A",
            "#4CB050",
            "#43A047",
            "#398E3D",
            "#2F7D32",
            "#1C5E20",
            "#F1F7E9",
            "#DDEDC8",
            "#C5E1A6",
            "#AED582",
            "#9CCC66",
            "#8BC24A",
            "#7DB343",
            "#689F39",
            "#548B2E",
            "#33691E",
            "#F9FBE6",
            "#F0F4C2",
            "#E6EE9B",
            "#DDE776",
            "#D4E056",
            "#CDDC39",
            "#C0CA33",
            "#B0B42B",
            "#9E9E24",
            "#817716",
            "#FFFDE8",
            "#FFFAC3",
            "#FFF59C",
            "#FFF176",
            "#FFEE58",
            "#FFEB3C",
            "#FDD734",
            "#FAC02E",
            "#F9A825",
            "#F47F16",
            "#FEF8E0",
            "#FFECB2",
            "#FFE083",
            "#FFD54F",
            "#FFC928",
            "#FEC107",
            "#FFB200",
            "#FF9F00",
            "#FF8E01",
            "#FF6F00",
            "#FFF2DF",
            "#FFE0B2",
            "#FFCC80",
            "#FFB64D",
            "#FFA827",
            "#FF9700",
            "#FB8C00",
            "#F67C01",
            "#EF6C00",
            "#E65100",
            "#FBE9E7",
            "#FFCCBB",
            "#FFAB91",
            "#FF8A66",
            "#FF7143",
            "#FE5722",
            "#F5511E",
            "#E64A19",
            "#D74315",
            "#BF360C",
            "#EFEBEA",
            "#D7CCC8",
            "#BCABA4",
            "#A0887E",
            "#8C6E63",
            "#7B5347",
            "#6D4D42",
            "#5D4038",
            "#4D342F",
            "#3E2622",
            "#FAFAFA",
            "#F5F5F5",
            "#EEEEEE",
            "#E0E0E0",
            "#BDBDBD",
            "#9E9E9E",
            "#757575",
            "#616161",
            "#424242",
            "#212121",
            "#EBEFF2",
            "#CED9DD",
            "#B0BFC6",
            "#90A4AD",
            "#798F9A",
            "#607D8B",
            "#546F7A",
            "#465A65",
            "#36474F",
            "#273238"
        };
    }

    #endregion

}
