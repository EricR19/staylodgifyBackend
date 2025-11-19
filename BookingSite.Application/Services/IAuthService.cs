using BookingSite.Application.DTOs;
using System.Threading.Tasks;

namespace BookingSite.Application.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
    }
}