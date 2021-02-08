using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Shop.Controllers
{
    //Endpoint => URL
    //http://localhost:5000
    //https://localhost:5001 // atual
    //https://meuapp.azurewebsites.net/

    [Route("categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Category>>> Get(
            [FromServices] DataContext dataContext
        )
        {
            var categorias = await dataContext.Categories.AsNoTracking().ToListAsync();

            return Ok(categorias);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<Category>> GetById(
            [FromServices] DataContext dataContext,
            int id)
        {
            var categoria = await dataContext.Categories.AsNoTracking().FirstOrDefaultAsync();

            return Ok(categoria);
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<List<Category>>> Post(
            [FromServices] DataContext dataContext,
            [FromBody] Category category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                dataContext.Categories.Add(category);
                await dataContext.SaveChangesAsync();
                return Ok(category);    
            }
            catch (System.Exception)
            {
                return BadRequest(new {message = "Não foi possível criar a categoria"});
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult<List<Category>>> Put(
            [FromServices] DataContext dataContext,
            int id, 
            [FromBody] Category category)
        {
            // Verifica se o ID informadado é o memso do modelo
            if (id != category.Id)
            {
                return NotFound(new { message = "Categoria não encontrada" });
            }

            // Verifica se os dados são válidos
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                dataContext.Entry<Category>(category).State = EntityState.Modified;
                await dataContext.SaveChangesAsync();
                return Ok(category);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                return BadRequest(new {message = "Este  registro já foi atualizado"});
            }
            catch (Exception exception)
            {
                return BadRequest(new {message = "Não foi possível atualizar a categoria"});
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult<List<Category>>> Delete(
            [FromServices] DataContext dataContext,
            int id
        )
        {
            var categoria = await dataContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (categoria == null)
            {
                return NotFound(new {message = "Categoria não encontrada"});
            }

            try
            {
                dataContext.Categories.Remove(categoria);
                await dataContext.SaveChangesAsync();
                return Ok(new {message = "Categoria removida com sucesso"});
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possível remover a categoria"});
            }
        }
    }
}