using SafeDose.Domain.Common;

namespace SafeDose.Application.Contracts.Persistence
{
    public interface IGenericRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    {
        Task<IReadOnlyList<TEntity>> GetAsync();
        Task<TEntity?> GetByIdAsync(TKey id);
        Task CreateAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TKey entity);
    }
}
