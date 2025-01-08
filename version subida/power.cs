class Power
{
    public string Name { get; }
    public string Description { get; }
    public int Cooldown { get; set; }
    public Action<Player, Random> Activate { get; }

    public Power(string name, string description, int cooldown, Action<Player, Random> activate)
    {
        Name = name;
        Description = description;
        Cooldown = cooldown;
        Activate = activate;
    }
}


