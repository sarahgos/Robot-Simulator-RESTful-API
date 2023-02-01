﻿namespace robot_controller_api.Models
{
    public class Login
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public Login(string email, string password) 
        { 
            Email = email;
            Password = password;
        }
    }
}
