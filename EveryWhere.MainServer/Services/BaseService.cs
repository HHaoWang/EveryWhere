using EveryWhere.Database;
using EveryWhere.Database.PO;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EveryWhere.MainServer.Services;

public class BaseService<T> where T:BasePO
{
    protected readonly Repository Repository;

    public BaseService(Repository repository)
    {
        Repository = repository;
    }

    public virtual Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return Repository.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public virtual Task<T?> GetByIdAsync(int id)
    {
        return Repository.Set<T>().FindAsync(id).AsTask();
    }

    public virtual List<T> GetAll(Expression<Func<T, bool>> predicate)
    {
        return Repository.Set<T>().Where(predicate).ToList();
    }

    public virtual IQueryable<T> GetQuery(Expression<Func<T, bool>> predicate)
    {
        return Repository.Set<T>().Where(predicate);
    }

    public virtual async Task<int> AddAsync(T t)
    {
        await Repository.Set<T>().AddAsync(t);
        return await Repository.SaveChangesAsync();
    }

    public virtual async Task<int> UpdateAsync(T updatedEntity)
    {
        Repository.Set<T>().Attach(updatedEntity);
        var entry = Repository.Entry(updatedEntity);
        foreach (PropertyInfo propertyInfo in updatedEntity.GetType().GetProperties())
        {
            if (propertyInfo.GetValue(updatedEntity) != null)
            {
                entry.Property(t => propertyInfo).IsModified = true;
            }
        }

        return await Repository.SaveChangesAsync();
    }

    public virtual async Task<int> Delete(T deletedEntity)
    {
        Repository.Remove(deletedEntity);
        return await Repository.SaveChangesAsync();
    }
}