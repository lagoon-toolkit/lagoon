namespace Lagoon.UI.Components;

/// <summary>
/// Header cell
/// </summary>
public partial class LgGridHeaderGroupCell<TItem> : LgGridBaseCell<TItem>
{
    #region properties

    /// <summary>
    /// Indicate if header group has name
    /// </summary>
    private bool _hasGroupName;

    /// <summary>
    /// Group name
    /// </summary>
    private string _groupName;

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _groupName = Column.State.GetGroupName();
        _hasGroupName = !string.IsNullOrEmpty(_groupName);
    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        if (!_hasGroupName)
        {
            builder.Add("empty-cell");
        }
    }

    #endregion
}
