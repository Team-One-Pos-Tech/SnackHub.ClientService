using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SnackHub.ClientService.Infra.Extensions;
using SnackHub.ClientService.Infra.Repositories.Abstractions;

namespace SnackHub.ClientService.Infra.Repositories;

public class BaseRepository<TModel, TDbContext> : IBaseRepository<TModel>
    where TModel : class
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;
    private readonly DbSet<TModel> _dbSet;

    protected HashSet<string> _expandProperties = [];

    protected BaseRepository(TDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TModel>();
    }

    public async Task InsertAsync(TModel model)
    {
        await _dbSet.AddAsync(model);
        await CompleteAsync();
    }

    public async Task UpdateAsync(TModel model)
    {
        _dbSet.Update(model);
        await CompleteAsync();
    }

    public async Task DeleteByPredicateAsync(Expression<Func<TModel, bool>> predicate)
    {
        var model = await _dbSet
            .AsNoTracking()
            .Where(predicate)
            .FirstOrDefaultAsync();

        if (model is null)
            return;

        _dbSet.Remove(model);
        await CompleteAsync();
    }

    public async Task<IEnumerable<TModel>> ListByPredicateAsync(Expression<Func<TModel, bool>> predicate)
    {
        return await _dbSet
            .AsNoTracking()
            .Inflate(_expandProperties)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<TModel?> FindByPredicateAsync(Expression<Func<TModel, bool>> predicate)
    {
        return await _dbSet
            .AsNoTracking()
            .Inflate(_expandProperties)
            .Where(predicate)
            .FirstOrDefaultAsync();
    }

    public async Task<(int pageNumber, int pageSize, int lastPage, IEnumerable<TModel> items)> ListPaginateAsync(
        Expression<Func<TModel, bool>> predicate, int pageNumber, int pageSize)
    {
        var count = await _dbSet
            .AsNoTracking()
            .Where(predicate)
            .CountAsync();

        if (count == 0)
            return (pageNumber, pageSize, 1, new List<TModel>());

        var elements = await _dbSet
            .Inflate(_expandProperties)
            .Where(predicate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalPages = count / pageSize;
        if (count % pageSize != 0) totalPages++;

        return (pageNumber, pageSize, totalPages, elements);
    }

    public async Task<int> CountAsync(Expression<Func<TModel, bool>> predicate)
    {
        return await _dbSet
            .CountAsync(predicate);
    }

    private async Task CompleteAsync()
    {
        //TODO: Move it to a better context {UnitOfWork or Transactions based}
        await _dbContext.SaveChangesAsync();
    }
}