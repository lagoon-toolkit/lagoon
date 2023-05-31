using System.Web;

namespace Lagoon.UI.Components;

/// <summary>
/// Gridview - profiles
/// </summary>
public partial class LgGridView<TItem>
{

    #region fields

    /// <summary>
    /// The current statedId used for profiles
    /// </summary>
    private string _profileStateId;

    /// <summary>
    /// Profile key in local storage
    /// </summary>
    private string _profileIdPattern; // {StateId}-{ProfileIndex}

    /// <summary>
    /// Profile key in local storage
    /// </summary>
    private string _profileKeyPattern; // {LocalStoragePrefix}GridView-Profile-{ProfileId}

    /// <summary>
    /// Profile list key in local storage
    /// </summary>
    private string _profilesListKey; // {LocalStoragePrefix}GridView-ListProfile-{StateId}

    /// <summary>
    /// Last selected profile key in local storage
    /// </summary>
    private string _lastSelectedProfileKey; // {LocalStoragePrefix}GridView-LastProfile-{StateId}

    /// <summary>
    /// Profile modal ref
    /// </summary>
    private LgModal _lgModalProfiles;

    /// <summary>
    /// Profile list
    /// </summary>
    private List<ProfileItem> _profileItems = new();

    /// <summary>
    /// Add profile model
    /// </summary>
    private AddProfileModel _addProfileModel = new();

    /// <summary>
    /// LgEditForm ref
    /// </summary>
    private LgEditForm _formProfile;

    /// <summary>
    /// The current profile.
    /// </summary>
    private ProfileItem _currentProfileItem;

    /// <summary>
    /// The default profile.
    /// </summary>
    private ProfileItem _defaultProfileItem;

    /// <summary>
    /// Indicate if profile are active
    /// </summary>
    private bool _allowProfiles;

    /// <summary>
    /// check if user has access to Shared Profile management
    /// </summary>
    private bool SharedProfileManagement { get; set; }

    /// <summary>
    /// check if user has access to Shared Profile
    /// </summary>
    private bool HasAccessToSharedProfile => !string.IsNullOrEmpty(SharedProfileAdministratorPolicy);

    /// <summary>
    /// Profile database action cancellation
    /// </summary>
    private CancellationTokenSource _profileCancellationTokenSource;

    #endregion Fields

    #region properties

    /// <summary>
    /// Gets or sets current active profile
    /// </summary>
    internal GridViewProfile CurrentProfile { get; set; }

    #endregion

    #region methods

    #region Storage profile

    /// <summary>
    /// Init LocalStorage key pattern
    /// </summary>
    private void InitProfileKeysPattern()
    {
        if (_profileStateId != StateId)
        {
            _profileStateId = StateId;
            _profileIdPattern = $"{StateId}-{{0}}";
            _profileKeyPattern = App.GetLocalStorageKey($"GridView-Profile-{{0}}");
            _profilesListKey = App.GetLocalStorageKey($"GridView-ListProfile-{StateId}");
            _lastSelectedProfileKey = App.GetLocalStorageKey($"GridView-LastProfile-{StateId}");
        }
    }

    /// <summary>
    /// Get or initialize grid profiles
    /// </summary>
    /// <returns></returns>
    private async Task InitializeProfileAsync(bool useSharedProfiles)
    {
        _allowProfiles = Features.HasFlag(GridFeature.Profile);
        // Get profile list            
        if (StorageMode == GridViewProfileStorage.Local)
        {
            // Get profile list from LocalStorage
            _profileItems = App.LocalStorage.GetItem<List<ProfileItem>>(_profilesListKey) ?? new();
            // Get shared profile list 
            if (useSharedProfiles)
            {
                _profileItems.AddRange(await JsonHttp.TryGetAsync<List<ProfileItem>>
                    ($"LgGridViewProfile/List/Shared/{HttpUtility.UrlEncode(StateId)}"));
            }
        }
        else
        {
            // Get shared profile list 
            string target = useSharedProfiles ? "All" : "User";            
            _profileItems = await JsonHttp.TryGetAsync<List<ProfileItem>>
                    ($"LgGridViewProfile/List/{target}/{HttpUtility.UrlEncode(StateId)}") ?? new List<ProfileItem>();
        }
        // Find the default profile
        _defaultProfileItem = _profileItems.FirstOrDefault(i => i.IsDefault());
        //Add default profile to list if it does not already exist
        if (_defaultProfileItem is null)
        {
            _defaultProfileItem = new ProfileItem { Id = GetProfileId(0), Label = "GridViewDefaultProfileName".Translate() };
            _profileItems.Insert(0, _defaultProfileItem);
        }
        // Sort the profile list
        _profileItems.Sort();
        // Load the last profile
        ProfileItem lastProfileItem = _allowProfiles
            ? App.LocalStorage.GetItem<ProfileItem>(_lastSelectedProfileKey)
            : _defaultProfileItem;
        await LoadProfileAsync(lastProfileItem ?? _defaultProfileItem, false);
    }

