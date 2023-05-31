namespace Lagoon.UI.Components;

/// <summary>
/// Component used to add a step in <see cref=" LgTimelineContainer"/>
/// </summary>
public partial class LgTimelineStep : LgComponentBase
	{

		#region Cascading Parameters

		/// <summary>
		/// Parent container
		/// </summary>
		[CascadingParameter]
		public LgTimelineContainer TimelineContainer { get; set; }

		#endregion

		#region Parameters

		/// <summary>
		/// Get or set the id
		/// </summary>
		[Parameter]
		public string Id { get; set; }

		/// <summary>
		/// Get or set a css class to apply
		/// </summary>
		[Parameter]
		public string CssClass { get; set; }

		/// <summary>
		/// Get or set the top label
		/// </summary>
		[Parameter]
		public string TopLabel { get; set; }

		/// <summary>
		/// Get or set the bottom label
		/// </summary>
		[Parameter]
		public string BottomLabel { get; set; }

		/// <summary>
		/// Get or set the clickable state
		/// </summary>
		[Parameter]
		public bool IsClickable { get; set; }

		/// <summary>
		/// Get or set the validation state
		/// </summary>
		[Parameter]
		public bool IsValidated { get; set; }

		/// <summary>
		/// Get or set the custom renderer for the label above the "Step"
		/// </summary>
		[Parameter]
		public RenderFragment TopLabelContent { get; set; }

		/// <summary>
		/// Get or set the custom renderer for the label below the "Step"
		/// </summary>
		[Parameter]
		public RenderFragment BottomLabelContent { get; set; }

		/// <summary>
		/// Get or set the custom render for the step
		/// </summary>
		[Parameter]
		public RenderFragment StepContent { get; set; }

		/// <summary>
		/// Get or set the childs 
		/// </summary>
		[Parameter]
		public RenderFragment ChildContent { get; set; }

		#endregion

		#region Initialisation

		/// <inheritdoc />
		protected override void OnInitialized()
		{
			base.OnInitialized();
			if (TimelineContainer is null)
			{
				throw new Exception($"{nameof(LgTimelineStep)} must be wrapped inside a {(nameof(LgTimelineContainer))} component.");
			}
			if (Id is null)
			{
				throw new Exception($"All {nameof(LgTimelineStep)} must have an {nameof(Id)}");
			}
			TimelineContainer.Add(this);
		}

		/// <summary>
		/// Remove this item from parent container
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			TimelineContainer.Remove(this);
		}

		#endregion

		#region Internal methods

		/// <summary>
		/// Shortcut for <see cref="LgTimelineContainer.IsFirstStep(LgTimelineStep)"/>
		/// </summary>
		private bool IsFirstStep()
		{
			return TimelineContainer.IsFirstStep(this);
		}

		/// <summary>
		/// Shortcut for <see cref="LgTimelineContainer.IsLastStep(LgTimelineStep)"/>
		/// </summary>
		private bool IsLastStep()
		{
			return TimelineContainer.IsLastStep(this);
		}

		/// <summary>
		/// Shortcut for <see cref="LgTimelineContainer.GetStepPosition(LgTimelineStep)"/>
		/// </summary>
		private int GetStepPosition()
		{
			return TimelineContainer.GetStepPosition(this);
		}

		/// <summary>
		/// Shortcut for <see cref="LgTimelineContainer.GetStepNumber(LgTimelineStep)"/>
		/// </summary>
		private int GetStepNumber()
		{
			return TimelineContainer.GetStepNumber(this);
		}

		/// <summary>
		/// Shortcut for <see cref="LgTimelineContainer.IsPrevisousStepValidated(LgTimelineStep)"/>
		/// </summary>
		private bool IsPreviousStepValidated()
		{
			return TimelineContainer.IsPrevisousStepValidated(this);
		}

		/// <summary>
		/// Shortcut for <see cref="LgTimelineContainer.IsNextStepValidated(LgTimelineStep)"/>
		/// </summary>
		private bool IsNextStepValidated()
		{
			return TimelineContainer.IsNextStepValidated(this);
		}

		/// <summary>
		/// Shortcut for <see cref="LgTimelineContainer.OnChildClickAsync(LgTimelineStep)"/>
		/// </summary>
		private Task OnChildClickAsync()
		{
        return TimelineContainer.OnChildClickAsync(this);
    }

		#endregion

	}

	#region Helpers

	/// <summary>
	/// Helpers
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Create a string with <paramref name="value"/> content repeated <paramref name="count"/> time
		/// </summary>
		/// <param name="value"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static string Repeat(this string value, int count)
		{
			if (count <= 0)
			{
				count = 1;
			}
			return new StringBuilder(value.Length * count).Insert(0, value, count).ToString();
		}

	}

	#endregion
