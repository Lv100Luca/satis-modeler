using System.Globalization;
using System.Text;

namespace Satis_Modeller;

public class Recipe(IEnumerable<ItemNode> inputs, ItemNode output, ItemNode byproduct)
{
    public IEnumerable<ItemNode> Inputs { get; } = inputs;
    public ItemNode Output { get; } = output;
    public ItemNode Byproduct { get; } = byproduct;

    public Recipe(IEnumerable<ItemNode> inputs, ItemNode output) : this(inputs, output, ItemNode.GetEmpty())
    {
    }

    public Recipe(ItemNode output) : this([ItemNode.GetEmpty()], output)
    {
    }

    public Recipe(ItemNode input, ItemNode output) : this([input], output)
    {
    }
}