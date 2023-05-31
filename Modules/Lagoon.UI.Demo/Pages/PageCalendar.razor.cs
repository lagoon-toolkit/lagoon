namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageCalendar : DemoPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/calendar";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "Calendar", IconNames.All.Calendar);
    }

    #endregion

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "374";
    }

    #region Public properties

    public List<CalendarIndicator> RangesSource { get; set; } = new List<CalendarIndicator>();

    #endregion

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _startDate = new DateTime(2021, 1, 1);
        _endDate = new DateTime(2021, 12, 31);

        CalendarIndicatorRangeCollection DataZoneA = new()
        {
            new CalendarIndicatorRange()
            {
                StartDate = new DateTime(2021, 01, 12),
                EndDate = new DateTime(2021, 01, 22)
            },
            new CalendarIndicatorRange()
            {
                StartDate = new DateTime(2021, 04, 27),
                EndDate = new DateTime(2021, 05, 05)
            }
        };

        CalendarIndicatorRangeCollection DataZoneB = new()
        {
            //DataZoneB.Add(new YearlyCalendarDataRange()
            //{
            //    StartDate = new DateTime(2021, 01, 12),
            //    EndDate = new DateTime(2021, 01, 19)
            //});
            new CalendarIndicatorRange()
            {
                StartDate = new DateTime(2021, 04, 25),
                EndDate = new DateTime(2021, 05, 12)
            },
            new CalendarIndicatorRange()
            {
                StartDate = new DateTime(2021, 07, 16),
                EndDate = new DateTime(2021, 07, 16)
            }
        };

        CalendarIndicatorRangeCollection DataZoneC = new()
        {
            new CalendarIndicatorRange()
            {
                StartDate = new DateTime(2021, 01, 12),
                EndDate = new DateTime(2021, 01, 19)
            },
            new CalendarIndicatorRange()
            {
                StartDate = new DateTime(2021, 04, 25),
                EndDate = new DateTime(2021, 05, 19)
            },
            new CalendarIndicatorRange()
            {
                StartDate = new DateTime(2021, 07, 13),
                EndDate = new DateTime(2021, 07, 16)
            }
        };

        RangesSource.Add(new CalendarIndicator()
        {
            Id = "Range1",
            Label = "ZoneA",
            Color = "red",
            Ranges = DataZoneA
        });
        RangesSource.Add(new CalendarIndicator()
        {
            Id = "Range2",
            Label = "ZoneB",
            Color = "Green",
            Ranges = DataZoneB
        });
        RangesSource.Add(new CalendarIndicator()
        {
            Id = "Range3",
            Label = "ZoneC",
            Color = "Blue",
            Ranges = DataZoneC
        });

    }

    public void OnClickDay(ClickDayEventArgs e)
    {
        ShowInformation(e.Date.ToShortDateString());
    }

    private DateTime _startDate;
    private DateTime _endDate;
}
