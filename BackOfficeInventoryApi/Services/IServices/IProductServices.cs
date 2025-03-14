using BackOfficeInventoryApi.Models;

namespace BackOfficeInventoryApi.Services.IServices
{
    public interface IProductServices
    {
        IEnumerable<Products> GetAllProducts();
        Products GetProductById(int productId);

        void AddProduct(Products product);

        void UpdateProduct(Products product);

        void DeleteProduct(int productId);
    }
}
