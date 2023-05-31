namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
	public partial class PageTimeline : DemoPage
	{

		#region URL, Icon and title for this page

		/// <summary>
		/// Route to reach the page.
		/// </summary>
		private const string ROUTE = "PageTimeline";

		/// <summary>
		/// Gets the URL for this page.
		/// </summary>
		/// <returns>The URL for this page.</returns>
		public static LgPageLink Link()
		{
			return new LgPageLink(ROUTE, "Timeline", @IconNames.All.Window);
		}

		#endregion

		#region Initialization

		/// <summary>
		/// Method to load data for the current page.
		/// </summary>
		/// <param name="e"></param>
		protected override async Task OnLoadAsync(PageLoadEventArgs e)
		{
			try
			{
				await base.OnLoadAsync(e);
				// Initialize the title and the icon of the page
				await SetTitleAsync(Link());
			}
			catch (Exception ex)
			{
				ShowException(ex);
			}
		}

		#endregion

		#region Sample 1 & 2 - Simple Timeline

		// Collection of steps for sample 1
		private List<StepDescription> _steps = new()
		{
			{ new StepDescription() { Id = "Draft", Label="Draft", Validated = true, Message="Create by Admin", IconName = IconNames.All.GearFill } },
			{ new StepDescription() { Id = "Init", Label="Initialisation", Clickable = true, IconName = IconNames.All.GearFill } },
			{ new StepDescription() { Id = "Review", Label="Plannification", IconName = IconNames.All.Calendar2MonthFill } },
			{ new StepDescription() { Id = "Send", Label="En cours", IconName = IconNames.All.Cpu} },
			{ new StepDescription() { Id = "Close", Label="Terminé", IconName = IconNames.All.DoorClosedFill } }
		};

		/// <summary>
		/// Step clicked: Validate the current step and activate the next one
		/// </summary>
		/// <param name="id"></param>
		private void StepClickSample1(string id)
		{
			// Validate the current step
			var step = _steps.Where(x => x.Id == id).First();
			step.Clickable = false;
			step.Validated = true;
			step.Message = "Validated by Admin \n " + DateTime.Now.ToLongTimeString();
			// Make the next step clickable (if exist)
			var nextStep = _steps.IndexOf(step) + 1;
			if (nextStep < _steps.Count)
			{
				_steps.ElementAt(nextStep).Clickable = true;
			}
		}

		/// <summary>
		/// Reset steps
		/// </summary>
		private void ResetSample1()
		{
			_steps = new()
			{
				{ new StepDescription() { Id = "Draft", Label="Draft", Validated = true, Message="Create by Admin", IconName = IconNames.All.GearFill } },
				{ new StepDescription() { Id = "Init", Label="Initialisation", Clickable = true, IconName = IconNames.All.GearFill } },
				{ new StepDescription() { Id = "Review", Label="Plannification", IconName = IconNames.All.Calendar2MonthFill } },
				{ new StepDescription() { Id = "Send", Label="En cours", IconName = IconNames.All.Cpu} },
				{ new StepDescription() { Id = "Close", Label="Terminé", IconName = IconNames.All.DoorClosedFill } }
			};
		}

		/// <summary>
		/// Insert a step
		/// </summary>
		private void AddStepSample1()
		{
			_steps.Insert(2, new StepDescription() { Id = "Validate", Label = "Validation", IconName = IconNames.Save });
		}

		/// <summary>
		/// Go to next step
		/// </summary>
		private void ValidateCurrentStep()
		{
			var step = _steps.Where(x => x.Clickable).FirstOrDefault();
			if (step != null)
			{
				StepClickSample1(step.Id);
			}
		}

		#endregion

		#region Sample 3

		// Collection of steps for sample 1
		private List<StepDescription> _stepsSample2 = new()
		{
			{ new StepDescription() { Id = "Init", Label="Initialisation", Clickable = true } },
			{ new StepDescription() { Id = "Review", Label="Déclaration" } },
			{ new StepDescription() { Id = "Send", Label="Caractérisation"} },
			{ new StepDescription() { Id = "Close", Label="Vérification et validation" } },
			{ new StepDescription() { Id = "Cancel", Label="Annulé", Clickable = true, Css = "redStep" } }
		};

		/// <summary>
		/// Step clicked
		/// </summary>
		/// <param name="id"></param>
		private void StepClickSample2(string id)
		{
			var step = _stepsSample2.Where(x => x.Id == id).First();
			step.Clickable = false;
			if (id == "Cancel")
			{
				// Make all step unclickable and 'disable' step wich are not validated
				step.Validated = true;
				step.Message = "Cancelled by Admin \n " + DateTime.Now.ToLongTimeString();
				foreach (var s in _stepsSample2.Where(s => s.Id != id))
				{
					s.Clickable = false;
					if (!s.Validated)
					{
						s.Css = "canceledStep";
					}
				}
			}
			else
			{
				// Validate the current step
				step.Validated = true;
				step.Message = "Validated by Admin \n " + DateTime.Now.ToLongTimeString();
				// Make the next step clickable (if exist)
				var nextStep = _stepsSample2.IndexOf(step) + 1;
				if (nextStep < _stepsSample2.Count)
				{
					_stepsSample2.ElementAt(nextStep).Clickable = true;
				}
				if (id == "Close")
				{
					var cancelStep = _stepsSample2.Last();
					cancelStep.Clickable = false;
					cancelStep.Css = "canceledStep";
				}
			}
		}

		private void ResetSample3()
		{
			_stepsSample2 = new()
			{
				{ new StepDescription() { Id = "Init", Label="Initialisation", Clickable = true } },
				{ new StepDescription() { Id = "Review", Label="Déclaration" } },
				{ new StepDescription() { Id = "Send", Label="Caractérisation"} },
				{ new StepDescription() { Id = "Close", Label="Vérification et validation" } },
				{ new StepDescription() { Id = "Cancel", Label="Annulé", Clickable = true, Css = "redStep" } }
			};
		}

		#endregion

		#region Sample 4

		private List<StepDescription> _stepsSample4 = new()
		{
			{ new StepDescription() { Id = "Init", Label="Initialisation", Clickable = true, IconName = IconNames.All.GearFill } },
			{ new StepDescription() { Id = "Review", Label="Plannification", IconName = IconNames.All.Calendar2MonthFill } },
			{ new StepDescription() { Id = "Send", Label="En cours", IconName = IconNames.All.Cpu} },
			{ new StepDescription() { Id = "Val", Label="Validation", IconName = IconNames.Save} },
			{ new StepDescription() { Id = "Close", Label="Terminé", IconName = IconNames.All.DoorClosedFill } }
		};

		private void StepClickSample4(string id)
		{
			// Validate the current step
			var step = _stepsSample4.Where(x => x.Id == id).First();
			step.Clickable = true;
			step.Validated = true;
			step.Message = "Validated by Admin \n " + DateTime.Now.ToLongTimeString();
			// 
			var nextStepIndex = _stepsSample4.IndexOf(step) + 1;
			bool nextClickable = true;
			while (nextStepIndex < _stepsSample4.Count)
			{
				var nextStep = _stepsSample4.ElementAt(nextStepIndex);
				nextStep.Clickable = nextClickable;
				nextStep.Validated = false;
				nextStep.Message = "";
				nextClickable = false;
				nextStepIndex++;
			}
		}

		#endregion

		#region Private classes 

		private class StepDescription
		{
			public string Id { get; set; }
			public string Label { get; set; }
			public string Message { get; set; }
			public string Css { get; set; }
			public bool Clickable { get; set; }
			public bool Validated { get; set; }
			public string IconName { get; set; }
		}

		#endregion

	}
