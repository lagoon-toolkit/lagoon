using Microsoft.AspNetCore.Components.Authorization;

namespace Lagoon.UI.Components;


/// <summary>
/// Component used to define the 'PolicyVisible' and 'PolicyEdit' properties and apply the defined policies to children 
/// </summary>
public partial class LgAuthorizeView : ComponentBase, ILgComponentPolicies
{

    #region fields

    // Indicate the state of component after policies applied
    private PolicyState _policyState = new();

    #endregion

    #region dependencies injection

    /// <summary>
    /// Used to check policy for the connected user
    /// </summary>
    [Inject]
    public PolicyService PolicyService { get; set; }

    #endregion

    #region cascading parameters

    [CascadingParameter]
    private Task<AuthenticationState> AuthenticationState { get; set; }

    #endregion

    #region render fragments

    /// <summary>
    /// The content that will be displayed if the user is authorized.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// The content that will be displayed if the user is not authorized.
    /// </summary>
    [Parameter]
    public RenderFragment<AuthenticationState> NotAuthorized { get; set; }

    /// <summary>
    /// The content that will be displayed if the user is authorized.
    /// If you specify a value for this parameter, do not also specify a value for <see cref="ChildContent"/>.
    /// </summary>
    [Parameter]
    public RenderFragment<AuthenticationState> Authorized { get; set; }

    /// <summary>
    /// The content that will be displayed while asynchronous authorization is in progress.
    /// </summary>
    [Parameter]
    public RenderFragment Authorizing { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets the DIV CSS class
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Indicate if the LgAuthorizeView should encapsulate it's content in a div
    /// </summary>
    /// <value></value>
    [Parameter]
    public bool? ShowDiv { get; set; }

    #endregion

    #region ILgComponentPolicies implementation

    /// <summary>
    /// Get or set the policy wich allow to show a control
    /// </summary>
    /// <value>Policy name</value>
    [Parameter]
    public string PolicyVisible { get; set; }

    /// <summary>
    /// Get or set the policy wich allow to edit a control
    /// </summary>
    /// <value>Policy name</value>
    [Parameter]
    public string PolicyEdit { get; set; }

    /// <summary>
    /// To retrieve policy defined by <see cref="ParentPolicy" /> 
    /// </summary>
    /// <value>CascadingPolicy if defined, null otherwise</value>
    [CascadingParameter]
    public LgAuthorizeView ParentPolicy { get; set; }

    /// <summary>
    /// Get or set a value indicating if annonymous user is allowed or not. Default is <c>false</c>
    /// </summary>
    /// <remarks>Should only be used to overload a parent LgAuthorizeView.</remarks>
    [Parameter]
    public bool AllowAnnonymous { get; set; }

    #endregion

    #region Internal properties

    /// <summary>
    /// Get or set the 'Editable' state for the current CascadingPolicy component
    /// </summary>
    internal bool IsEditable => _policyState.Editable;

    /// <summary>
    /// Get or set the 'Visible' state for the current CascadingPolicy component
    /// </summary>
    internal bool IsVisible => _policyState.Visible;

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        // We allow 'ChildContent' for convenience in basic cases, and 'Authorized' for symmetry
        // with 'NotAuthorized' in other cases. Besides naming, they are equivalent. To avoid
        // confusion, explicitly prevent the case where both are supplied.
        if (ChildContent != null && Authorized != null)
        {
            throw new InvalidOperationException($"Do not specify both '{nameof(Authorized)}' and '{nameof(ChildContent)}'.");
        }
    }

    /// <summary>
    /// Update state on parameter change
    /// </summary>
    protected override Task OnParametersSetAsync()
    {
        return ((ILgComponentPolicies)this).UpdatePolicyStateAsync(AuthenticationState, _policyState, !AllowAnnonymous);
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (AllowAnnonymous)
        {
            if (Authorized is not null)
            {
                throw new InvalidOperationException("Don't use 'Authorized' RenderFragment with 'AllowAnnonymous'");
            }
            builder.AddContent(0, AuthorizedContent());
        }
        else
        {
            // We're using the same sequence number for each of the content items here
            // so that we can update existing instances if they are the same shape
            if (!_policyState.IsAuthenticated)
            {
                builder.AddContent(0, Authorizing);
            }
            else if (_policyState.Visible)
            {
                builder.AddContent(0, AuthorizedContent());
            }
            else
            {
                builder.AddContent(0, NotAuthorized?.Invoke(_policyState.AuthenticationState));
            }
        }
    }

    /// <summary>
    /// Get the authorized content.
    /// </summary>
    /// <returns></returns>
    private RenderFragment AuthorizedContent()
    {
        return builder =>
        {
            RenderFragment authorized = ChildContent ?? Authorized?.Invoke(_policyState.AuthenticationState);
            if (ShowDiv ?? !string.IsNullOrEmpty(CssClass))
            {
                builder.OpenElement(10, "div");
                builder.AddAttribute(11, "class", CssClass);
                builder.AddContent(12, builder =>
                 {
                     builder.AddCascadingValueComponent(20, this, authorized, true);
                 });
                builder.CloseElement();
            }
            else
            {
                builder.AddCascadingValueComponent(10, this, authorized, true);
            }
        };
    }

    #endregion

}