    /// <summary>
    /// Load profile
    /// </summary>
    /// <param name="profileItem"></param>
    /// <param name="refresh"></param>
    /// <returns></returns>
    private async Task LoadProfileAsync(ProfileItem profileItem, bool refresh = true)
    {
        // Save last used profile
        App.LocalStorage.SetItem(_lastSelectedProfileKey, profileItem);
        // Keep the selected profile like the current profile
        _currentProfileItem = profileItem;
        // Load the profile
        CurrentProfile = await GetProfileAsync(profileItem) ?? profileItem.GetNewProfile();
        // Apply profile data
        LoadCurrentProfileColumnsState();
         // Load the filter summaries
         await FilterStateChangeAsync();
        // Load the grouping
        List<GridViewGroupProfile> profileGroups = CurrentProfile?.Groups;
        if (profileGroups is null || !Features.HasFlag(GridFeature.Group))
        {
            // Load the default grouping
            LoadDefaultGrouping();
        }
        else
        {
            // Load the profile grouping
            GroupByColumnKeyList.Clear();
            int groupLevel = 0;
            foreach (GridViewGroupProfile group in profileGroups)
            {
                foreach (string field in group.Columns)
                {
                    AddGroupField(groupLevel, field.Trim());
                }
                groupLevel++;
            }
        }
        // Page size
        if (Features.HasFlag(GridFeature.Paging))
        {
            PaginationState.PageSize = CurrentProfile.PageSize > 0 ? CurrentProfile.PageSize : DefaultPageSize;
            // Page size check            
            if (PaginationState.PageSize < 1)
            {
                PaginationState.PageSize = PaginationSizeSelector[0];
            }
        }
        else
        {
            // No pagination
            PaginationState.PageSize = 0;
        }
        // Reload the rows
        RebuildRows = true;
        // Resize the colunm width.
        await ResizeColumnsWidthsAsync();
        // Indicate the columns layout has changed
        ColumnsLayoutState = Guid.NewGuid();
        // Rebuild columns style
        if (refresh)
        {
            // Rebuild columns style
            await BuildGridViewStyleAsync();
            await MoveToPageAsync(1);
            await LoadDataAsync(true);
        }
    }

    /// <summary>
    /// Laod current profile columns parameters
    /// </summary>
    private void LoadCurrentProfileColumnsState()
    {
        bool hasFilter = DefaultShowFilters;
        foreach (GridColumnState state in ColumnList)
        {
            GridViewColumnProfile columnProfile = CurrentProfile?.Columns.FirstOrDefault(p => p.UniqueKey == state.UniqueKey);
            state.LoadProfile(columnProfile, Features);
            if (state.Filter is not null)
            {
                hasFilter = true;
            }
        }
        // Restore the filter state            
        ShowFilters = hasFilter;
    }

    /// <summary>
    /// Load the default grouping.
    /// </summary>
    private void LoadDefaultGrouping()
    {
        GroupByColumnKeyList.Clear();
        if (!string.IsNullOrEmpty(GroupBy))
        {
            int groupLevel = 0;
            foreach (string group in GroupBy.Split(_levelSeparator, StringSplitOptions.RemoveEmptyEntries))
            {
                foreach (string field in group.Split(_groupSeparator, StringSplitOptions.RemoveEmptyEntries))
                {
                    AddGroupField(groupLevel, field.Trim());
                }
                groupLevel++;
            }
        }
    }

    /// <summary>
    /// Load the default profile (After removing another selected profile).
    /// </summary>
    private Task LoadDefaultProfileAsync()
    {
        return LoadProfileAsync(_defaultProfileItem, true);
    }

    /// <summary>
    /// Initialize columns parameters with default values
    /// </summary>        
    /// <returns></returns>
    private async Task ResetColumnsDefinitionAsync()
    {
        int order = 1;
        foreach (GridColumnState state in ColumnList)
        {
            state.Order = order;
            state.LoadDefaultProfile();
            order++;
        }
        // Group definition
        LoadDefaultGrouping();
        RebuildRows = true;
        await ResizeColumnsWidthsAsync();
        // Reset filter            
        foreach (GridColumnState state in ColumnList)
        {
            await state.ResetFilterAsync(false);
        }
        await FilterStateChangeAsync();
        await UpdateCurrentProfileAsync();
        await MoveToPageAsync(1);
        await LoadDataAsync(true);
    }

