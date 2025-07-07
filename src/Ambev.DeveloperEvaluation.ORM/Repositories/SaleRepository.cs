using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementação de ISaleRepository utilizando o Entity Framework Core e cache Redis.
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;
    private readonly IDistributedCache _cache;
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
    private readonly ILogger<SaleRepository> _logger;

    /// <summary>
    /// Inicializa uma nova instância de SaleRepository.
    /// </summary>
    /// <param name="context">O contexto do banco de dados.</param>
    /// <param name="cache">O cache distribuído (Redis).</param>
    /// <param name="logger">O logger para registrar eventos.</param>
    public SaleRepository(DefaultContext context, IDistributedCache cache, ILogger<SaleRepository> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Obtém uma venda pelo identificador único.
    /// </summary>
    /// <param name="id">Identificador da venda.</param>
    /// <returns>A venda encontrada ou null.</returns>
    public async Task<Sale?> GetByIdAsync(Guid id)
    {
        var cacheKey = CacheKeys.SaleById(id);
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
            return JsonSerializer.Deserialize<Sale>(cached, _jsonOptions);

        var sale = await _context.Sales.AsNoTracking().Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id);
        if (sale != null)
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(sale, _jsonOptions));
        return sale;
    }

    /// <summary>
    /// Obtém todas as vendas.
    /// </summary>
    /// <returns>Lista de vendas.</returns>
    public async Task<IEnumerable<Sale>> GetAllAsync()
    {
        var cacheKey = CacheKeys.SalesAll;
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
            return JsonSerializer.Deserialize<List<Sale>>(cached, _jsonOptions) ?? new List<Sale>();

        var sales = await _context.Sales.AsNoTracking().Include(s => s.Items).ToListAsync();
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(sales, _jsonOptions));
        return sales;
    }

    /// <summary>
    /// Adiciona uma nova venda ao banco de dados.
    /// </summary>
    /// <param name="sale">A venda a ser adicionada.</param>
    public async Task AddAsync(Sale sale)
    {
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();
        await _cache.RemoveAsync(CacheKeys.SalesAll);
        await _cache.RemoveAsync(CacheKeys.SaleById(sale.Id));
    }

    /// <summary>
    /// Atualiza uma venda existente no banco de dados.
    /// </summary>
    /// <param name="sale">A venda a ser atualizada.</param>
    public async Task UpdateAsync(Sale sale)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync();
        await _cache.RemoveAsync(CacheKeys.SalesAll);
        await _cache.RemoveAsync(CacheKeys.SaleById(sale.Id));
    }

    /// <summary>
    /// Remove uma venda do banco de dados pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da venda a ser removida.</param>
    public async Task DeleteAsync(Guid id)
    {
        var sale = await GetByIdAsync(id);
        if (sale != null)
        {
            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync(CacheKeys.SalesAll);
            await _cache.RemoveAsync(CacheKeys.SaleById(id));
        }
    }
}