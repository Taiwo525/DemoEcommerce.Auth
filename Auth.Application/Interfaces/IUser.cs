using Auth.Application.Dtos;
using Ecommerce.SharedLibrary.Common;

namespace Auth.Application.Interfaces
{
    public interface IUser
    {
        Task<Response> Register(AppUserDto appUserDto);
        Task<Response> Login(LoginDto loginDto);
        Task<GetUserDto> GetUser(int id);
    }
}
