namespace Lagoon.UI.Components;

/// <summary>
/// The cascading error handler. Use the "ErrorContent" defined in "App.razor" as default content.
/// </summary>
public class LgErrorBoundary : ErrorBoundaryBase
{

    #region cascading parameters

    /// <summary>
    /// The "LgApp" instance.
    /// </summary>
    [CascadingParameter]
    protected LgApp App { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Invoked by the base class when an error is being handled. The default implementation
    /// logs the error.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> being handled.</param>
    protected override Task OnErrorAsync(Exception exception)
    {
        App.App.TraceException(exception);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        BuildRenderTree(builder, CurrentException);
    }

    /// <inheritdoc />
    protected void BuildRenderTree(RenderTreeBuilder builder, Exception exception)
    {
        if (exception is null)
        {
            builder.AddContent(0, ChildContent);
        }
        else if (ErrorContent is not null)
        {
            builder.AddContent(1, ErrorContent(exception));
        }
        else if (App.ErrorContent is not null)
        {
            builder.AddContent(2, App.ErrorContent(exception));
        }
        else
        {
            builder.OpenComponent<LgBigMessage>(3);
            builder.AddAttribute(4, nameof(LgBigMessage.IconName), IconNames.Error);
            builder.AddAttribute(5, nameof(LgBigMessage.Title), App.App.GetContactAdminMessage(exception, false));
            if (App.App.ApplicationInformation.IsDevelopment)
            {
                builder.AddAttribute(6, nameof(LgBigMessage.ChildContent), RenderStackTrace(exception.ToString()));
            }
            builder.CloseComponent();
        }
    }

    /// <summary>
    /// Render the formated stack trace.
    /// </summary>
    /// <param name="stackTrace">The stack trace.</param>
    /// <param name="highlightStyle">The style of the lines with a known line number.</param>
    /// <returns>The method to render the formated stack trace.</returns>
    public static RenderFragment RenderStackTrace(string stackTrace, string highlightStyle = "color: var(--red)")
    {
        return (builder) =>
        {
            string line;
            string style;
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "stackTrace");
            int seq = 2;
            if (!string.IsNullOrEmpty(stackTrace))
            {
                using (StringReader sr = new(stackTrace))
                {
                    while ((line = sr.ReadLine()) is not null)
                    {
                        builder.OpenElement(seq++, "div");
                        style = line.Contains(":line ") ? highlightStyle : null;
                        builder.AddAttribute(seq++, "style", style);
                        builder.AddContent(seq++, line);
                        builder.CloseElement();
                    }
                }
            }
            builder.CloseElement(); // div - 0
        };
    }

    #endregion

}
