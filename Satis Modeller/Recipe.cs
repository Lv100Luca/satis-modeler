using System.Globalization;
using System.Text;

namespace Satis_Modeller;

public class Recipe(IEnumerable<ItemNode> inputs, ItemNode output, ItemNode? byproduct = null)
{
    public IEnumerable<ItemNode> Inputs { get; } = inputs;
    public ItemNode Output { get; } = output;
    public ItemNode? Byproduct { get; } = byproduct;

    public Recipe(ItemNode output) : this([ItemNode.GetEmpty()], output)
    {
    }

    public Recipe(ItemNode input, ItemNode output) : this([input], output)
    {
    }

    public Recipe(ItemNode input, ItemNode output, ItemNode? byproduct) : this([input], output, byproduct)
    {
    }
}