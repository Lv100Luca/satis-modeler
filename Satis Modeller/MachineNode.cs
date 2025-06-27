using System.Globalization;
using System.Text;

namespace Satis_Modeller;

public class MachineNode(Recipe recipe, MachineType type)
{
    public MachineType Type { get; } = type;
    public Recipe Recipe { get; } = recipe;

    public double MachineCount { get; private set; }

    public List<MachineNode> Inputs { get; set; } = [];
    public List<MachineNode> Outputs { get; set; } = [];

    public double? OutputLimit { get; set; }
    public bool IsBottleneck => CalculateIsBottleneck();

    private bool CalculateIsBottleneck()
    {
        if (OutputLimit == null)
            return false;

        var recipeOutput = Recipe.Output.Amount;

        var productionAmount = recipeOutput * MachineCount;

        return productionAmount.Equals(OutputLimit);
    }

    public void SetOutputRate(double desiredOutputPerMinute)
    {
        var demandMap = new Dictionary<MachineNode, double>();
        CalculateRequiredOutputs(this, desiredOutputPerMinute, demandMap);

        foreach (var (node, rate) in demandMap)
        {
            var perMachineOutput = node.Recipe.Output.Amount;
            node.MachineCount = rate / perMachineOutput;
        }
    }

    public void MaximizeOutput()
    {
        var visited = new HashSet<MachineNode>();
        PropagateForward(this, visited);

        // here we go again
        visited.Clear();
        PropagateBack(this, visited);
    }

    private static void PropagateForward(MachineNode node, HashSet<MachineNode> visited)
    {
        if (!visited.Add(node)) return;

        foreach (var input in node.Inputs)
            PropagateForward(input, visited);

        var limitingInputRatio = double.MaxValue;

        foreach (var input in node.Recipe.Inputs)
        {
            var supplyingNode = node.Inputs.FirstOrDefault(n =>
                n.Recipe.Output.Resource == input.Resource ||
                n.Recipe.Byproduct?.Resource == input.Resource);

            if (supplyingNode == null) continue;

            var availableRate = supplyingNode.GetTotalOutputAmount(input.Resource);
            var requiredPerMachine = input.Amount;

            var ratio = availableRate / requiredPerMachine;
            limitingInputRatio = Math.Min(limitingInputRatio, ratio);
        }

        if (node.OutputLimit.HasValue)
        {
            var maxByOutput = node.OutputLimit.Value / node.Recipe.Output.Amount;
            limitingInputRatio = Math.Min(limitingInputRatio, maxByOutput);
        }

        node.MachineCount = limitingInputRatio;

        foreach (var output in node.Outputs)
            PropagateForward(output, visited);
    }

    private static void PropagateBack(MachineNode node, HashSet<MachineNode> visited)
    {
        if (!visited.Add(node)) return;

        foreach (var input in node.Recipe.Inputs)
        {
            var inputRateNeeded = input.Amount * node.MachineCount;

            var supplyingNode = node.Inputs.FirstOrDefault(n =>
                n.Recipe.Output.Resource == input.Resource ||
                n.Recipe.Byproduct?.Resource == input.Resource);

            if (supplyingNode == null)
                continue;

            var requiredMachines = inputRateNeeded / supplyingNode.Recipe.Output.Amount;

            supplyingNode.MachineCount = Math.Min(supplyingNode.MachineCount, requiredMachines);

            PropagateBack(supplyingNode, visited);
        }
    }

    private static void CalculateRequiredOutputs(MachineNode node, double rate, Dictionary<MachineNode, double> demandMap)
    {
        demandMap.TryAdd(node, 0);

        demandMap[node] += rate;

        foreach (var input in node.Recipe.Inputs)
        {
            var inputPerMachine = input.Amount;
            var amount = node.Recipe.GetRateForResource(input.Resource);
            var totalInputNeeded = (rate / amount) * inputPerMachine;

            foreach (var inputNode in node.Inputs.Where(inputNode => inputNode.Recipe.Output.Resource == input.Resource))
                CalculateRequiredOutputs(inputNode, totalInputNeeded, demandMap);
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
        if (Recipe.Byproduct is not null && Recipe.Byproduct.Resource == resource)
            return Recipe.Byproduct.Amount * MachineCount;

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
        var remainingOutputs = new List<ItemNode> { Recipe.Output };
        if (Recipe.Byproduct is {} byproduct)
            remainingOutputs.Add(byproduct);

        var basePadding = 3;
        var resourcePadding = longestInputResourceName;
        var amountPadding = longestInputAmountName + basePadding;

        while (remainingInputs.Count != 0 || remainingOutputs.Count != 0)
        {
            var middle = "  ";

            var input = remainingInputs.FirstOrDefault();
            var output = remainingOutputs.FirstOrDefault();

            var inputString = input == null
                ? ""
                : string.Format(
                    $"{input.Resource.ToString().PadRight(resourcePadding)} {input.Amount.ToString(CultureInfo.InvariantCulture).PadLeft(amountPadding)}");

            middle = "->";

            var outputString = output == null
                ? ""
                : string.Format(
                    $"{output.Resource.ToString().PadRight(resourcePadding)} {output.Amount.ToString(CultureInfo.InvariantCulture).PadLeft(amountPadding)}");

            result.AppendLine($"{inputString} {middle} {outputString}");

            if (input != null)
                remainingInputs.Remove(input);

            if (output != null)
                remainingOutputs.Remove(output);
        }

        return result.ToString();
    }
}