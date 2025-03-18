using BackOfficeInventoryApi.Data;
using BackOfficeInventoryApi.Models;
using BackOfficeInventoryApi.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BackOfficeInventoryApi.Repository
{
    public class ProductRepository : IProductRepository
    {

        private readonly ToDoContext _context;

        public ProductRepository(ToDoContext context)
        {
            _context = context;
        }

        public async Task AddProduct(Products product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProduct(int productId)
        {
            var productToBeDeleted = await _context.Products.FindAsync(productId);
            if (productToBeDeleted != null) { 
                _context.Products.Remove(productToBeDeleted);
                await _context.SaveChangesAsync();
            }
           
        }

        public async Task<IEnumerable<Products>> GetAllProducts()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Products> GetProductById(int productId)
        {
            return await _context.Products.FindAsync(productId);
        }

        public async Task UpdateProduct(Products product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}
