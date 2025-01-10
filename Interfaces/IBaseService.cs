namespace ChristmasGiftApi.Interfaces;

public interface IBaseService<T, TCreate, TUpdate>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> CreateAsync(TCreate createDto);
    Task<T?> UpdateAsync(int id, TUpdate updateDto);
    Task<bool> DeleteAsync(int id);
} 