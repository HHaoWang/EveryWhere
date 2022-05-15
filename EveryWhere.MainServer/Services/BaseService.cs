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

    /// <summary>
    /// 获取满足条件的一个实体
    /// </summary>
    /// <param name="predicate">条件</param>
    /// <param name="noTracking">是否不追踪更改</param>
    /// <returns>满足条件的实体或空</returns>
    public virtual Task<T?> GetAsync(Expression<Func<T, bool>> predicate, bool noTracking = true)
    {
        return noTracking
            ? Repository.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate)
            : Repository.Set<T>().FirstOrDefaultAsync(predicate);
    }

    /// <summary>
    /// 根据ID获取一个实体
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <returns>满足条件的实体或空</returns>
    public virtual Task<T?> GetByIdAsync(int id)
    {
        return Repository.Set<T>().FindAsync(id).AsTask();
    }

    /// <summary>
    /// 获取所有满足条件的实体集合
    /// </summary>
    /// <param name="predicate">条件</param>
    /// <param name="noTracking">是否不追踪更改</param>
    /// <returns>所有满足条件的实体集合</returns>
    public virtual List<T> GetAll(Expression<Func<T, bool>> predicate, bool noTracking = true)
    {
        return noTracking ? 
            Repository.Set<T>().Where(predicate).AsNoTracking().ToList() 
            : Repository.Set<T>().Where(predicate).ToList();
    }

    /// <summary>
    /// 获取所有满足条件的实体查询集
    /// </summary>
    /// <param name="predicate">条件</param>
    /// <param name="noTracking">是否不追踪更改</param>
    /// <returns>所有满足条件的实体查询集</returns>
    public virtual IQueryable<T> GetQuery(Expression<Func<T, bool>> predicate, bool noTracking = true)
    {
        return noTracking ? Repository.Set<T>().AsNoTracking().Where(predicate) 
            : Repository.Set<T>().Where(predicate);
    }

    /// <summary>
    /// 获取实体查询集
    /// </summary>
    /// <param name="noTracking">是否不追踪更改</param>
    /// <returns>实体查询集</returns>
    public virtual IQueryable<T> GetQuery(bool noTracking = true)
    {
        return noTracking ? Repository.Set<T>().AsNoTracking() : Repository.Set<T>();
    }

    /// <summary>
    /// 添加一个实体到数据库
    /// </summary>
    /// <param name="t">要添加的实体</param>
    /// <returns>数据库更改行数</returns>
    public virtual async Task<int> AddAsync(T t)
    {
        await Repository.Set<T>().AddAsync(t);
        return await Repository.SaveChangesAsync();
    }

    /// <summary>
    /// 根据ID更新一个实体
    /// </summary>
    /// <param name="updatedEntity">一个包含ID和要修改的属性的实体。
    /// 无需修改的属性需要置空，否则将会覆盖数据库中数据</param>
    /// <returns>数据库更改行数</returns>
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

    /// <summary>
    /// 删除一个实体
    /// </summary>
    /// <param name="deletedEntity">要删除的实体，其中ID必须有</param>
    /// <returns>数据库更改行数</returns>
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