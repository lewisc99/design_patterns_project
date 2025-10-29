using System.Runtime.Intrinsics.Arm;

public class Computer
{
    public string CPU { get; }
    public string RAM { get; }
    public string Storage { get; }
    public string GraphicsCard { get; }
    public string PowerSupply { get; }
    public bool HasLiquidCooling { get; }

    internal Computer(string cpu, string ram, string storage, string graphicsCard, string powerSupply, bool hasLiquidCooling)
    {
        CPU = cpu;
        RAM = ram;
        Storage = storage;
        GraphicsCard = graphicsCard;
        PowerSupply = powerSupply;
        HasLiquidCooling = hasLiquidCooling;
    }
    public void DisplayConfiguration()
    {
        Console.WriteLine("--- PC Configuration ---");
        Console.WriteLine($"CPU: {CPU}");
        Console.WriteLine($"RAM: {RAM}");
        Console.WriteLine($"Storage: {Storage}");
        Console.WriteLine($"Graphics: {GraphicsCard ?? "Integrated"}");
        Console.WriteLine($"Power Supply: {PowerSupply}");
        Console.WriteLine($"Liquid Cooled: {HasLiquidCooling}");
    }
}

public class ComputerBuilder
{
    private string _cpu;
    private string _ram;
    private string _storage = "256GB SSD";
    private string _graphicsCard = null; // Optional
    private string _powerSupply = "500W";
    private bool _hasLiquidCooling = false; // Optional


    // The builder is instantiated with the *required* parameters.
    // This is a great way to enforce business rules.
    public ComputerBuilder(string cpu, string ram)
    {
        _cpu = cpu;
        _ram = ram;
    }

    // Each "With" method sets an optional part and returns 'this'
    // to allow for method chaining (a "fluent" API).

    public ComputerBuilder WithStorage(string storage)
    {
        _storage = storage;
        return this;
    }

    public ComputerBuilder WithGraphicsCard(string graphicsCard)
    {
        _graphicsCard = graphicsCard;
        return this;
    }

    public ComputerBuilder WithPowerSupply(string powerSupply)
    {
        _powerSupply = powerSupply;
        return this;
    }

    public ComputerBuilder WithLiquidCooling(bool enabled)
    {
        _hasLiquidCooling = enabled;
        return this;
    }

    public Computer Build()
    {
        if (_graphicsCard != null && _powerSupply == "500W")
        {
            throw new InvalidOperationException(
                "A powerful graphics card requires at least a 650W power supply."
            );
        }

        return new Computer(
            _cpu,
            _ram,
            _storage,
            _graphicsCard,
            _powerSupply,
            _hasLiquidCooling
        );
    }
}

public class ComputerManufacter
{
    // A Director method defines a specific sequence of steps
    public void ConstructGamingPc(ComputerBuilder builder)
    {
        builder
            .WithStorage("1 TB NVMe SSD")
            .WithGraphicsCard("NVIDIA RTX 4080")
            .WithPowerSupply("850W")
            .WithLiquidCooling(true);
    }

    public void ConstructOfficePC(ComputerBuilder builder)
    {
        builder.WithStorage("512 SSD");
    }
}