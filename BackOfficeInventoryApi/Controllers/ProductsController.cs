using BackOfficeInventoryApi.Data;
using BackOfficeInventoryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace BackOfficeInventoryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ToDoContext _context;
        
        public ProductsController(ToDoContext context)
        {
            _context = context;
        }


        [HttpGet]
        public ActionResult<IEnumerable<Products>> GetProducts()
        {

            var allProducts= _context.Products.ToList();
            return Ok(allProducts);

        }

        [HttpGet("{id}")]

        public ActionResult<Products> GetProduct(int id)
        {
            if(id == null)
            {
                return NotFound();
            }

            return _context.Products.Find(id);

        }

        [HttpPost]

        public ActionResult<Products> CreateProduct(Products products)
        {

            if (products == null || string.IsNullOrEmpty(products.Name))
            {
                return BadRequest("Invalid Product Data");
            }

            _context.Products.Add(products);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetProduct), new {id=products.Id},products);

        }

        [HttpPut]
        public ActionResult<Products> UpdateProduct(int id,Products updatedProduct)
        {
            if (id == 0 || updatedProduct == null || id != updatedProduct.Id)
            {
                return BadRequest("Invalid Id or Product data.");
            }

            var productToBeUpdated = _context.Products.Find(id);  // Find the product by id

            if (productToBeUpdated == null)
            {
                return NotFound("Product not found.");
            }

            productToBeUpdated.Name = updatedProduct.Name;
            productToBeUpdated.Description = updatedProduct.Description;
            productToBeUpdated.Quantity = updatedProduct.Quantity;


            _context.Products.Update(productToBeUpdated);
            _context.SaveChanges();

            return Ok(productToBeUpdated);
        }

        [HttpDelete]

        public ActionResult DeleteProduct(int id)
        {
            if (id == 0)
            {
                return BadRequest("Id not Found");
            }

            var productToBeDeleted = _context.Products.Find(id);

            _context.Products.Remove(productToBeDeleted);

            return NoContent();
        }


    }
}
