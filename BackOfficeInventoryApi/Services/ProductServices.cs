using BackOfficeInventoryApi.Models;
using BackOfficeInventoryApi.Repository;
using BackOfficeInventoryApi.Repository.IRepository;
using BackOfficeInventoryApi.Services.IServices;

namespace BackOfficeInventoryApi.Services
{
    public class ProductServices : IProductServices
    {
        private readonly IProductRepository _productRepository;

        public ProductServices(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public void AddProduct(Products product)
        {
             _productRepository.AddProduct(product);
        }

        public void DeleteProduct(int productId)
        {
             _productRepository.DeleteProduct(productId);
        }

        public IEnumerable<Products> GetAllProducts()
        {
            return _productRepository.GetAllProducts();
        }

        public Products GetProductById(int productId)
        {
            return _productRepository.GetProductById(productId);
        }

        public void UpdateProduct(Products product)
        {
            _productRepository.UpdateProduct(product);
        }
    }
}
