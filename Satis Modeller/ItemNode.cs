namespace Satis_Modeller;

public class ItemNode
{
    public Resource Resource { get; init; }
    public double Amount { get; init; }

    public static ItemNode GetEmpty()
    {
        return new ItemNode { Resource = Resource.Empty, Amount = 0 };
    }
}