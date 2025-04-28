using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using efilling_api.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace efilling_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        //DI of DB Context
        private readonly EFilling_DBContext _context;
        public UsersController(EFilling_DBContext context)
        {
            _context = context;
        }

        // 1
        // Signup : add/create user
        // POST: api/Users/signup
        [HttpPost]
        [Route("signup")]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'EFilling_DBContext.Users'  is null.");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // 2
        // check login details entered (or get user by id to show user data)
        // GET: api/Users/login/{id}/{password}
        [HttpGet("login/{id}/{password}")]
        public async Task<ActionResult<User>> GetUser(int id, string password)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            
            var user = await _context.Users.Where(i => (i.Id == id) && (i.PasswordEncrypted == password)).FirstOrDefaultAsync();

            if (!UserExists(id))
            {
                return NotFound("No user found with the given id");
            }
            else if ( user == null)
            {
                return NotFound("No user found with the given data");
            }
            else if (user.PasswordEncrypted != password) 
            {
                return BadRequest("Wrong password, please try again!");
            }
            
            return Ok(user);
        
        }

        // 3
        // update user - save button in "Edit User Page"
        // PUT: api/Users/edituser/{id}
        [HttpPut("edituser/{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            //if the id int the route path is not matching the id in the body request (user)
            if (id != user.Id)
            {
                return BadRequest();
            }

            //update data of user
            //Modified entities are updated in the database and then become Unchanged when SaveChanges returns.
            _context.Entry(user).State = EntityState.Modified;

            // save updates 
            try
            {
                await _context.SaveChangesAsync();
                return Ok();

            }
            //handling exceptions
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    // re-throwing an exception(without an exception parameter) to pass an exception to the caller to handle it in a way they want.
                    throw;
                }
            }
        }

        // a function that checks if the given id is matchign to a user in the DB
        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }



        // For testing purposes *(get all users)
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            return await _context.Users.ToListAsync();
        }

        // 4
        // "Change Password" = "update only one value in User" = "change email"
        // PATCH: api/Users/{id}
        [HttpPatch("PatchUser/{id:int}")]
        public IActionResult PatchUser(int id, [FromBody] JsonPatchDocument<User> patchUser )
        {
            var updatedUser = _context.Users.FirstOrDefault(user => user.Id == id);

            if (updatedUser == null)
            {
                return NotFound();
            }

            patchUser.ApplyTo(updatedUser, ModelState);

            _context.SaveChanges();

            return Ok(updatedUser);
        }

        // 5
        // "Change Password" in case of "change password page" that **checks the old password**
        [HttpPatch("PatchUserPassword/{userId:int}")]
        public IActionResult PatchUserPassword(int userId, [FromBody] ChangePasswordRequest changePasswordRequest)
        {
            // Check if the user ID is valid
            if (!UserExists(userId))
            {
                return BadRequest("Invalid user ID.");
            }

            // Get the user from the data source
            var user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();

            // Check if the old password is correct
            if (!IsCorrectPassword(user, changePasswordRequest.OldPassword))
            {
                return BadRequest("The old password is incorrect.");
            }

            // Change the user's password
            user.PasswordEncrypted = changePasswordRequest.NewPassword;

            // Update the user in the data source
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();

            // Return a success response
            return Ok("Password changed successfully.");
        }

        // a function that checks if the old password entered by user is correct or not
        private bool IsCorrectPassword(User user, string oldPassword)
        {
            if ( (user == null) || (oldPassword == null) )
            {
                return false;
            }
            else
            {
                if (user.PasswordEncrypted == oldPassword) 
                { 
                    //return Ok("the old password is correct"); 
                    return true;
                }
                return false;
            }
        }



        // ------------------- User Roles ---------------------

        // 1 : add/create user role
        // POST: api/Users/AddUserRole
        [HttpPost]
        [Route("AddUserRole")]
        public async Task<ActionResult> PostUserRole(UserRole userRole)
        {
            if (userRole == null)
            {
                return BadRequest("No user role to add");
            }

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return Ok(userRole);
        }


        // 2 : update user role
        // PUT: api/Users/UpdateUserRole/{id}
        [HttpPut]
        [Route("UpdateUserRole/{id}")]
        public async Task<ActionResult> PutUserRole(int id, UserRole userRole)
        {
           if (id != userRole.id)
            {
                BadRequest("id of userRole is not the same");
            }

            _context.Entry(userRole).State = EntityState.Modified;

            // save updates 
            try
            {
                await _context.SaveChangesAsync();
                return Ok("userRole Updated");

            }

            //handling exceptions
            catch (DbUpdateConcurrencyException)
            {
                if (!UserRoleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    // re-throwing an exception(without an exception parameter) to pass an exception to the caller to handle it in a way they want.
                    throw;
                }
            }
        }

        // a function that checks if the given user role id is matching to one in the DB
        private bool UserRoleExists(int id)
        {
            return (_context.UserRoles?.Any(e => e.id == id)).GetValueOrDefault();
        }


        // 3 : get all user roles
        // GET: api/Users/UserRoles
        [HttpGet]
        [Route("UserRoles")]
        public async Task<ActionResult<IEnumerable<UserRole>>> GetUserRoles()
        {
            if (_context.UserRoles == null)
            {
                return NotFound();
            }

            return await _context.UserRoles.ToListAsync();
        }



        // 4 : get user role by userRoleId
        // GET: api/Users/UserRoleByUserRoleId/{userRoleId}
        [HttpGet]
        [Route("UserRoleByUserRoleId/{userRoleId}")]
        public async Task<ActionResult> GetUserRole(int userRoleId)
        {
            if (_context.UserRoles == null)
            {
                return NotFound();
            }

            var userRole = await _context.UserRoles.Where(u => u.id == userRoleId).FirstOrDefaultAsync();
            
            if (userRole == null)
            {
                return NotFound();
            }

            return Ok(userRole);
        }


        // 5 : get user roles by userId
        // GET: api/Users/UserRolesByUserId/{userId}
        [HttpGet]
        [Route("UserRolesByUserId/{userId}")]
        public async Task<ActionResult<IEnumerable<UserRole>>> GetUserRolesByUserId(int userId)
        {
            if (_context.UserRoles == null)
            {
                return NotFound();
            }
            
            var userRoles = await _context.UserRoles.Where(u => u.UserId == userId).ToListAsync();

            
            if (userRoles.Count == 0)
            {
                return NotFound("No user roles found for this user");
            }

            return userRoles;
        }

    }







    //-- define a custom request object called "ChangePasswordRequest"
    public class ChangePasswordRequest
    {
        public ChangePasswordRequest(string oldPassword, string newPassword)
        {
            OldPassword = oldPassword;
            NewPassword = newPassword;
        }

        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
