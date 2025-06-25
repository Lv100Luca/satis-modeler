// See https://aka.ms/new-console-template for more information

using Satis_Modeller;

var ironOreRecipe = new Recipe(
    new ItemNode { Resource = Resource.IronOre, Amount = 60 });

var ironOreMiner = new MachineNode(ironOreRecipe);

var ironIngotsRecipe = new Recipe(
    new ItemNode { Resource = Resource.IronOre, Amount = 30 },
    new ItemNode { Resource = Resource.IronIngot, Amount = 30 });

var ironIngots = new MachineNode(ironIngotsRecipe);

var platesConstructorRecipe = new Recipe(
    new ItemNode { Resource = Resource.IronIngot, Amount = 30 },
    new ItemNode { Resource = Resource.IronPlate, Amount = 20 });

var platesConstructor = new MachineNode(platesConstructorRecipe);

var screwsConstructorRecipe = new Recipe(
    new ItemNode { Resource = Resource.IronIngot, Amount = 12.5 },
    new ItemNode { Resource = Resource.Screws, Amount = 50 });

var screwsConstructor = new MachineNode(screwsConstructorRecipe);

var reinforcedIronPlateRecipe = new Recipe(
    [
        new ItemNode { Resource = Resource.IronPlate, Amount = 30 },
        new ItemNode { Resource = Resource.Screws, Amount = 60 },
    ],
    new ItemNode { Resource = Resource.ReinforcedIronPlate, Amount = 5 });

var reinforcedIronPlateAssembler = new MachineNode(reinforcedIronPlateRecipe);

ironOreMiner.AddOutput(ironIngots);

ironIngots.AddOutput(platesConstructor);
ironIngots.AddOutput(screwsConstructor);

screwsConstructor.AddOutput(reinforcedIronPlateAssembler);
platesConstructor.AddOutput(reinforcedIronPlateAssembler);

reinforcedIronPlateAssembler.SetOutputRate(12);

Console.WriteLine($"Constructor: {reinforcedIronPlateAssembler.MachineCount} machines");
Console.WriteLine($"Smelter: {ironIngots.MachineCount} machines");
Console.WriteLine($"Miner: {ironOreMiner.MachineCount} machines");

Console.WriteLine(
    $"Total ore used: {ironOreMiner.GetTotalOutputAmount(Resource.IronOre)} ore/min in {ironOreMiner.MachineCount} machines");

Console.WriteLine(
    $"Total plates produced: {platesConstructor.GetTotalOutputAmount(Resource.IronPlate)} plates/min in {platesConstructor.MachineCount} machines");

Console.WriteLine(
    $"Total screws produced: {screwsConstructor.GetTotalOutputAmount(Resource.Screws)} screws/min in {screwsConstructor.MachineCount} machines");

Console.WriteLine(
    $"Total reinforced plates produced: {reinforcedIronPlateAssembler.GetTotalOutputAmount(Resource.ReinforcedIronPlate)} plates/min");

Console.WriteLine(RecipiePrinter.Print(reinforcedIronPlateAssembler));