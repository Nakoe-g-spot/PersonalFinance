﻿using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.API.DTOs
{
    public class RegisterRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;  // Lưu ý: giá trị này sẽ được hash lại sau.
    }
}