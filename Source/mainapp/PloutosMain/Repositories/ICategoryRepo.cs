using PloutosMain.Models;

namespace PloutosMain.Repositories
{
    public interface ICategoryRepo
    {
        Category GetCategory(int categoryId);
        Category InsertCategory(Category newCategory);
        Category UpdateCategory(Category modifiedCategory);
        void DeleteCategory(int categoryId);
    }
}
