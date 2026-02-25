using AssignmentMVC.Models;

namespace AssignmentMVC.Repositories
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAll();
        void Add(Product product);
        void Update(Product product);
        void Delete(int id);
    }
}
