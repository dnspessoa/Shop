using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Shop.Controllers
{
    
    [Route("products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Product>>> Get(
            [FromServices] DataContext dataContext
        )
        {
            var products = await dataContext.Products
                .Include(p => p.Category)
                .AsNoTracking().ToListAsync();

            return Ok(products);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<Product>> GetById(
            [FromServices] DataContext dataContext,
            int id
        )
        {
            var product = await dataContext.Products
                .Include(c => c.Category)
                .AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

            return Ok(product);
        }


        [HttpGet] //products/categories/1
        [Route("categories/{id:int}")]
        public async Task<ActionResult<List<Product>>> GetByIdCategory(
            [FromServices] DataContext dataContext,
            int id
        )
        {
            var products = await dataContext.Products
                .Include(c => c.Category).AsNoTracking()
                .Where(c => c.CategoryId == id).ToListAsync();
            
            return Ok(products);
        }

        public async Task<ActionResult<Product>> Post(
            [FromServices] DataContext dataContext,
            [FromBody] Product product
        )
        {
            if (ModelState.IsValid)
            {
                dataContext.Products.Add(product);
                await dataContext.SaveChangesAsync();
                return Ok(product);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}