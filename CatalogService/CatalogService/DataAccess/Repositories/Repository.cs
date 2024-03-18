using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace CatalogService.DataAccess.Repositories;

public class Repository<T> where T : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;
    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual IEnumerable<T> Get(Expression<Func<T, bool>>? filter = null, string includeProperties = "")
    {
        IQueryable<T> query = _dbSet;

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        foreach (var prop in includeProperties.Split
            (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(prop);
        }

        return query.ToList();
    }

    public virtual T? FindByKeys(params object[] keys)
    {
        return _dbSet.Find(keys);
    }

    public virtual EntityEntry<T> Insert(T entity)
    {
        return _dbSet.Add(entity);
    }

    public virtual EntityEntry<T> Update(T entity)
    {
        return _dbSet.Update(entity);
    }

    public virtual EntityEntry<T> Delete(int id)
    {
        T? entity = FindByKeys(id);

        if (entity is not null)
        {
            return Delete(entity);
        }

        return default!;
    }

    public virtual EntityEntry<T> Delete(T entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }

        return _dbSet.Remove(entity);
    }
}