    /// <summary>
    /// Update current profile
    /// </summary>
    private Task UpdateCurrentProfileAsync()
    {
        return UpdateProfileAsync(_currentProfileItem);
    }

    /// <summary>
    /// Call update current profil from JS
    /// </summary>
    /// <returns></returns>
    [JSInvokable]
    public Task UpdateCurrentProfileJsAsync()
    {
        return UpdateCurrentProfileAsync();
    }

    /// <summary>
    /// The profile have been updated from the IHM.
    /// </summary>
    /// <param name="profileItem">profile to save</param>        
    /// <param name="fromButton">Indicate if the method was called by button</param>
    /// <returns></returns>        
    private async Task UpdateProfileAsync(ProfileItem profileItem, bool fromButton = false)
    {
        GridViewProfile gridViewProfile = new(profileItem)
        {
            Columns = ColumnList.Select(c => c.SaveToProfile(Features, profileItem.IsSharedProfile)).ToList(),
            Groups = GroupByColumnKeyList.Values.Select(g => new GridViewGroupProfile(g)).ToList(),
            PageSize = PaginationState.PageSize
        };
        if((gridViewProfile.IsSharedProfile && fromButton) ||
            (!gridViewProfile.IsSharedProfile && 
            (ProfileSaveMode == GridViewProfileSave.Auto || fromButton)))
        {
            await SaveProfileAsync(gridViewProfile);
        }
        // Indicate the columns layout has changed
        ColumnsLayoutState = Guid.NewGuid();
    }

    /// <summary>
    /// Add new profile
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="isSharedProfile"></param>
    /// <returns></returns>
    private Task AddNewProfileAsync(string profileName, bool isSharedProfile)
    {
        ProfileItem profileItem = new()
        {
            Id = GetProfileId(GetAvailableProfileId()),
            Label = profileName,
            IsSharedProfile = isSharedProfile
        };
        _profileItems.Add(profileItem);
        _currentProfileItem = profileItem;
        return UpdateProfileAsync(profileItem, isSharedProfile || ProfileSaveMode == GridViewProfileSave.Button);
    }

    /// <summary>
    /// Remove profile from IHM and load default if necessary
    /// </summary>
    private async Task RemoveAndLoadDefaultProfileIfNecessaryAsync(ProfileItem profileItem)
    {
        await RemoveProfileAsync(profileItem);
        if (_currentProfileItem.Id == profileItem.Id)
        {
            await LoadDefaultProfileAsync();
        }
    }

    /// <summary>
    /// Save profile 
    /// </summary>
    /// <param name="profile">Profile to save</param>        
    /// <returns></returns>
    private async Task SaveProfileAsync(GridViewProfile profile)
    {
        if (_allowProfiles)
        {
            if ((profile.IsSharedProfile && SharedProfileManagement)
                || (!profile.IsSharedProfile && StorageMode == GridViewProfileStorage.Remote))
            {
                _profileCancellationTokenSource?.Cancel();
                _profileCancellationTokenSource = new CancellationTokenSource();
                await JsonHttp.TryPostAsync("LgGridViewProfile", profile, _profileCancellationTokenSource.Token);
            }
            else if (!profile.IsSharedProfile)
            {
                App.LocalStorage.SetItem(GetProfileLocalStorageKey(profile.Id), profile);
                App.LocalStorage.SetItem(_profilesListKey, _profileItems.Where(x => !x.IsSharedProfile).ToList());
            }
            CurrentProfile = profile;
        }
    }

    /// <summary>
    /// Remove profile 
    /// </summary>
    /// <param name="profileItem"></param>
    /// <returns></returns>
    private async Task RemoveProfileAsync(ProfileItem profileItem)
    {
        if ((profileItem.IsSharedProfile && SharedProfileManagement)
                || (!profileItem.IsSharedProfile && StorageMode == GridViewProfileStorage.Remote))
        {
            _profileCancellationTokenSource?.Cancel();
            _profileCancellationTokenSource = new CancellationTokenSource();
            // We are using Uri.EscapeDataString and not HtmlUtility.UrlEncode because "+" for spaces aren't reconize in MVC routes
            await JsonHttp.TryDeleteAsync($"LgGridViewProfile/{Uri.UnescapeDataString(profileItem.Id)}",
                _profileCancellationTokenSource.Token);
        }
        else
        {
            App.LocalStorage.RemoveItem(GetProfileLocalStorageKey(profileItem.Id));
        }
        _profileItems.Remove(profileItem);
        if (StorageMode == GridViewProfileStorage.Local)
        {
            App.LocalStorage.SetItem(_profilesListKey, _profileItems.Where(x => !x.IsSharedProfile).ToList());
        }
    }

