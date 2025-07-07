using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementação de IUserRepository utilizando Entity Framework Core e cache Redis.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly DefaultContext _context;
    private readonly IDistributedCache _cache;
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Inicializa uma nova instância de UserRepository.
    /// </summary>
    /// <param name="context">O contexto do banco de dados.</param>
    /// <param name="cache">O cache distribuído (Redis).</param>
    public UserRepository(DefaultContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }

    /// <inheritdoc />
    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        await _cache.RemoveAsync(CacheKeys.UserById(user.Id), cancellationToken);
        await _cache.RemoveAsync(CacheKeys.UserByEmail(user.Email), cancellationToken);
        await _cache.RemoveAsync(CacheKeys.UsersAll, cancellationToken);
        return user;
    }

    /// <inheritdoc />
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeys.UserById(id);
        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached != null)
            return JsonSerializer.Deserialize<User>(cached, _jsonOptions);

        var user = await _context.Users.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        if (user != null)
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(user, _jsonOptions), cancellationToken);
        return user;
    }

    /// <inheritdoc />
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeys.UserByEmail(email);
        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached != null)
            return JsonSerializer.Deserialize<User>(cached, _jsonOptions);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (user != null)
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(user, _jsonOptions), cancellationToken);
        return user;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(id, cancellationToken);
        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
        await _cache.RemoveAsync(CacheKeys.UserById(id), cancellationToken);
        await _cache.RemoveAsync(CacheKeys.UserByEmail(user.Email), cancellationToken);
        await _cache.RemoveAsync(CacheKeys.UsersAll, cancellationToken);
        return true;
    }
}
