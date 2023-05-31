using System.ComponentModel.DataAnnotations;

namespace Lagoon.UI.Demo.ViewModel;

public class FormData
{
    public bool Toggle { get; set; } = true;
    public bool? Check { get; set; } = true;

    public List<(string Text, int Value, bool Disabled)> CheckBoxListData = new()
    {
        ("Item1", 1, false),
        ("Item2", 2, false),
        ("Item3", 3, false),
        ("Item4", 4, false)
    };

    [Required(ErrorMessage = "Please check one option at least")]
    public ICollection<int> CheckBoxSelectedItems { get; set; }

    [Required(ErrorMessage = "Color is required")]

    public int? Color { get; set; }

    public List<(int? value, string Label, bool Disabled)> Colors { get; set; } = new List<(int? value, string Label, bool Disabled)>()
            {
                ( 1 , "Blue" ,false),
                ( 2 , "Red",false),
                ( 3 , "Black",false ),
                ( 4 , "White",false ),
                ( 5 , "Orange",false ),
                ( 6 , "Purple",false ),
                ( 8 , "Green",false ),
                ( 9 , "Grey",false )
            };

    [Required(ErrorMessage = "Color is required")]
    public int? ColorOnDemand { get; set; } = null;

    public List<(int? value, string Label, bool Disabled)> ColorsOnDemand { get; set; } = new List<(int? value, string Label, bool Disabled)>();

    [Required(ErrorMessage = "Please select a value")]
    public List<int?> ColorsMultiple { get; set; } = new List<int?>();

    public List<(int? value, string Label, bool Disabled)> ColorsMultipleList { get; set; } = new List<(int? value, string Label, bool Disabled)>()
            {
                ( 1 , "Blue" ,false),
                ( 2 , "Red",false),
                ( 3 , "Black",false ),
                ( 4 , "White",false ),
                ( 5 , "Orange",false ),
                ( 6 , "Purple",false ),
                ( 7 , "Yellow",false ),
                ( 8 , "Green",false ),
            };

    [Required(ErrorMessage = "Please select a value")]
    public List<int?> ColorsMultipleOnDemands { get; set; } = new List<int?>();

    public List<(int? value, string Label, bool Disabled)> ColorsMultipleOnDemandsList { get; set; } = new List<(int? value, string Label, bool Disabled)>();

    public string LastName { get; set; }

    [MaxLength(20)]
    public string FirstName { get; set; }

    public string ColorText { get; set; }

    public DateTime? Date1 { get; set; }

    [Required(ErrorMessage = "#mandatoryDate")]
    public DateTime? Date2 { get; set; }

    public int? Year { get; set; } = DateTime.Now.Year;

    public DateTime? MonthPickerDate { get; set; } 

    public double DoubleValue { get; set; } = 1000;

    public string HtmlTextContent { get; set; } = "<p>Hello !</> ";

    public string Multiline { get; set; } = "";

    [Required(ErrorMessage = "Select one option!")]
    public string Option { get; set; }

    [Required(ErrorMessage = "Select one option!")]
    public Boolean OptionBool { get; set; }

    public static List<(string value, string lable, string tooltip)> Options => new()
            {
                ( "Option 1", "Option 1","Tooltip option 1" ),
                ( "Option 2", "Option 2","Tooltip option 2" ),
                ( "Option 3", "Option 3","Tooltip option 3" ),
            };

    public string Password { get; set; } = "Mon_Password!";

    public string Summary { get; set; }

    [Range(-10, 50, ErrorMessage = "Please enter a number between -10 and 50.")]
    public int TemperatureC { get; set; }

    [Required(ErrorMessage = "Required time")]
    public DateTime? Time { get; set; }

    [Required(ErrorMessage = "Required time (timeSpan)")]
    public TimeSpan? TimeSP { get; set; }

    public Pet Pet { get; set; }

    [MinLength(5)]
    public string LongText { get; set; }

    public List<(int? value, string Label, bool Disabled)> ColorList { get; set; } = new List<(int? value, string Label, bool Disabled)>()
            {
                ( 1 , "Blue" ,false),
                ( 2 , "Red",false),
                ( 3 , "Black",false ),
                ( 4 , "White",false ),
                ( 5 , "Orange",false ),
                ( 6 , "Purple",false ),
                ( 7 , "Yellow",false ),
                ( 8 , "Green",false ),
            };


}
