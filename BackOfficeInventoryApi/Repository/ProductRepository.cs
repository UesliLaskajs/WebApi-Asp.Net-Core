using BackOfficeInventoryApi.Data;
using BackOfficeInventoryApi.Models;
using BackOfficeInventoryApi.Repository.IRepository;

namespace BackOfficeInventoryApi.Repository
{
    public class ProductRepository : IProductRepository
    {

        private readonly ToDoContext _context;

        public ProductRepository(ToDoContext context)
        {
            _context = context;
        }

        public void AddProduct(Products product)
        {
            _context.Products.Add(product);
        }

        public void DeleteProduct(int productId)
        {
            var productToBeDeleted = _context.Products.Find(productId);
            if (productToBeDeleted != null) { 
                _context.Products.Remove(productToBeDeleted);
            }
        }

        public IEnumerable<Products> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        public Products GetProductById(int productId)
        {
            return _context.Products.Find(productId);
        }

        public void UpdateProduct(Products product)
        {
            _context.Products.Update(product);
        }
    }
}
