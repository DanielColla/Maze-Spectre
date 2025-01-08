using System;

public class Player
{
    public string Name { get; set; }
    public string PowerDescription { get; set; }
    public string Story { get; set; }  // Nueva propiedad para la historia

    public Player(string name, string powerDescription, string story)
    {
        Name = name;
        PowerDescription = powerDescription;
        Story = story;
    }
}

