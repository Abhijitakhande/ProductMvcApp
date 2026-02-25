namespace AssignmentMVC.Repositories
{
    using AssignmentMVC.Data;
    using AssignmentMVC.Models;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Distributed;
    using Newtonsoft.Json;
    using System.Text;

    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;

        public ProductRepository(ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public IEnumerable<Product> GetAll()
        {
            string cacheKey = "productList";
            var cachedData = _cache.GetString(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonConvert.DeserializeObject<List<Product>>(cachedData);
            }

            var products = _context.Products
                .FromSqlRaw("EXEC sp_GetProducts")
                .ToList();

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));

            _cache.SetString(cacheKey, JsonConvert.SerializeObject(products), options);

            return products;
        }

        public void Add(Product product)
        {
            try
            {
                _context.Database.ExecuteSqlRaw(
                    "EXEC sp_AddProduct @Name, @Price, @Quantity",
                    new SqlParameter("@Name", product.Name),
                    new SqlParameter("@Price", product.Price),
                    new SqlParameter("@Quantity", product.Quantity));

                _cache.Remove("productList");
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) 
                {
                    throw new Exception("Product name already exists.");
                }
                throw;
            }
        }
        public void Update(Product product)
        {
            try
            {
                _context.Database.ExecuteSqlRaw(
                    "EXEC sp_UpdateProduct @Id, @Name, @Price, @Quantity",
                    new SqlParameter("@Id", product.Id),
                    new SqlParameter("@Name", product.Name),
                    new SqlParameter("@Price", product.Price),
                    new SqlParameter("@Quantity", product.Quantity));

                _cache.Remove("productList");
            }
            catch (SqlException ex)
            {
                if (ex.Number == 50000 || ex.Number == 2627)
                {
                    throw new Exception("Product name already exists.");
                }
                throw;
            }
        }

        public void Delete(int id)
        {
            _context.Database.ExecuteSqlRaw(
                "EXEC sp_DeleteProduct @Id",
                new SqlParameter("@Id", id));

            _cache.Remove("productList");
        }
    }
}
