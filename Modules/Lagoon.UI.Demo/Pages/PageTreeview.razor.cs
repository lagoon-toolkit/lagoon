namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageTreeview : DemoPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/treeview";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "Treeview", IconNames.All.Tree);
    }

    #endregion

    #region fields

    /// <summary>
    /// Page loading status.
    /// </summary>
    private TreeNodeCollection<int> _nodes;

    #endregion

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "2559";
        PopulateTreeview();
    }

    private void PopulateTreeview()
    {
        _nodes = new TreeNodeCollection<int>();
        var endDate = new DateTime(2022, 1, 1);
        int prevMonth = 0;
        int prevYear = 0;
        TreeNode<int> treeViewItemYear = null;
        TreeNode<int> treeViewItemMonth = null;
        for (DateTime current = new(2020, 1, 1); current < endDate; current = current.AddDays(10))
        {
            if (prevYear != current.Year)
            {
                treeViewItemYear = new TreeNode<int>(current.Year)
                {
                    Expanded = true
                };
                _nodes.Add(treeViewItemYear);

            }
            prevYear = current.Year;
            if (prevMonth != current.Month)
            {
                treeViewItemMonth = treeViewItemYear.AddChild(current.Month);
                treeViewItemMonth.Expanded = true;
            }
            prevMonth = current.Month;
            treeViewItemMonth.AddChild(current.Day);
        }
    }

    /// <summary>
    /// Display 
    /// </summary>
    /// <param name="node"></param>
    private void NodeSelected(TreeNode<int> node)
    {
        ShowInformation($"Node => {node.Item}");
    }

    #region parameters

    /// <summary>
    /// Gets or sets treeview style
    /// </summary>
    public TreeviewKind Kind { get; set; } = TreeviewKind.None;

    #endregion
}
