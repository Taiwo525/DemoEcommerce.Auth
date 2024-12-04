﻿namespace Auth.Application.Dtos
{
    public class AppUserDto
    {
        //public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public DateTime DateRegister { get; set; } = DateTime.UtcNow;

    }
}
