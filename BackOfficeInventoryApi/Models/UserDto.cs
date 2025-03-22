﻿namespace BackOfficeInventoryApi.Models
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHashed { get; set; } = string.Empty;
    }
}
