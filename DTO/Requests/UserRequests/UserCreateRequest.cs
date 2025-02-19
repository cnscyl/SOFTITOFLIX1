﻿using System.ComponentModel.DataAnnotations;

namespace SOFTITOFLIX.DTO.Requests
{
    public class UserCreateRequest
    {
        [StringLength(100, MinimumLength = 2)]
        [Required]
        public string Name { get; set; } = null!;

        [EmailAddress]
        [StringLength(100, MinimumLength = 5)]
        [Required]
        public string Email { get; set; } = null!;

        [Phone]
        [StringLength(30)]
        public string? PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public string Password { get; set; } = null!;
    }
}