    /// <summary>
    /// Get gridview profile 
    /// </summary>
    /// <param name="profileItem"></param>
    /// <returns></returns>
    private async Task<GridViewProfile> GetProfileAsync(ProfileItem profileItem)
    {
        if (profileItem.IsSharedProfile || StorageMode == GridViewProfileStorage.Remote)
        {
            bool shared = profileItem.IsSharedProfile && HasAccessToSharedProfile;
            _profileCancellationTokenSource?.Cancel();
            _profileCancellationTokenSource = new CancellationTokenSource();
            try
            {
                // We are using Uri.EscapeDataString and not HtmlUtility.UrlEncode because "+" for spaces aren't reconize in MVC routes                
                return await JsonHttp.TryGetAsync<GridViewProfile>($"LgGridViewProfile/Profile/{Uri.EscapeDataString(profileItem.Id)}/{shared}",
                    _profileCancellationTokenSource.Token);
            }
            catch (Exception)
            {
                // WebAssembly.JsException : AbortError : The user abort the request
                // System.Threading.Tasks.TaskCanceledException: A task was canceled
                return null;
            }
        }
        else
        {
            return App.LocalStorage.GetItem<GridViewProfile>(GetProfileLocalStorageKey(profileItem.Id));
        }
    }

    #endregion

    /// <summary>
    /// Get the profile id from an index.
    /// </summary>
    /// <param name="index">New index.</param>
    /// <returns>The new profile Id.</returns>
    private string GetProfileId(int index)
    {
        return string.Format(_profileIdPattern, index);
    }

    /// <summary>
    /// Get the key to save the profile in the local storage.
    /// </summary>
    /// <param name="profileId">The profile Id.</param>
    /// <returns>The key to use to save the profile to the local storage.</returns>
    private string GetProfileLocalStorageKey(string profileId)
    {
        return string.Format(_profileKeyPattern, profileId);
    }

    /// <summary>
    /// Get available profile Id
    /// </summary>
    /// <returns></returns>
    private int GetAvailableProfileId()
    {
        int index = 1;
        while (_profileItems.Any(x => Equals(x.Id, GetProfileId(index))))
        {
            index++;
        }
        return index;
    }

    #region Modal methods

    /// <summary>
    /// Open add new profile pop-up
    /// </summary>
    /// <returns></returns>
    private Task OpenAddNewProfilePopupAsync()
    {
        return _lgModalProfiles.ShowAsync();

    }

    /// <summary>
    /// On save profile
    /// </summary>
    /// <returns></returns>
    private async Task OnSaveProfilesAsync()
    {
        _formProfile.Validator.ClearErrors();
        if (string.IsNullOrEmpty(_addProfileModel.ProfileName))
        {
            _formProfile.Validator.AddError("ProfileName", "GridViewErroAddProfileNameCannotBeNullOrEmpty".Translate());
            _formProfile.Validator.ThrowPendingErrors();
            return;
        }

        if (_profileItems.Any(x => _addProfileModel.ProfileName.Equals(x.Label)))
        {
            _formProfile.Validator.AddError("ProfileName", "GridViewErroAddProfileNameAlreadyExists".Translate());
            _formProfile.Validator.ThrowPendingErrors();
            return;
        }

        await _lgModalProfiles.CloseAsync();
        await AddNewProfileAsync(_addProfileModel.ProfileName, _addProfileModel.IsSharedProfile);
        _addProfileModel.ProfileName = string.Empty;
        _addProfileModel.IsSharedProfile = false;
    }

    #endregion Modal methods

    /// <summary>
    /// Return true if can user remove profile
    /// </summary>
    /// <param name="profileItem"></param>
    /// <returns></returns>
    private bool CanRemoveProfile(ProfileItem profileItem)
    {
        if (profileItem.IsDefault())
        {
            return false;
        }

        return !profileItem.IsSharedProfile || SharedProfileManagement;
    }

    /// <summary>
    /// Return true if can user save profile
    /// </summary>
    /// <param name="gridViewProfile"></param>
    /// <returns></returns>
    private bool CanSaveProfile(ProfileItem gridViewProfile)
    {
        return gridViewProfile.IsSharedProfile ? SharedProfileManagement : ProfileSaveMode == GridViewProfileSave.Button;
    }

    #endregion methods

    #region private classes

    /// <summary>
    /// Add profile model
    /// </summary>
    private class AddProfileModel
    {
        /// <summary>
        /// Profile name
        /// </summary>
        public string ProfileName
        {
            get; set;
        }

        /// <summary>
        /// Is shared profile
        /// </summary>
        public bool IsSharedProfile
        {
            get; set;
        }
    }

    #endregion

}
