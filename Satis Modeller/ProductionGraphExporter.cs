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
            var label = $@"Machine Type\n{node.Recipe.Output.Resource}\n{node.MachineCount.ToString("0.####", CultureInfo.InvariantCulture)} machines";
            dot.AppendLine($"\"{node.GetHashCode()}\" [label=\"{label}\", shape=box];");

            foreach (var output in node.Outputs)
            {
                var edgeLabel = node.Recipe.Output.Amount + "/min";
                dot.AppendLine($"\"{node.GetHashCode()}\" -> \"{output.GetHashCode()}\" [label=\"{edgeLabel}\"];");
            }

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
}