namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public sealed class ProductInfo : IEquatable<ProductInfo>
{
    public Guid Id { get; }
    public string Name { get; }

    public ProductInfo(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public override bool Equals(object? obj) => Equals(obj as ProductInfo);
    public bool Equals(ProductInfo? other) =>
        other != null && Id == other.Id && Name == other.Name;

    public override int GetHashCode() => HashCode.Combine(Id, Name);
}