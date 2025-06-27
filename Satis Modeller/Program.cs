using Satis_Modeller;

public class Program
{
    public static void Main(string[] args)
    {
        // var nodes = ComputerExample();
        var nodes = ReinforcedIronPlateExample();

        ProductionGraphExporter.ExportToPng("production_graph", nodes.ToList());
    }

    public static IEnumerable<MachineNode> ComputerExample()
    {
        var copperOreRecipe = new Recipe(
            new ItemNode { Resource = Resource.CopperOre, Amount = 60 });

        var copperOreMiner = new MachineNode(copperOreRecipe, MachineType.Miner);

        var copperIngotsRecipe = new Recipe(
            new ItemNode { Resource = Resource.CopperOre, Amount = 30 },
            new ItemNode { Resource = Resource.CopperIngot, Amount = 30 });

        var copperSmelter = new MachineNode(copperIngotsRecipe, MachineType.Smelter);

        var wireRecipe = new Recipe(
            new ItemNode { Resource = Resource.CopperIngot, Amount = 15 },
            new ItemNode { Resource = Resource.Wire, Amount = 30 });

        var wireConstructor = new MachineNode(wireRecipe, MachineType.Constructor);

        var cableRecipe = new Recipe(
            new ItemNode { Resource = Resource.Wire, Amount = 60 },
            new ItemNode { Resource = Resource.Cable, Amount = 30 });

        var cableConstructor = new MachineNode(cableRecipe, MachineType.Constructor);

        var copperSheetRecipe = new Recipe(
            new ItemNode { Resource = Resource.CopperIngot, Amount = 20 },
            new ItemNode { Resource = Resource.CopperSheet, Amount = 10 });

        var copperSheetConstructor = new MachineNode(copperSheetRecipe, MachineType.Constructor);

// ------

        var oilExtractorRecipe = new Recipe(
            new ItemNode { Resource = Resource.CrudeOil, Amount = 60 });

        var oilExtractor = new MachineNode(oilExtractorRecipe, MachineType.Extractor);

        var plasticRecipe = new Recipe(
            new ItemNode { Resource = Resource.CrudeOil, Amount = 30 },
            new ItemNode { Resource = Resource.Plastic, Amount = 20 },
            new ItemNode { Resource = Resource.HeavyOilResidue, Amount = 10 });

        var plasticRefinery = new MachineNode(plasticRecipe, MachineType.Refinery);

        var fuelRecipe = new Recipe(
            new ItemNode { Resource = Resource.HeavyOilResidue, Amount = 60 },
            new ItemNode { Resource = Resource.Fuel, Amount = 40 });

        var fuelRefinery = new MachineNode(fuelRecipe, MachineType.Refinery);

// var heavyoilResidueStorage = StorageBoxFactory.CreateStorage(Resource.HeavyOilResidue);

// 15 sheet, 30 plastic -> 7.5  boards
        var circuitBoardRecipe = new Recipe(
            [
                new ItemNode { Resource = Resource.CopperSheet, Amount = 15 },
                new ItemNode { Resource = Resource.Plastic, Amount = 30 },
            ],
            new ItemNode { Resource = Resource.CircuitBoard, Amount = 7.5 });

        var circuitBoardAssembler = new MachineNode(circuitBoardRecipe, MachineType.Assembler);

        var computerRecipe = new Recipe(
            [
                new ItemNode { Resource = Resource.CircuitBoard, Amount = 10 },
                new ItemNode { Resource = Resource.Cable, Amount = 20 },
                new ItemNode { Resource = Resource.Plastic, Amount = 40 },
            ],
            new ItemNode { Resource = Resource.Computer, Amount = 2.5 }
        );

        var computerManufacturer = new MachineNode(computerRecipe, MachineType.Manufacturer);

        computerManufacturer.AddInput(circuitBoardAssembler);
        computerManufacturer.AddInput(cableConstructor);
        computerManufacturer.AddInput(plasticRefinery);

        circuitBoardAssembler.AddInput(copperSheetConstructor);
// circuitBoardAssembler.OutputLimit = 3;
        circuitBoardAssembler.AddInput(plasticRefinery);

        cableConstructor.AddInput(wireConstructor);
        wireConstructor.AddInput(copperSmelter);
        copperSheetConstructor.AddInput(copperSmelter);
        copperSmelter.AddInput(copperOreMiner);

        oilExtractor.OutputLimit = 30;
        plasticRefinery.AddInput(oilExtractor);
        fuelRefinery.AddInput(plasticRefinery);

        computerManufacturer.MaximizeOutput();
// computerManufacturer.SetOutputRate(4);
// fuelRefinery.SetOutputRate(20);

        Console.WriteLine($"Computer Manufacturer: {computerManufacturer.MachineCount} machines");
        Console.WriteLine($"Plastic Refinery: {plasticRefinery.MachineCount} machines");
        Console.WriteLine($"Copper Smelter: {copperSmelter.MachineCount} machines");
        Console.WriteLine($"Wire Constructor: {wireConstructor.MachineCount} machines");
        Console.WriteLine($"Cable Constructor: {cableConstructor.MachineCount} machines");
        Console.WriteLine($"Copper Sheet Constructor: {copperSheetConstructor.MachineCount} machines");
        Console.WriteLine($"Oil Extractor: {oilExtractor.MachineCount} machines");

        Console.WriteLine(
            $"Total copper used: {copperOreMiner.GetTotalOutputAmount(Resource.CopperOre)} copper/min in {copperOreMiner.MachineCount} machines");

        Console.WriteLine(
            $"Total copper ingots produced: {copperSmelter.GetTotalOutputAmount(Resource.CopperIngot)} ingots/min in {copperSmelter.MachineCount} machines");

        Console.WriteLine(
            $"Total wire produced: {wireConstructor.GetTotalOutputAmount(Resource.Wire)} wire/min in {wireConstructor.MachineCount} machines");

        Console.WriteLine(
            $"Total cables produced: {cableConstructor.GetTotalOutputAmount(Resource.Cable)} cables/min in {cableConstructor.MachineCount} machines");

        Console.WriteLine(
            $"Total copper sheets produced: {copperSheetConstructor.GetTotalOutputAmount(Resource.CopperSheet)} sheets/min in {copperSheetConstructor.MachineCount} machines");

        Console.WriteLine(
            $"Total oil extracted: {oilExtractor.GetTotalOutputAmount(Resource.CrudeOil)} crude oil/min in {oilExtractor.MachineCount} machines");

        Console.WriteLine(RecipiePrinter.Print(computerManufacturer.Recipe, 1));
        Console.WriteLine(RecipiePrinter.Print(computerManufacturer));
        Console.WriteLine(RecipiePrinter.Print(plasticRefinery));
        Console.WriteLine(RecipiePrinter.Print(fuelRefinery));


        var allNodes = new List<MachineNode>
        {
            computerManufacturer,
            circuitBoardAssembler,
            plasticRefinery,
            copperSmelter,
            wireConstructor,
            cableConstructor,
            copperSheetConstructor,
            oilExtractor,
            copperOreMiner,
            fuelRefinery,
        };

        return allNodes;
    }

