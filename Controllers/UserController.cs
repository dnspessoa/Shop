using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using Backoffice.Data;
//using Backoffice.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Shop.Models;
using Shop.Data;
using Shop.Services;
//using Backoffice.Services;

namespace Shop.Controllers
{
    [Route("users")]
    public class UserController : Controller
    {
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get
        (
            [FromServices] DataContext dataContext
        )
        {
            var users = await dataContext
                .Users
                .AsNoTracking()
                .ToListAsync();
            return users;
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Post(
            [FromServices] DataContext dataContext,
            [FromServices] User user
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                //Força o usuário a ser sempre "funcionário"
                user.Role = "emmployee";

                dataContext.Users.Add(user);
                await dataContext.SaveChangesAsync();
                
                //Esconde a senha
                user.Password = "";
                return user;
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o usuário" });
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate(
            [FromServices] DataContext dataContext,
            [FromBody] User modelUser
        )
        {
            var user = await dataContext.Users
                .AsNoTracking()
                .Where(x => x.Username == modelUser.Username && x.Password == modelUser.Password)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "Usuário ou senha inváldo " });
            }

            var token = TokenServices.GenerateToken(user);
            return new
            {
                user = user,
                token = token
            };
        }
    }
}