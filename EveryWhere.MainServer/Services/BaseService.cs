using System.Diagnostics;
using EveryWhere.Database;
using EveryWhere.Database.PO;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EveryWhere.MainServer.Services;

public class BaseService<T> where T:BasePO
{
    protected readonly Repository Repository;

    public BaseService(Repository repository)
    {
        Repository = repository;
    }

    public virtual Task<T?> GetAsync(Expression<Func<T, bool>> predicate, bool noTracking = true)
    {
        return noTracking
            ? Repository.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate)
            : Repository.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public virtual Task<T?> GetByIdAsync(int id)
    {
        return Repository.Set<T>().FindAsync(id).AsTask();
    }

    public virtual List<T> GetAll(Expression<Func<T, bool>> predicate)
    {
        return Repository.Set<T>().Where(predicate).ToList();
    }

    public virtual IQueryable<T> GetQuery(Expression<Func<T, bool>> predicate, bool noTracking = true)
    {
        return noTracking ? Repository.Set<T>().AsNoTracking().Where(predicate) 
            : Repository.Set<T>().Where(predicate);
    }

    public virtual IQueryable<T> GetQuery(bool noTracking = true)
    {
        return noTracking ? Repository.Set<T>().AsNoTracking() : Repository.Set<T>();
    }

    public virtual async Task<int> AddAsync(T t)
    {
        await Repository.Set<T>().AddAsync(t);
        return await Repository.SaveChangesAsync();
    }

    public virtual async Task<int> UpdateAsync(T updatedEntity)
    {
        List<PropertyInfo> propertyInfos = new(updatedEntity.GetType().GetProperties());
        int id = (int)updatedEntity.GetType().GetProperty("Id")?.GetValue(updatedEntity)!;
        T? existPo = await GetByIdAsync(id);
        if (existPo == null)
        {
            Debug.WriteLine("更新数据时未查询到相关数据！");
            return 0;
        }

        foreach (PropertyInfo propertyInfo in propertyInfos
                     .Where(propertyInfo => propertyInfo.GetValue(updatedEntity) != null))
        {
            existPo.GetType().GetProperty(propertyInfo.Name)!.SetValue(existPo,propertyInfo.GetValue(updatedEntity));
        }

        updatedEntity = existPo;
        
        return await Repository.SaveChangesAsync();
    }

    public virtual async Task<int> Delete(T deletedEntity)
    {
        Repository.Remove(deletedEntity);
        return await Repository.SaveChangesAsync();
    }

    /// <summary>
    /// 删除指定ID对应的实体
    /// </summary>
    /// <param name="id"></param>
    /// <returns>删除实体数，若没有ID对应的实体则返回1</returns>
    public virtual async Task<int> Delete(int id)
    {
        T? needDeletedPo = await GetByIdAsync(id);
        if (needDeletedPo == null) return 1;
        Repository.Set<T>().Remove(needDeletedPo);
        return await Repository.SaveChangesAsync();
    }
}