using Application.Enums;

namespace Application.Services.Interfaces;

public interface IService<T> where T : class
{
  Task<IEnumerable<T>> GetAllAsync();
  Task<T?> GetByIdAsync(int id);
  Task<T> AddAsync(T model);
  Task<UpdateResult> UpdateAsync(int id, T model);
  Task<bool> DeleteAsync(int id);
}