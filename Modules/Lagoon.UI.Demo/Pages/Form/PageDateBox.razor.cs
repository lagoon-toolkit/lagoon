namespace Lagoon.UI.Demo.Pages.Form;

[Route(ROUTE)]
public partial class PageDateBox : DemoPageForm
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/datebox";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "DateBox", IconNames.All.ChatLeftTextFill);
    }

    #endregion

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        SetTitle(Link());
        DocumentationComponent = "381";

        formData.Date2 = null;
        formData.Year = null;
    }

    #region parameters
    public bool IsMonthPicker { get; set; } = false;
    private bool _haveMinDate = false;
    private bool HaveMinDate
    {
        get => _haveMinDate;
        set
        {
            if (!value)
            {
                DateMin = null;
            }

            _haveMinDate = value;
        }
    }
    private bool _haveMaxDate = false;
    private bool HaveMaxDate
    {
        get => _haveMaxDate;
        set
        {
            if (!value)
            {
                DateMax = null;
            }

            _haveMaxDate = value;
        }
    }
    private DateTime? DateMin { get; set; } = null;
    private DateTime? DateMax { get; set; } = null;

    private void OnDateChange(ChangeEventArgs args)
    {
        Console.WriteLine("OnChange DateBox done ==> " + formData.Date2.ToString() + "/" + args.Value.ToString());
        ShowSuccess("OnChange DateBox done ==> " + formData.Date2.ToString() + "/" + args.Value.ToString());
    }
    #endregion

    private static void OnDateChangeWeekPicker(ChangeEventArgs args)
    {
        Console.WriteLine("OnChange DateBox done ==> " + args.Value.ToString());
    }

    private static void OnDateChangeYearPicker(ChangeEventArgs args)
    {
        Console.WriteLine("OnChange YearPicker done ==> " + args.Value.ToString());
    }
}
