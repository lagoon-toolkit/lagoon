using Lagoon.UI.Components.WorkScheduler.Configuration;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageWorkScheduler : DemoPage
{

    #region Private properties

    private DateTime _from = new(2022, 1, 1);
    private DateTime _to = new(2023, 3, 31);
    private IList<TestData> _data = null;
    private IList<TestData> _dataWithoutFilter = null;
    private TimelineMode _timelineMode = TimelineMode.MonthDay;
    private DisplayMode _displayMode = DisplayMode.Timeline;
    private TimelineDisplayMode _timelineDisplayMode = TimelineDisplayMode.TwoRows;
    private double _zoomLevel = 1;
    private bool _showWeekend = true;
    private bool _virtualScrolling = false;
    public bool _projectResize = true;
    private int _rowHeight = 40;
    private int _projectListWidth = 200;
    private int _paddingRowHeight = 7;
    private int _groupMargin = 30;
    private int _agendaRowStep = 30;
    private bool _show = true;
    private bool _showMouseIndicator = true;
    private bool _activeGroupBy = false;
    private bool _activeGroupName = false;
    private bool _activeSubGroupName = false;
    private bool _groupAsColumn = false;
    private bool _groupAsFixedColumn = false;
    private bool _groupCustomStyle = true;
    private bool _showEditModal = false;
    private bool _showEditProjectModal = false;
    private TestData _editedItem = null;
    private TimeSpan _agendaHourFrom = new(6, 0, 0);
    private TimeSpan _agendaHourTo = new(21, 0, 0);

    private TimeSpan _agendaHlHourFromTS
    {
        get
        {
            return new TimeSpan(_agendaHlHourFrom.Hour, _agendaHlHourFrom.Minute, _agendaHlHourFrom.Second);
        }
    }

    private DateTime _agendaHlHourFrom = new(2000, 1, 1, 8, 0, 0);
    private TimeSpan _agendaHlHourTo = new(19, 0, 0);

    private int _count = 100;
    private string _filterText;

    private bool? _hasModification;


    private List<Func<TestData, object>> _groupBy = null;

    private LgWorkScheduler<TestData> _workScheduler;

    private bool _showJoinMilestone = true;

    private DragStepType _dragStepType = DragStepType.Free;

    private bool _createTaskOnClick = false;

    private bool _overflowWkrRow = false;

    private bool _highlightWeekend = false;

    #endregion

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "wks";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "Workscheduler", IconNames.All.Calendar);
    }

    #endregion

    private void SaveWkChanges()
    {
        _hasModification = false;
    }

    private void SetModified()
    {
        _hasModification = true;
    }

    #region Initialisation

    /// <summary>
    /// Load WorkScheduler data
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _hasModification = false;
        // Generate random data
        _data = new ObservableCollection<TestData>();
        GenerateData(_virtualScrolling);


    }

    private void OnItemClick(TestData item)
    {
        ShowInformation("Item clicked : " + item.ToString());
        OpenModalEditProjectBar(item);
    }

    private bool _cancelLastDragAndDrop = false;

    private void WksDragCompleted(OnDragCompletedEventArgs<TestData> e)
    {
        Console.WriteLine($"WksDragCompleted {e.RowId} / {e.NewStart} / {e.NewEnd} ");
        try
        {
            TestData currentRow = _data.Where(x => x.RowId == int.Parse(e.RowId.ToString())).FirstOrDefault();

            if (!_cancelLastDragAndDrop)
            {

                if (e.IsNew) // Comming for an external drag
                {

                    _data.Add(new TestData(int.Parse(e.RowId.ToString()))
                    {
                        From = e.NewStart,
                        To = _displayMode == DisplayMode.Timeline ? e.NewStart.AddDays(4) : e.NewStart.AddMinutes(30),//e.NewEnd,
                        Background = "#41a0a0",
                        AllowHorizontalMove = true,
                        AllowVerticalMove = true,
                        ProjectBarInfo = $"{e.DroppedItemId}",
                        IsExternal = true,
                        GroupName = currentRow?.GroupName,
                        SubGroupName = currentRow?.SubGroupName
                    });
                    _externalDraggableItems.Remove(e.DroppedItemId);
                    ShowInformation("Item dragged from external : Id:'" + e.DroppedItemId);

                }
                else
                {
                    ShowInformation("Item dragged : '" + e.RowId + "' " + e.Item.ToString() + " " + e.NewStart.ToShortDateString() + " " + e.NewStart.ToLongTimeString());
                    if (e.Item.AllowVerticalMove && _displayMode == DisplayMode.Timeline)
                    {
                        e.Item.RowId = int.Parse(e.RowId.ToString());
                        e.Item.GroupName = currentRow?.GroupName;
                        e.Item.SubGroupName = currentRow?.SubGroupName;
                    }
                }
            }
            else
            {
                e.Cancel = true;
                ShowConfirm("Are you sure ?", () =>
                {
                    if (e.IsNew)
                    {
                        _data.Add(new TestData(int.Parse(e.RowId.ToString()))
                        {
                            From = e.NewStart,
                            To = e.NewEnd,
                            Background = "#41a0a0",
                            AllowHorizontalMove = true,
                            AllowVerticalMove = true,
                            GroupName = currentRow?.GroupName,
                            SubGroupName = currentRow?.SubGroupName
                        });
                        _externalDraggableItems.Remove(e.DroppedItemId);
                    }
                    else
                    {
                        e.Item.From = e.NewStart;
                        e.Item.To = e.NewEnd;
                        e.Item.RowId = int.Parse(e.RowId.ToString());
                        e.Item.GroupName = currentRow?.GroupName;
                        e.Item.SubGroupName = currentRow?.SubGroupName;
                    }
                    StateHasChanged();
                    return Task.CompletedTask;
                });
            }
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    // Insert new task where user clicked
    private void OnEmptyClick(OnEmptyClickEventArgs e)
    {
        Console.WriteLine("OnEmptyClick page test");
        if (_createTaskOnClick)
        {
            TestData newItem = null;
            if (_displayMode == DisplayMode.Timeline)
            {
                TestData currentRow = _data.Where(x => x.RowId == int.Parse(e.RowId.ToString())).FirstOrDefault();

                var newDate = new DateTime(e.Date.Year, e.Date.Month, e.Date.Day);
                newItem = new TestData(int.Parse(e.RowId.ToString()))
                {
                    From = newDate,
                    To = newDate.AddDays(1),
                    Background = "#FFD965",
                    GroupName = currentRow?.GroupName,
                    SubGroupName = currentRow?.SubGroupName
            };
            }
            else
            {
                var newDate = e.Date;
                newItem = new TestData(0)
                {
                    From = newDate,
                    To = newDate.AddMinutes(_agendaRowStep),
                    Background = "#FFD965"
                };
            }
            _data.Add(newItem);
            OpenModalEditProjectBar(newItem);
        }
        else
        {
            ShowInformation($"OnEmptyClick {e.Date}");
        }
    }

    #endregion

    #region Actions (component options)

    private void UpdateData()
    {
        //_data.First().RowId = 666666;
        //_data.Insert(1, new TestData(555));

        //_data = new List<TestData>();
        //_data.Add(new TestData(555));

        var item = _data.Skip(10).First();
        item.Background = "purple";
        //zz item.To = item.To.Value.AddDays(10);
        _hasModification = true;

        //List<TestData> copy = new(_data);
        //if (_data != null)
        //{
        //    _data.Clear();
        //    _data.Add(new TestData(555) { GroupName = "GRP", SubGroupName = "GRP2" });
        //    _data.Add(new TestData(556) { GroupName = "GRP", SubGroupName = "GRP2" });

        //    foreach (var item in copy)
        //    {
        //        _data.Add(item);
        //    }
        //    //_data.AddRange(copy);
        //}
    }


    private IEnumerable<TestData> _sortTestData(IEnumerable<TestData> items)
    {
        return items.OrderBy(i => i.GroupName).ThenBy(i => i.SubGroupName).ThenByDescending(i => i.ProjectName);
    }

    private bool _toolbarCollapsed = false;

    private void ToggleToolbar()
    {
        _toolbarCollapsed = !_toolbarCollapsed;
    }

    private void zoomIn()
    {
        _zoomLevel += 0.2;
        Console.WriteLine($"ZoomLevel: {_zoomLevel}");
    }

    private void zoomOut()
    {
        if (_zoomLevel > 0.2)
            _zoomLevel -= 0.2;
    }

    private Task ScrollToToday()
    {
        return _workScheduler.ScrollToTodayAsync();
    }
    private void rowIncrease()
    {
        _rowHeight += 10;
    }

    private void rowDecrease()
    {
        _rowHeight -= 10;
    }

    private void paddingRowIncrease()
    {
        _paddingRowHeight += 1;
    }

    private void paddingRowDecrease()
    {
        _paddingRowHeight -= 1;
    }

    private void groupIncrease()
    {
        _groupMargin += 10;
    }

    private void groupDecrease()
    {
        _groupMargin -= 10;
    }

    private void UpdateGroupBy()
    {
        if (_activeGroupName || _activeSubGroupName)
        {
            _groupBy = new List<Func<TestData, object>>();
            if (_activeGroupName)
            {
                _groupBy.Add(e => e.GroupName);
            }
            if (_activeSubGroupName)
            {
                _groupBy.Add(e => e.SubGroupName);
            }
        }
        else
        {
            _groupBy = null;
        }
        _projectListWidth = _displayMode == DisplayMode.Timeline && _groupAsColumn ? 500 : 250;
    }

    private async Task ForceReloadSchedulerAsync(ChangeEventArgs e)
    {

        _show = false;
        StateHasChanged();
        await Task.Delay(10);

        bool vs = bool.Parse(e.Value.ToString());
        GenerateData(vs);

        _show = true;
        StateHasChanged();
    }

    private void OnGroupChange(ChangeEventArgs e)
    {
        _data.Where(x => x.RowId == _editedItem.RowId).ToList().ForEach(x => x.GroupName = (string)e.Value);
    }
    private void OnSubGroupChange(ChangeEventArgs e)
    {
        _data.Where(x => x.RowId == _editedItem.RowId).ToList().ForEach(x => x.SubGroupName = (string)e.Value);
    }

    private void GenerateData(bool virtualScrolling)
    {
        bool modeAgenda = _displayMode == DisplayMode.Agenda;
        if (_data != null)
        {
            _data.Clear();

            if (_activeGroupBy)
            {
                _groupBy = new List<Func<TestData, object>>() { e => e.GroupName, e => e.SubGroupName }; //, e => e.SubGroupName
            }
            else
            {
                _groupBy = null;
            }

            if (!modeAgenda)
            {
                for (var i = 0; i < _count; i++)
                {
                    _data.Add(new TestData(i) { GroupName = i < 10 ? "First" : "Second", SubGroupName = i < 5 ? "Sub A" : "Sub B", AllowHorizontalMove = true, AllowVerticalMove = true, Background = (i == 6 ? "orange" : null) });
                }

                // Insert a Milestone in the first row
                var newFrom = _data.First().From.AddDays(10);
                _data.Insert(0, new TestData(0) { GroupName = "First", SubGroupName = "Sub A", From = newFrom, To = null, Background = "#B8ECF6A8" });
                _data.Insert(0, new TestData(0) { GroupName = "First", SubGroupName = "Sub A", From = newFrom.AddDays(1), To = null, Background = "#B8ECF6A8" });

                _data[0].From = _data[0].From.AddHours(13).AddMinutes(30);

                //_data.Insert(0, new TestData(0) { GroupName = "First", SubGroupName = "Sub A", From = newFrom.AddDays(10), To = null, Background = "green" });
                //_data.Insert(0, new TestData(0) { GroupName = "First", SubGroupName = "Sub A", From = newFrom.AddDays(20), To = null, Background = "green" });
                //_data.Insert(0, new TestData(0) { GroupName = "First", SubGroupName = "Sub A", From = newFrom, To = newFrom.AddDays(5), Background = "repeating-linear-gradient(-55deg, #B8ECF6, #B8ECF6 10px, #007bff61 10px, #007bff61 20px)" });

                _data.Add(new TestData(2) { GroupName = "First", SubGroupName = "Sub A", From = new DateTime(2022, 1, 15), To = null, Background = "green" });
                _data.Add(new TestData(3) { GroupName = "First", SubGroupName = "Sub A", From = new DateTime(2022, 1, 20), To = null, Background = "purple", AllowVerticalMove = true, AllowHorizontalMove = true });
            }
            else
            {
                _data.Add(new TestData(0) { From = new DateTime(2023, 3, 8, 10, 0, 0), To = new DateTime(2023, 3, 8, 11, 15, 0), Background = "#B8ECF6A8", ProjectBarInfo = "De 10h à 11h15" });
                //_data.Add(new TestData(1) { From = new DateTime(2023, 3, 8, 13, 0, 0), To = new DateTime(2023, 3, 8, 14, 00, 0), Background = "#B8ECF6A8" });
                //_data.Add(new TestData(2) { From = new DateTime(2023, 3, 9, 10, 0, 0), To = new DateTime(2023, 3, 8, 15, 00, 0), Background = "#B8ECF6A8" });
                //_data.Add(new TestData(3) { From = new DateTime(2023, 3, 10, 9, 0, 0), To = new DateTime(2023, 3, 10, 12, 0, 0), Background = "#B8ECF6A8" });
            }

            foreach (var d in _data)
            {
                d.AllowVerticalMove = d.AllowHorizontalMove = true;
            }

            _dataWithoutFilter = new List<TestData>(_data.AsEnumerable());
        }

    }

    private void SetDisplayMode(DisplayMode mode)
    {
        _displayMode = mode;
        if (_displayMode == DisplayMode.Agenda)
        {
            _rowHeight = 50;
            _zoomLevel = 10;
            _virtualScrolling = false;
            _timelineMode = TimelineMode.MonthDay;
        }
        else
        {
            _rowHeight = 30;
            _zoomLevel = 1;
        }
        GenerateData(_virtualScrolling);
    }

    private List<string> _externalDraggableItems = new List<string>
        {
            "10000", "10001", "10002", "10003", "10004", "10005"
        };

    #endregion

    #region UI Interactions (by the developper)

    private void OpenEditModal(TestData item)
    {
        _editedItem = item;
        _showEditModal = true;
    }

    private void CloseEditModal()
    {
        _showEditModal = false;
        _editedItem = null;
    }

    private void RemoveItem(TestData item)
    {
        _data?.Remove(item);
    }

    private void ValidateItem(TestData item)
    {
        // Retrieve all associated elements from the datasource
        var items = _data.Where(x => x.RowId == item.RowId);
        foreach (var dataitem in items)
        {
            // Set the background task (binded to task renderer in the SchedulerView RenderFragment)
            dataitem.Background = "green";
            // Set the background/foreground task (binded to project renderer in the ProjectView RenderFragement)
            dataitem.ProjectBackground = "green";
            dataitem.ProjectColor = "white";
        }
        ShowSuccess("Project Validated");
    }

    private void OpenModalEditProjectBar(TestData item)
    {
        _editedItem = item;
        _showEditProjectModal = true;
    }

    private void CloseEditBarModal()
    {
        //_hasModification = true;
        _showEditProjectModal = false;
        _editedItem = null;
    }

    private void OnTextFilterChanged(ChangeEventArgs e)
    {
        if (_data != null)
        {
            _data.Clear();
            //_data.AddRange(_dataWithoutFilter.Where(x => x.ProjectName.ToLower().Contains(e.Value.ToString().ToLower())).ToList());
            var t = _dataWithoutFilter.Where(x => x.ProjectName.ToLower().Contains(e.Value.ToString().ToLower())).ToList();
            foreach (var tt in t)
            {
                _data.Add(tt);
            }
        }
    }

    #endregion

    #region Test data

    public class TestData : IWorkSchedulerDraggableData, INotifyPropertyChanged
    {

        public int RowId { get; set; }

        private DateTime _from;

        public DateTime From
        {
            get
            {
                return _from;
            }
            set
            {
                OnPropertyChanged(nameof(From));
                _from = value;
            }
        }

        private DateTime? _to;

        public DateTime? To
        {
            get
            {
                return _to;
            }
            set
            {
                OnPropertyChanged(nameof(To));
                _to = value;
            }
        }

        public bool AllowHorizontalMove { get; set; } = true;
        public bool AllowVerticalMove { get; set; } = true;

        public string ProjectName { get; set; }

        public string ProjectBarInfo { get; set; }

        public string ProjectColor { get; set; }

        public string ProjectBackground { get; set; }

        public bool CanDelete { get; set; }

        public string GroupName { get; set; }

        public string SubGroupName { get; set; }

        public string Background { get; set; } = "#B8ECF6A8;";

        public bool IsExternal { get; set; }

        public TestData(int rowId)
        {
            RowId = rowId;
            ProjectColor = "black";
            ProjectBarInfo = rowId.ToString();
            ProjectName = $"Project {RowId}";
            CanDelete = RowId == 9;
            if (rowId == 2)
            {
                AllowHorizontalMove = AllowVerticalMove = true;
            }
            Random rnd = new();

            From = new DateTime(2022, rnd.Next(1, 1), rnd.Next(1, 31));
            To = From.AddDays(rnd.Next(1, 10));

            if (rowId == 10)
            {
                ProjectColor = "red";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }


        public string CssClass()
        {
            if (!To.HasValue)
            {
                if (RowId == 0)
                {
                    return "wk-milestone-square";
                }
                else if (RowId == 2)
                {
                    return "wk-milestone-arrow-up arrow-red";
                }
                else if (RowId == 3)
                {
                    return "custom-milestone-circle";
                }
            }
            return string.Empty;
        }

        public override string ToString()
        {
            return $"Item {RowId}";
        }

    }

    #endregion

}