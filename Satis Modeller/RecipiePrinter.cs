using System.Globalization;
using System.Text;

namespace Satis_Modeller;

public class RecipiePrinter
{
    public static string Print(Recipe recipe, double multiplier)
    {
        // output should look like this
        // IronPlate 30/min  ->  ReinforcedIronPlate 5/min
        // Screws    60/min

        var result = new StringBuilder();

        var longestInputResourceName = recipe.Inputs.Max(i => i.Resource.ToString().Length);
        var longestInputAmountName = recipe.Inputs.Max(i => (i.Amount * multiplier).ToString(CultureInfo.InvariantCulture).Length);

        var longestOutputResourceName = recipe.Output.Resource.ToString().Length;
        var longestOutputAmountName = (recipe.Output.Amount * multiplier).ToString(CultureInfo.InvariantCulture).Length;

        var remainingInputs = recipe.Inputs.Where(i => i.Resource != Resource.Empty).ToList();
        var remainingOutputs = new List<ItemNode> { recipe.Output, recipe.Byproduct }.Where(i  => i.Resource != Resource.Empty).ToList();

        const int amountPadding = 4;
        var inputResourcePadding = longestInputResourceName;
        var inputAmountPadding = longestInputAmountName;

        var outputResourcePadding = longestOutputResourceName;
        var outputAmountPadding = longestOutputAmountName;

        var row = 0;

        while (remainingInputs.Count != 0 || remainingOutputs.Count != 0)
        {
            var middle = "  ";

            var input = remainingInputs.FirstOrDefault();
            var output = remainingOutputs.FirstOrDefault();

            if (input != null)
                remainingInputs.Remove(input);

            if (output != null)
                remainingOutputs.Remove(output);

            var inputString = input == null
                ? "".PadRight(inputResourcePadding + 1 + inputAmountPadding + amountPadding)
                : string.Format($"{input.Resource.ToString().PadRight(inputResourcePadding)} {(input.Amount * multiplier).ToString(CultureInfo.InvariantCulture).PadLeft(inputAmountPadding)}/min");

            if (row == 0)
                middle = "->";

            var outputString = output == null
                ? string.Empty
                : string.Format($"{output.Resource.ToString().PadRight(outputResourcePadding)} {(output.Amount * multiplier).ToString(CultureInfo.InvariantCulture).PadLeft(outputAmountPadding)}/min");

            result.AppendLine($"{inputString}   {middle}   {outputString}");

            row++;
        }

        return result.ToString();
    }

    public static string Print(MachineNode machine)
    {
        return Print(machine.Recipe, machine.MachineCount);
    }
}