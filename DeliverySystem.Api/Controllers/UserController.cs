using DeliverySystem.Data.Common;
using DeliverySystem.Data.Helpers;
using DeliverySystem.Data.Models.Views;
using DeliverySystem.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace DeliverySystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        #region Property Initialization
        private Authentication.Authorization _authorization;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IUserService _userService;
        private int LoggedInUserId
        {
            get
            {
                ClaimsPrincipal userClaims = this.User as ClaimsPrincipal;
                return _authorization.GetUserId(userClaims);
            }
        }
        #endregion

        #region 'Constructor'
        public UserController(IUserService userService)
        {
            _authorization = new Authentication.Authorization();
            _userService = userService;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }
        #endregion

        [HttpGet("profile")]
        public async Task<OperationResult<UserDto>> GetProfile()
        {
            var user = await _userService.FirstOrDefaultAsync(x => x.Id == LoggedInUserId);

            if (user == null)
                return new OperationResult<UserDto>(false, "User not found.");

            var result = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };

            return new OperationResult<UserDto>(true, "", result);
        }

        [HttpPut("profile-update")]
        public async Task<OperationResult<string>> UpdateProfile(UpdateProfileDto dto)
        {
            var user = await _userService.FirstOrDefaultAsync(x => x.Id == LoggedInUserId);

            if (user == null)
                return new OperationResult<string>(false, "User not found.");

            user.Name = dto.Name;

            _userService.UpdateAsync(user, user.Id);
            await _userService.SaveAsync();

            return new OperationResult<string>(true, "Profile updated successfully.");
        }

        [HttpPost("change-password")]
        public async Task<OperationResult<string>> ChangePassword(ChangePasswordDto dto)
        {
            var user = await _userService.FirstOrDefaultAsync(x => x.Id == LoggedInUserId);
            if (user == null) return new OperationResult<string>(false, "User not found.");

            var currentHash = ShaHash.Encrypt(dto.CurrentPassword);

            if (user.PasswordHash != currentHash)
                return new OperationResult<string>(false, "Current password is incorrect.");

            user.PasswordHash = ShaHash.Encrypt(dto.NewPassword);

            _userService.UpdateAsync(user, user.Id);
            await _userService.SaveAsync();

            return new OperationResult<string>(true, "Password changed successfully.");
        }
    }
}
