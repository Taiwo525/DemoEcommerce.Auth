using Auth.Application.Dtos;
using Auth.Application.Interfaces;
using Auth.Core.Entities;
using Auth.Infrastructure.Persistence;
using Ecommerce.SharedLibrary.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.Infrastructure.Repositories
{
    public class UserRepo : IUser
    {
        private readonly AuthDbContext _context;
        private readonly IConfiguration _config;
        public UserRepo(AuthDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        private async Task<AppUser> GetUserByEmail(string email)
        {
            var user = await _context.AppUsers.FirstOrDefaultAsync(x => x.Email == email);
            return user is null ? null! : user;
        }
        public async Task<GetUserDto> GetUser(int id)
        {
            var user = await _context.AppUsers.FindAsync(id);
            return user is not null ? new GetUserDto(
                user.Email,
                user.Name,
                user.PhoneNumber,
                user.Address,
                user.Role) : null!;
        }

        public async Task<Response> Login(LoginDto loginDto)
        {
            var user = await GetUserByEmail(loginDto.Email);
            if (user is null) return new Response(false, "Email does not exist");

            bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);
            if (!verifyPassword) return new Response(false, "Invalid credentials");

            string token = GenerateToken(user);
            return new Response(true, token);
        }

        private string GenerateToken(AppUser user)
        {
            var claims = new[]
            {
                new Claim (ClaimTypes.NameIdentifier, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Auth:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: _config["Auth:Issuer"],
                audience: _config["Auth:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: credentials

            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public async Task<Response> Register(AppUserDto appUserDto)
        {
            var user = await GetUserByEmail(appUserDto.Email);
            if (user is not null) return new Response(false, "User already exist");

            var newUser = new AppUser
               { 
                Name = appUserDto.Name,
                Email = appUserDto.Email,
                Address = appUserDto.Address,
                Password = BCrypt.Net.BCrypt.HashPassword(appUserDto.Password),
                PhoneNumber = appUserDto.PhoneNumber,
                Role = appUserDto.Role
                };
           
            var result = await _context.AppUsers.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return result is not null ? new Response(true, "Registration is successful") :
                new Response(false, "Invalid data provided");
        }
    }
}
