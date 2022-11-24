using login.Data;
using login.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace login.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static UserContext _context;

        public UserController(UserContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x=>x.Id == id);
            if (user == null)
                return BadRequest("invalid Id");
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Post(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", user.Id, user);
        }

        [HttpPatch]
        public async Task<IActionResult> Patch(int id, string group)
        {

            var user = await _context.Users.FirstOrDefaultAsync(x =>x.Id == id);

            if (user == null)
                return BadRequest("Invalid Id");
            user.Group = group;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return BadRequest("Invalid Id");
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
