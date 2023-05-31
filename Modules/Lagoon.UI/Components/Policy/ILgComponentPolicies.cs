using Microsoft.AspNetCore.Components.Authorization;

namespace Lagoon.UI.Components;


/// <summary>
/// Interface to use when a component can have a different visual behavior (visible, editable) depending on the roles of the user
/// </summary>
public interface ILgComponentPolicies
{

    #region Properties

    /// <summary>
    /// Service to help policy check for the connected user
    /// </summary>
    /// <remark>
    /// This property should be injected via DI (width [Inject] annotation).
    /// PolicyService is added by Lagoon as Scoped Service
    /// </remark>
    PolicyService PolicyService { get; set; }

    /// <summary>
    /// Get or set the applicable policy to view the control
    /// </summary>
    /// <remark>
    /// This property should be a [Parameter] on component wich implement this interface
    /// </remark>
    string PolicyVisible { get; set; }

    /// <summary>
    /// Get or set the applicable policy to edit the control
    /// </summary>
    /// <remark>
    /// This property should be a [Parameter] on component wich implement this interface
    /// </remark>
    string PolicyEdit { get; set; }

    /// <summary>
    /// To retrieve policy defined by <see cref="ParentPolicy" /> 
    /// MUST be a [CascadingParameter] 
    /// </summary>
    /// <value>CascadingPolicy if defined, null otherwise</value>
    LgAuthorizeView ParentPolicy { get; set; }

    #endregion

    #region Pré-implemented methods 

    /// <summary>
    /// Get if the current component can be visible and editable.
    /// </summary>
    /// <param name="authenticationStateTask">Task returning informations about the user.</param>
    /// <param name="state">Result of the policy evaluation.</param>
    /// <param name="authorizeView">Indicate the state is updated for an authorize view.</param>
    /// <returns>Get if the current component can be visible or editable.</returns>
    internal async Task UpdatePolicyStateAsync(Task<AuthenticationState> authenticationStateTask, PolicyState state, bool authorizeView = false)
    {            
        state.IsAuthenticated = false;
        if(authenticationStateTask is not null)
        {
            state.AuthenticationState = await authenticationStateTask;
            state.IsAuthenticated = true;
        }            
        if (!string.IsNullOrEmpty(PolicyVisible))
        {
            state.Visible = PolicyVisible == "*" || await PolicyService.IsInPolicyAsync(state.GetAuthentificationUser(), PolicyVisible);
        }
        else if (ParentPolicy is null)
        {
            state.Visible = !authorizeView || (state.GetAuthentificationUser()?.Identity?.IsAuthenticated ?? false);
        }
        else
        {
            state.Visible = ParentPolicy.IsVisible;
        }
        if (state.Visible)
        {
            if (!string.IsNullOrEmpty(PolicyEdit))
            {
                state.Editable = PolicyEdit == "*" || await PolicyService.IsInPolicyAsync(state.GetAuthentificationUser(), PolicyEdit);
            }
            else if (ParentPolicy is null)
            {
                state.Editable = true;
            }
            else
            {
                state.Editable = ParentPolicy.IsEditable;
            }
        }           
    }

    #endregion

    #region Obsolete methods

    /// <summary>
    /// Pré-implemented method to check if IsEditable = true or false according to the current PolicyEdit or ParentPolicy
    /// </summary>
    /// <returns>True if editable, false otherwise</returns>
    [Obsolete("Use the \"IsInPolicyAsync\" method.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    async Task<bool> IsEditableAsync()
    {
        if (!string.IsNullOrEmpty(PolicyEdit))
        {
            return await PolicyService.IsInPolicyAsync(PolicyEdit);
        }
        else if (ParentPolicy != null)
        {
            return ParentPolicy.IsEditable;
        }

        return true;
    }

    /// <summary>
    /// Pré-implemented method to check if IsVisible = true or false according to the current PolicyVisible or ParentPolicy
    /// </summary>
    /// <returns>True if visible, false otherwise</returns>
    [Obsolete("Use the \"IsInPolicyAsync\" method.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    async Task<bool> IsVisibleAsync()
    {
        if (!string.IsNullOrEmpty(PolicyVisible))
        {
            return await PolicyService.IsInPolicyAsync(PolicyVisible);
        }
        else if (ParentPolicy != null)
        {
            return ParentPolicy.IsVisible;
        }

        return true;
    }

    #endregion

}
