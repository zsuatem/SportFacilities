using System.Threading.Tasks;
using SportFacilities.Contracts;

namespace SportFacilities.Frontend.Services
{
    public interface IProfileService
    {
        Task<UserProfileDto> GetProfileAsync();
        Task<ApiResponse> ChangePasswordAsync(ChangePasswordDto dto);
    }
}
