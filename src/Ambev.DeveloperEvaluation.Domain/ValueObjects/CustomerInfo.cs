namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public sealed class CustomerInfo : IEquatable<CustomerInfo>
{
    public Guid Id { get; }
    public string Name { get; }
    public string Email { get; }

    public CustomerInfo(Guid id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }

    public override bool Equals(object? obj) => Equals(obj as CustomerInfo);
    public bool Equals(CustomerInfo? other) =>
        other != null && Id == other.Id && Name == other.Name && Email == other.Email;

    public override int GetHashCode() => HashCode.Combine(Id, Name, Email);
}