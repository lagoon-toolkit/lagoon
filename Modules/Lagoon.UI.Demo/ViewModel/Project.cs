using System.ComponentModel.DataAnnotations;

namespace Lagoon.UI.Demo.ViewModel;

public class Project
{
    public int Id { get; set; }

    public string Name { get; set; }

    public ProjectStatus Status { get; set; }

    public Project()
    {

    }
}

public enum ProjectStatus
{
    [Display(Name = "In progress")]
    InProgress,

    Done,

    Critical
}

