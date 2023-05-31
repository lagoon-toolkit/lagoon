namespace Lagoon.UI.Helpers;

/// <summary>
/// A class to handle "Command" events and raise the StateHasChanged only if the command has been handled by the delegate.
/// </summary>
public static class CommandHandler
{

    #region static fields

    private static FieldInfo _delegateField;
    private static MethodInfo _invokeMethod;
    private static FieldInfo _receiverField;

    #endregion

    #region static methods

    /// <summary>
    /// Invoke the delegate of the EventCallback if any, and raise the StateAsChanged Method
    /// on the component linked to the EventCallback, only if the args.Handled is <c>true.</c>
    /// </summary>
    /// <param name="onCommand">The EventCallBack for handle the command.</param>
    /// <param name="args">The command informations.</param>
    public static Task InvokeAsync(EventCallback<CommandEventArgs> onCommand, CommandEventArgs args)
    {
        return onCommand.HasDelegate ? InvokeAndUpdateStateAsync(onCommand, args) : Task.CompletedTask;
    }

    /// <summary>
    /// Invoke the delegate of the EventCallback if any, and raise the StateAsChanged Method
    /// on the component linked to the EventCallback, only if the args.Handled is <c>true.</c>
    /// </summary>
    /// <param name="onCommand">The EventCallBack for handle the command.</param>
    /// <param name="args">The command informations.</param>
    private static async Task InvokeAndUpdateStateAsync(EventCallback<CommandEventArgs> onCommand, CommandEventArgs args)
    {
        if (_delegateField is null)
        {
            // Get an acces to the "Delegate" and  "Receiver" private fields from "EventCallback<CommandEventArgs>"
            Type type = typeof(EventCallback<CommandEventArgs>);
            _delegateField = type.GetField("Delegate", BindingFlags.Instance | BindingFlags.NonPublic);
            _receiverField = type.GetField("Receiver", BindingFlags.Instance | BindingFlags.NonPublic);
            // Get an access to the "InvokeAsync" private static method from the "EventCallbackWorkItem"
            Type genericType = Type.MakeGenericMethodParameter(0);
            MethodInfo genericMethod = typeof(EventCallbackWorkItem).GetMethod("InvokeAsync", 1, BindingFlags.Static | BindingFlags.NonPublic,
            null, CallingConventions.Standard, new Type[] { typeof(MulticastDelegate), genericType }, null);
            _invokeMethod = genericMethod.MakeGenericMethod(typeof(CommandEventArgs));
        }
        // Get the method to execute
        MulticastDelegate @delegate = (MulticastDelegate)_delegateField?.GetValue(onCommand);
        if (@delegate is null)
        {
            // The previous call to "onCommand.HasDelegate" indicate that this value cannot be null
            throw new EntryPointNotFoundException("The \"Delegate\" field was not found in EventCallback<CommandEventArgs>.");
        }
        // Execute the method
        await (Task)_invokeMethod.Invoke(null, new object[] { @delegate, args });
        // If the method handled the command, raise the StateHasChanged
        if (args.Handled)
        {
            IHandleEvent receiver = (IHandleEvent)_receiverField?.GetValue(onCommand);
            if (receiver is not null)
            {
                await receiver.HandleEventAsync(EventCallbackWorkItem.Empty, null);
            }
        }
    }
    #endregion
}