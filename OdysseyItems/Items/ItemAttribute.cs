namespace OdysseyItems.Items;

[AttributeUsage(AttributeTargets.Class)]
public class ItemAttribute : Attribute
{
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string BodyName { get; set; }
    public string CapName { get; set; }
}