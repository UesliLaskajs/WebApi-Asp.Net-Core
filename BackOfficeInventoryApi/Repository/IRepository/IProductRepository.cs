using BackOfficeInventoryApi.Models;

namespace BackOfficeInventoryApi.Repository.IRepository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Products>> GetAllProducts();
        Task<Products> GetProductById(int productId);

        Task AddProduct(Products product);

        Task UpdateProduct(Products product);

        Task DeleteProduct(int productId);
    }
}
