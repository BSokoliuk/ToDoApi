namespace Application.Services.Interfaces;

public interface IService<T> where T : class
{
  Task<IEnumerable<T>> GetAllAsync();
  Task<T?> GetByIdAsync(int id);
  Task<T> AddAsync(T model);
  Task<bool> Update(int id, T model);
  Task<bool> Delete(int id);
}