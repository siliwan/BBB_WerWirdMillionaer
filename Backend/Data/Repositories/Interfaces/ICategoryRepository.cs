using Backend.Data.Models;

namespace Backend.Data.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void AddCategory(string categoryName);
    }
}
