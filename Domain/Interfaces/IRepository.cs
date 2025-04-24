namespace Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> AddAsync(T model);
    void Update(T model);
    void Delete(T model);
}
// This interface defines the basic CRUD operations for a repository pattern.