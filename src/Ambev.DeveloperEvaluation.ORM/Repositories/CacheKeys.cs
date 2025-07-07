namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Contém as chaves de cache utilizadas pelos repositórios.
/// </summary>
public static class CacheKeys
{
    public const string SalesAll = "sales:all";
    public static string SaleById(Guid id) => $"sale:{id}";

    public const string UsersAll = "users:all";
    public static string UserById(Guid id) => $"user:{id}";
    public static string UserByEmail(string email) => $"user:email:{email}";
}