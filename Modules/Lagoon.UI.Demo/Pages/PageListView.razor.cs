using Lagoon.UI.Demo.ViewModel;

namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageListView : DemoPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/listview";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "ListView", IconNames.All.Wrench);
    }

    #endregion

    private List<Project> _projectsItems = new();
    private List<Project> _selectedProjects = new();
    private string _groupBy;
    private List<string> _propProjectList = new();

    private void AddProject()
    {
        _projectsItems.Add(new Project() { Id = _projectsItems.Count + 1, Name = "Project TBD n°" + _projectsItems.Count + 1, Status = ProjectStatus.InProgress });
        StateHasChanged();
    }

    private void GroupByStatus()
    {
        _groupBy = string.IsNullOrEmpty(_groupBy) ? nameof(Project.Status) : string.Empty;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        LoadData();
    }

    private void LoadData()
    {
        _projectsItems.Add(new Project() { Id = _projectsItems.Count + 1, Name = "LagoonGenix", Status = ProjectStatus.InProgress });
        _projectsItems.Add(new Project() { Id = _projectsItems.Count + 1, Name = "Lagoonify", Status = ProjectStatus.Critical });
        _projectsItems.Add(new Project() { Id = _projectsItems.Count + 1, Name = "Lagoontastic", Status = ProjectStatus.Done });
        _projectsItems.Add(new Project() { Id = _projectsItems.Count + 1, Name = "Mega Lagoon", Status = ProjectStatus.Critical });
        _projectsItems.Add(new Project() { Id = _projectsItems.Count + 1, Name = "Sky Lagoon", Status = ProjectStatus.Done });

    }

    /// <summary>
    /// Get color matching the status
    /// </summary>
    public static string GetColorBar(ProjectStatus status)
    {
        if (status == ProjectStatus.Done)
        {
            return "#62AC87";
        }
        else
        {
            return status == ProjectStatus.Critical ? "#CB2A55" : "#DF7430";
        }
    }

    /// <summary>
    /// Mark as done selected value
    /// </summary>
    public void MarkAsDone(List<Project> projects)
    {
        foreach (Project project in projects)
        {
            project.Status = ProjectStatus.Done;
        }
        ShowSuccess("Status successfully changed");
        StateHasChanged();
    }

    /// <summary>
    /// Mark as done selected value
    /// </summary>
    public void Delete(List<Project> projects)
    {
        foreach (Project project in projects)
        {
            _projectsItems.Remove(project);
        }
        ShowSuccess("Projects successfully deleted");
        _selectedProjects.Clear();
        StateHasChanged();
    }
}
