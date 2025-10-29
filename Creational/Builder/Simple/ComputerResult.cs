var gamingPCBuilder = new ComputerBuilder("Intel i9-13900k", "32GB DDR5");

Computer gamingPC = gamingPCBuilder
    .WithStorage("2TB NVMe SSD")
    .WithGraphicsCard("NVDIA RTX 4090")
    .WithPowerSupply("1000W")
    .WithLiquidCooling(true)
    .Build();

gamingPC.DisplayConfiguration();

Console.WriteLine();

// Build a simple office PC
// We only specify the required parts and get all the defaults.
var officePCBuilder = new ComputerBuilder("Intel i5-13400", "16GB DDR4");

Computer officePC = officePCBuilder
    .WithStorage("512GB SSD") // Just override one default
    .Build();

officePC.DisplayConfiguration();

Console.WriteLine();

//Intermediate Concept: The "Director"

var manufacturer = new ComputerManufacter();

var gamingBuilder = new ComputerBuilder("intel i7", "32GB");

manufacturer.ConstructGamingPc(gamingBuilder);

gamingBuilder.WithStorage("4TB SSD");

Computer customerGamingPC = gamingBuilder.Build();

customerGamingPC.DisplayConfiguration();
