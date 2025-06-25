using System.Globalization;
using System.Text;

namespace Satis_Modeller;

public class MachineNode(Recipe recipe)
{
    public Recipe Recipe { get; } = recipe;

    public double MachineCount { get; private set; }

    public List<MachineNode> Inputs { get; set; } = [];
    public List<MachineNode> Outputs { get; set; } = [];

    public void SetOutputRate(double desiredOutputPerMinute)
    {
        var demandMap = new Dictionary<MachineNode, double>();
        CalculateRequiredOutputs(this, desiredOutputPerMinute, demandMap);

        foreach (var kv in demandMap)
        {
            var node = kv.Key;
            var rate = kv.Value;
            var perMachineOutput = node.Recipe.Output.Amount;
            node.MachineCount = rate / perMachineOutput;
        }
    }

    private static void CalculateRequiredOutputs(MachineNode node, double rate, Dictionary<MachineNode, double> demandMap)
    {
        demandMap.TryAdd(node, 0);

        demandMap[node] += rate;

        foreach (var input in node.Recipe.Inputs)
        {
            var inputPerMachine = input.Amount;
            var totalInputNeeded = (rate / node.Recipe.Output.Amount) * inputPerMachine;

            foreach (var inputNode in node.Inputs.Where(inputNode => inputNode.Recipe.Output.Resource == input.Resource))
            {
                CalculateRequiredOutputs(inputNode, totalInputNeeded, demandMap);
            }
        }
    }

    public void AddInput(MachineNode input)
    {
        Inputs.Add(input);
        input.Outputs.Add(this);
    }

    public void AddOutput(MachineNode output)
    {
        Outputs.Add(output);
        output.Inputs.Add(this);
    }

    public double GetTotalInputAmount(Resource resource)
    {
        return Recipe.Inputs.Where(i => i.Resource == resource).Sum(i => i.Amount) * MachineCount;
    }

    public double GetTotalOutputAmount(Resource resource)
    {
        return Recipe.Output.Amount * MachineCount;
    }

    override public string ToString()
    {
        // output should look like this
        // Screws:     50 screws/min       ->   Iron Plate: 20 plates/min
        // Iron Plate: 20 plates/min            Screws:     50 screws/min
        // Iron Ingot: 30 ingots/min

        var result = new StringBuilder();

        var longestInputResourceName = Recipe.Inputs.Max(i => i.Resource.ToString().Length);
        var longestInputAmountName = Recipe.Inputs.Max(i => i.Amount.ToString(CultureInfo.InvariantCulture).Length);

        var remainingInputs = Recipe.Inputs.ToList();
        var remainingOutputs = new List<ItemNode> { Recipe.Output, Recipe.Byproduct };

        var basePadding = 3;
        var resourcePadding = longestInputResourceName;
        var amountPadding = longestInputAmountName + basePadding;

        var row = 0;

        while (remainingInputs.Count != 0 || remainingOutputs.Count != 0)
        {
            var middle = "  ";

            var input = remainingInputs.FirstOrDefault();
            var output = remainingOutputs.FirstOrDefault();

            var inputString = input == null ? "" : string.Format($"{input.Resource.ToString().PadRight(resourcePadding)} {input.Amount.ToString(CultureInfo.InvariantCulture).PadLeft(amountPadding)}");

            // if (row == 0)
                middle = "->";

            var outputString = output == null ? "" : string.Format($"{output.Resource.ToString().PadRight(resourcePadding)} {output.Amount.ToString(CultureInfo.InvariantCulture).PadLeft(amountPadding)}");

            result.AppendLine($"{inputString} {middle} {outputString}");

            if (input != null)
                remainingInputs.Remove(input);

            if (output != null)
                remainingOutputs.Remove(output);

            row++;
        }

        return result.ToString();
    }
}