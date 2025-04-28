using efilling_api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace efilling_api.Services
{
    public class UserService
    {
        private readonly EFilling_DBContext _context;

        public UserService(EFilling_DBContext context)
        {
            _context = context;
        }

        public string GetEmailFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            if (jwtToken == null) throw new Exception("Invalid token.");

            // Extract the email claim
            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (emailClaim == null) throw new Exception("Email claim not found.");

            return emailClaim.Value;
        }

        public async Task<string> GetPasswordHashByEmailAsync(string email)
        {
            var user = await _context.UserResponses.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) throw new Exception("User not found.");

            return user.PasswordHash;
        }
    }

}


