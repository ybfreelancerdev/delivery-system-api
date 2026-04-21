using DeliverySystem.Data.Common;
using DeliverySystem.Data.Helpers;
using DeliverySystem.Data.Models.Entities;
using DeliverySystem.Data.Models.Views;
using DeliverySystem.Data.Services;
using DeliverySystem.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace DeliverySystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region Property Initialization
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IUserService _userService;
        #endregion

        #region 'Constructor'
        public AuthController(IUserService userService)
        {
            _userService = userService;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }
        #endregion

        [HttpPost("Login")]
        public async Task<OperationResult<AuthResponseDto>> Login(LoginDto dto)
        {
            var user = await _userService.FirstOrDefaultAsync(x => x.Email.ToUpper() == dto.Email.ToUpper());
            if (user == null) return new OperationResult<AuthResponseDto>(false, "User not found.");

            var isValid = (user.PasswordHash == ShaHash.Encrypt(dto.Password));
            if (!isValid) return new OperationResult<AuthResponseDto>(false, "Incorrect password.");

            var token = new JwtTokenBuilder().GenerateToken(GetClaim(user), false);
            AuthResponseDto authResponse = new AuthResponseDto();
            authResponse.Token = token;
            authResponse.Email = dto.Email;
            authResponse.Role = user.Role;

            return new OperationResult<AuthResponseDto>(true, "Login successful.", authResponse);
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<OperationResult<string>> Register(RegisterDto dto)
        {
            var existing = await _userService.FirstOrDefaultAsync(x => x.Email.ToUpper() == dto.Email.ToUpper());
            if (existing != null) return new OperationResult<string>(false, "User already exists.");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = ShaHash.Encrypt(dto.Password),
                Role = "User"
            };

            _userService.Add(user);
            await _userService.SaveAsync();
            return new OperationResult<string>(true, "User registered successfully.");
        }

        [HttpPost("forgot-password")]
        public async Task<OperationResult<string>> ForgotPassword(ForgotPasswordDto dto)
        {
            var user = await _userService.FirstOrDefaultAsync(x => x.Email.ToUpper() == dto.Email.ToUpper());

            if (user == null)
                return new OperationResult<string>(false, "User not found.");

            // Random OTP Generate Function
            Random generator = new Random();
            String randomOTP = generator.Next(0, 1000000).ToString("D6");

            user.Otp = randomOTP;
            _userService.UpdateAsync(user, user.Id);
            await _userService.SaveAsync();

            // TODO: send email with otp
            // e.g. https://yourapp/reset-password?token=xxx

            return new OperationResult<string>(true, "Otp is sent to email, Please verify it.");
        }

        [HttpPost("reset-password")]
        public async Task<OperationResult<string>> ResetPassword(ResetPasswordDto dto)
        {
            var user = await _userService.FirstOrDefaultAsync(x =>
                x.Otp == dto.Otp &&
                x.Email.ToUpper() == dto.Email.ToUpper());

            if (user == null)
                return new OperationResult<string>(false, "Invalid otp.");

            user.PasswordHash = ShaHash.Encrypt(dto.NewPassword);

            _userService.UpdateAsync(user, user.Id);
            await _userService.SaveAsync();

            return new OperationResult<string>(true, "Password reset successful.");
        }

        #region 'Helper'
        private List<Claim> GetClaim(User obj)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim("sid", obj.Id.ToString()));
            claims.Add(new Claim("Email", obj.Email.ToString()));
            claims.Add(new Claim("Role", obj.Role.ToString()));
            claims.Add(new Claim("Name", obj.Name.ToString()));
            return claims;
        }

        #endregion
    }
}
