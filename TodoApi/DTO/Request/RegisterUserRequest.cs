using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.DTO.Request
{
    public class RegisterUserRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

}