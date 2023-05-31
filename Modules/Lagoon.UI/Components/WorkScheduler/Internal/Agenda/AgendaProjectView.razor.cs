namespace Lagoon.UI.Components.WorkScheduler.Internal;


/// <summary>
/// Component used to display all task in the scheduler area
/// </summary>
/// <typeparam name="TItem">Item type</typeparam>
	public partial class AgendaProjectView<TItem>: ComponentBase where TItem : IWorkSchedulerData
	{

		/// <summary>
		/// Get or set the parent component
		/// </summary>
		[CascadingParameter]
		private LgWorkScheduler<TItem> WorkScheduler { get; set; }

	}
