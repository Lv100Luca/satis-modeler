using System.Diagnostics;
using System.Globalization;
using System.Text;
using Satis_Modeller;

public static class ProductionGraphExporter
{
    public static void ExportToPng(string fileName, List<MachineNode> allNodes)
    {
        var dot = new StringBuilder();
        dot.AppendLine("digraph G {");
        dot.AppendLine("rankdir=LR;"); // left-to-right layout

        var added = new HashSet<MachineNode>();

        foreach (var node in allNodes)
        {
            dot.AppendLine($"\"{node.GetHashCode()}\" [label=\"{GetNodeLabel(node)}\", shape=box];");

            if (node.Type == MachineType.Refinery)
            {
                Console.Out.WriteLine("Refinery");
            }

            foreach (var output in node.Outputs)
            {
                var resource = node.Recipe.Output.Resource;
                //todo add extension
                dot.AppendLine($"\"{node.GetHashCode()}\" -> \"{output.GetHashCode()}\" [label=\"{GetEdgeLabel(node.Recipe.Output.Resource, output.GetTotalInputAmount(resource))}\"];");
            }

            if (IsByProductUsedUp(node))
                continue;

            var byproduct = node.Recipe.Byproduct;

            if (byproduct is null)
                continue;

            var byproductAmount = node.GetTotalOutputAmount(byproduct.Resource);

            var byproductLabel =
                $"Byproduct\n{byproduct.Resource}\n{byproductAmount.ToString("0.####", CultureInfo.InvariantCulture)}/min";

            var byproductNodeId = $"byproduct_{node.GetHashCode()}_{byproduct.Resource}";

            dot.AppendLine($"\"{byproductNodeId}\" [label=\"{byproductLabel}\", shape=ellipse, style=dashed];");
            dot.AppendLine($"\"{node.GetHashCode()}\" -> \"{byproductNodeId}\" [label=\"{GetEdgeLabel(byproduct.Resource, byproductAmount)}\",style=dotted];");

            added.Add(node);
        }

        dot.AppendLine("}");

        var outputDir = Path.GetDirectoryName(fileName);

        if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }


        var dotFile = $"{fileName}.dot";
        var pngFile = $"{fileName}.png";
        File.WriteAllText(dotFile, dot.ToString());

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = @"C:\Program Files\\Graphviz\\bin\\dot.exe",
                Arguments = $"-Tpng {dotFile} -o {pngFile}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit();

        Console.WriteLine($"Graph exported to {pngFile}");
    }

    private static bool IsByProductUsedUp(MachineNode machine)
    {
        var byProduct = machine.Recipe.Byproduct;

        if (byProduct is null)
            return true;

        var byproductAmount = machine.GetTotalOutputAmount(byProduct.Resource);
        Console.Out.WriteLine($"Byproduct {byProduct.Resource} amount: {byproductAmount}");

        return false;
    }

    private static string GetNodeLabel(MachineNode node)
    {
        return
            $@"{node.Type}\n{node.Recipe.Output.Resource}\n{node.MachineCount.ToString("0.####", CultureInfo.InvariantCulture)} machines";
    }

    private static string GetEdgeLabel(Resource resource, double amount)
    {
        return $"{resource}\n{amount.ToString("0.####", CultureInfo.InvariantCulture)}/min";
    }
}