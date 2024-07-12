using System.Linq.Expressions;

namespace Para.Data.GenericRepository;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task Save();
    Task<TEntity?> GetById(long Id);
    Task Insert(TEntity entity);
    Task Update(TEntity entity);
    Task Delete(TEntity entity);
    Task Delete(long Id);
    Task<List<TEntity>> GetAll();

    // Sorgulanabilir bir IQueryable döndürür.
    Task<IQueryable<TEntity>> GetQueryable();

    // Ýliþkili tablolarý dahil ederek tüm varlýklarý getirir.
    Task<List<TEntity>> GetWithInclude(params Expression<Func<TEntity, object>>[] includeProperties);

    // Belirli bir koþula göre filtreleyerek ve iliþkili tablolarý dahil ederek varlýklarý getirir.
    Task<List<TEntity>> GetWithWhereAndInclude(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties);

    // dbContext.Customers.Where(x=> x.Name == "Ali") gibi sorguyu dinamik almak için
    // propertyName = sorgulanacak özelliðin adý "Name"
    // comparison = Karþýlaþtýrma operatörü QueryHelper daki (eq, neq, gt, gte, lt, lte, contains)
    // value = Karþýlaþtýrýlacak deðer "Ali" gibi
    // includeProperties = Include edilecek tablolar.
    Task<List<TEntity>> GetWithDynamicQuery(string propertyName, string comparison, string value, params Expression<Func<TEntity, object>>[] includeProperties);

}