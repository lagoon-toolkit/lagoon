namespace Lagoon.UI.Demo.ViewModel;

public class Pet
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Type { get; set; } = "";

    public string Color { get; set; }

    public int Year { get; set; }

    public bool Lock { get; set; }

    public Pet(string name)
    {
        Name = name;
    }

    public Pet()
    {

    }


}
