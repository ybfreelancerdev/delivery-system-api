using System;
using System.Collections.Generic;
using System.Text;

namespace DeliverySystem.Data.Models.Views
{
    public class AuthDto
    {
    }
    public class LoginDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
    public class RegisterDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
