using System;
using System.Collections.Generic;
using System.Text;

namespace DeliverySystem.Data.Models.Views
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = "User";
    }
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
    public class ForgotPasswordDto
    {
        public string Email { get; set; }
    }
    public class ResetPasswordDto
    {
        public string Otp { get; set; }
        public string NewPassword { get; set; }
        public string Email { get; set; }
    }
    public class UpdateProfileDto
    {
        public string Name { get; set; }
    }
}
