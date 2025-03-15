using BackOfficeInventoryApi.Models;

namespace BackOfficeInventoryApi.Services.IServices
{
    public interface IProductServices
    {
        Task<IEnumerable<Products>> GetAllProducts();
        Task<Products> GetProductById(int productId);

        Task AddProduct(Products product);

        Task UpdateProduct(Products product);

        Task DeleteProduct(int productId);
    }
}
