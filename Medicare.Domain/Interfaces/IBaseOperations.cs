namespace Medicare.Domain.Interfaces
{
    public interface IBaseOperations<T>
    {
        Task CreateAsync(T model);
        Task UpdateAsync(T model);
        Task DeleteAsync(T model);
        Task GetAllAsync();
    }
}
