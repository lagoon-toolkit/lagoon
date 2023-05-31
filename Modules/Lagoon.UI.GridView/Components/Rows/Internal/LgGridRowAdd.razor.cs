namespace Lagoon.UI.Components.Internal;

/// <summary>
/// The add line of the datagrid.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class LgGridRowAdd<TItem> : LgGridRow<TItem>
{

    #region parameter

    /// <summary>
    /// Gets or sets if the add line has been placed above the data or after.
    /// </summary>
    [Parameter]
    public bool IsTop { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public LgGridRowAdd()
    {
        Index = -1;
        IsAddRow = true;
    }

    #endregion

    #region method


#if DEBUG //TOCLEAN

    private readonly System.Collections.Generic.Dictionary<string, object> _oldParameters = new();

    ///<inheritdoc/>
    public override async System.Threading.Tasks.Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);
        System.Text.StringBuilder sb = new();
        bool isEqual;
        foreach (ParameterValue par in parameters)
        {
            if (_oldParameters.TryGetValue(par.Name, out object oldValue))
            {
                isEqual = Equals(oldValue, par.Value);
            }
            else
            {
                isEqual = false;
            }
            if (!isEqual)
            {
                sb.AppendLine();
                sb.Append('\t');
                sb.Append($"Name: {par.Name} {(par.Cascading ? "[CASCADING] " : "")}, Value: {par.Value}");
                if (!_oldParameters.TryAdd(par.Name, par.Value))
                {
                    _oldParameters[par.Name] = par.Value;
                }
            }
        }
        Lagoon.Helpers.Trace.ToConsole(this, $"{(sb.Length == 0 ? "NO CHANGES" : sb)}");
    }

#endif

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add("gridview-row-add");
        builder.AddIf(IsTop, "addrow-top");
    }

    #endregion

}
