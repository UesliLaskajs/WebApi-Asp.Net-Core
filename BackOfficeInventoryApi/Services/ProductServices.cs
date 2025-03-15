using BackOfficeInventoryApi.Models;
using BackOfficeInventoryApi.Repository;
using BackOfficeInventoryApi.Repository.IRepository;
using BackOfficeInventoryApi.Services.IServices;

namespace BackOfficeInventoryApi.Services
{
    public class ProductServices : IProductServices
    {
        private readonly IProductRepository _productRepository;

        public   ProductServices(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task AddProduct(Products product)
        {
             await _productRepository.AddProduct(product);
        }

        public async Task DeleteProduct(int productId)
        {
            await _productRepository.DeleteProduct(productId);
        }

        public async Task<IEnumerable<Products>> GetAllProducts()
        {
            return await _productRepository.GetAllProducts();
        } 

        public async Task< Products> GetProductById(int productId)
        {
            return await _productRepository.GetProductById(productId);
        }

        public async Task UpdateProduct(Products product)
        {
            await _productRepository.UpdateProduct(product);
        }
    }
}
