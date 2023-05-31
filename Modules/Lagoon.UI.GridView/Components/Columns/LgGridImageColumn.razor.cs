namespace Lagoon.UI.Components;

/// <summary>
/// Gridview image column
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class LgGridImageColumn<TItem> : LgGridBaseValueColumn<TItem>
{

    //TODO: rajouter un Alt

    #region parameters

    /// <summary>
    /// Gets or sets image height
    /// </summary>
    /// <value></value>
    [Parameter]
    public string ImageHeight { get; set; } = "100%";

    /// <summary>
    /// Gets or sets image width
    /// </summary>
    /// <value></value>
    [Parameter]
    public string ImageWidth { get; set; } = "100%";

    /// <summary>
    /// Gets or sets begin of the url
    /// </summary>
    /// <value></value>
    [Parameter]
    public string UrlPrefix { get; set; }

    /// <summary>
    /// Gets or sets end of the url
    /// </summary>
    /// <value></value>
    [Parameter]
    public string UrlSuffix { get; set; }

    #endregion Parameters

    #region methods

    ///<inheritdoc/>
    internal override Type OnGetCellComponentType(Type itemType, Type columnValueType, Type cellValueType)
    {
        return typeof(LgGridImageCell<,,>).MakeGenericType(itemType, columnValueType, cellValueType);
    }

    ///<inheritdoc/>
    internal override Type OnGetFilterType(Type cellValueType)
    {
        return null;
    }

    ///<inheritdoc/>
    internal override Type OnGetFilterBoxType(Type cellValueType)
    {
        return null;
    }

    #endregion

}