    public static List<MachineNode> ReinforcedIronPlateExample()
    {
        var ironOreRecipe = new Recipe(
            new ItemNode { Resource = Resource.IronOre, Amount = 60 });

        var ironOreMiner = new MachineNode(ironOreRecipe, MachineType.Miner);

        var ironIngotsRecipe = new Recipe(
            new ItemNode { Resource = Resource.IronOre, Amount = 30 },
            new ItemNode { Resource = Resource.IronIngot, Amount = 30 });

        var ironIngots = new MachineNode(ironIngotsRecipe, MachineType.Smelter);

        var platesConstructorRecipe = new Recipe(
            new ItemNode { Resource = Resource.IronIngot, Amount = 30 },
            new ItemNode { Resource = Resource.IronPlate, Amount = 20 });

        var platesConstructor = new MachineNode(platesConstructorRecipe, MachineType.Assembler);

        var screwsConstructorRecipe = new Recipe(
            new ItemNode { Resource = Resource.IronIngot, Amount = 12.5 },
            new ItemNode { Resource = Resource.Screws, Amount = 50 });

        var screwsConstructor = new MachineNode(screwsConstructorRecipe, MachineType.Assembler);

        var reinforcedIronPlateRecipe = new Recipe(
            [
                new ItemNode { Resource = Resource.IronPlate, Amount = 30 },
                new ItemNode { Resource = Resource.Screws, Amount = 60 },
            ],
            new ItemNode { Resource = Resource.ReinforcedIronPlate, Amount = 5 });

        var reinforcedIronPlateAssembler = new MachineNode(reinforcedIronPlateRecipe, MachineType.Assembler);

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

        var allNodes = new List<MachineNode>
        {
            reinforcedIronPlateAssembler,
            ironOreMiner,
            ironIngots,
            platesConstructor,
            screwsConstructor,
        };

        return allNodes;
    }
}