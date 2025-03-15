using BackOfficeInventoryApi.Data;
using BackOfficeInventoryApi.Models;
using BackOfficeInventoryApi.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace BackOfficeInventoryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductServices _productServices;
        
        public ProductsController(IProductServices productServices)
        {
            _productServices = productServices;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Products>>> GetProducts()
        {

            var products = await _productServices.GetAllProducts();
             return  Ok(products);

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Products>> GetProduct(int id)
        {
            var product = await _productServices.GetProductById(id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }
            return Ok(product);
        }

        [HttpPost]

        public async Task<ActionResult<Products>> CreateProduct(Products products)
        {

            if (products == null || string.IsNullOrEmpty(products.Name))
            {
                return BadRequest("Invalid Product Data");
            }

             await _productServices.AddProduct(products);

            return CreatedAtAction(nameof(GetProduct), new {id=products.Id},products);

        }

        [HttpPut]
        public async Task<ActionResult<Products>> UpdateProduct(int id,Products updatedProduct)
        {
            if (id == 0 || updatedProduct == null || id != updatedProduct.Id)
            {
                return BadRequest("Invalid Id or Product data.");
            }

            var productToBeUpdated = await _productServices.GetProductById(id);  // Find the product by id

            if (productToBeUpdated == null)
            {
                return NotFound("Product not found.");
            }

            await _productServices.UpdateProduct(productToBeUpdated);

            return Ok(productToBeUpdated);
        }

        [HttpDelete]

        public async Task<ActionResult> DeleteProduct(int id)
        {
            if (id == 0)
            {
                return BadRequest("Id not Found");
            }

            var productToBeDeleted = await _productServices.GetProductById(id);

            if( productToBeDeleted == null)
            {
                return BadRequest("Bad Request");
            }
            await _productServices.DeleteProduct(id);

            return NoContent();
        }


    }
}
