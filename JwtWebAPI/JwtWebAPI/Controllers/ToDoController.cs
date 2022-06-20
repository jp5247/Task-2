using JwtWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JwtWebAPI.Controllers
{
    [Route("[controller]/api/v1.0/")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        // Db context
        private readonly ToDoApiContext _context;

        public ToDoController(ToDoApiContext context)
        {
            _context = context;
        }

        [HttpGet("Public")]
        public IActionResult Public()
        {
            return Ok("Hello, Welcome to ToDo API with version 1.0");
        }

        // GET: <ToDoController>/api/v1.0/ToDos
        [HttpGet("ToDos")]
        [Authorize]
        [ResponseCache(Duration = 3600)]
        public ActionResult<IEnumerable<ToDo>> GetToDos()
        {
            var role = GetCurrentUser().Role;
            if (role == "Manager")
            {
                return _context.ToDo.ToList();
            }
            return _context.ToDo.Where(p => !p.Status).ToList();
        }

        // GET <ToDoController>/api/v1.0/ToDo/5
        [HttpGet("ToDo/{id}")]
        [Authorize]
        public ActionResult<ToDo> GetToDo(int id)
        {
            var toDo = _context.ToDo.Find(id);
            if (toDo == null)
            {
                return NotFound($"No ToDo for id {id} exists.");
            }
            else
            {
                return toDo;
            }
        }

        // POST <ToDoController>/api/v1.0/ToDo
        [HttpPost("ToDo")]
        [Authorize]
        public ActionResult PostToDo([FromBody] ToDo toDo)
        {

            _context.ToDo.Add(toDo);
            _context.SaveChanges();
            return CreatedAtAction("GetToDo", new { id = toDo.Id }, toDo);
        }

        // PUT <ToDoController>/api/ToDo/5
        [HttpPut("Todo/{id}")]
        [Authorize]
        public ActionResult PutToDo(int id, [FromBody] ToDo toDo)
        {
            if (id != toDo.Id)
                return BadRequest();

            _context.Entry(toDo).State = EntityState.Modified;
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToDoExists(toDo.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok("ToDo updated successfully!!");
        }

        // DELETE <ToDoController>/api/ToDO/5
        [HttpDelete("Todo/{id}")]
        [Authorize(Roles = "Manager")]
        public ActionResult DeleteToDo(int id)
        {
            if (!ToDoExists(id))
            {
                return NotFound();
            }
            else
            {
                var toDo = GetToDo(id);
                toDo.Value.Status = true;
                PutToDo(id, toDo.Value);
                return Ok("ToDo updated successfully!");
            }
        }

        private bool ToDoExists(int id)
        {
            return _context.ToDo.Any(x => x.Id == id);
        }

        private UserModel GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
                return null;

            var userClaims = identity.Claims;
            return new UserModel
            {
                UserName = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
                Email = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value,
                Role = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value,
            };
        }
    }
}
