using VetApp.Core;
using VetApp.Core.Filters;
using VetApp.DTO;
using VetApp.Models;

namespace VetApp.Services
{
    public interface IUserService
    {
        Task<User> VerifyAndGetUserAsync(UserLoginDTO credentials);
        Task<UserReadOnlyDTO> GetUserByUsernameAsync(string username);
        Task<UserReadOnlyDTO> GetUserByIdAsync(int id);
        Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedUsersFilteredAsync(int pageNumber,
            int pageSize, UserFiltersDTO userFiltersDTO);
        string CreateUserToken(User user);
    }
}
