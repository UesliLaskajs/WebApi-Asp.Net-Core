using BackOfficeInventoryApi.Models;

namespace BackOfficeInventoryApi.Repository.IRepository
{
    public interface IProductRepository
    {
        IEnumerable<Products> GetAllProducts();
        Products GetProductById(int productId);

        void AddProduct(Products product);

        void UpdateProduct(Products product);

        void DeleteProduct(int productId);
    }
}
