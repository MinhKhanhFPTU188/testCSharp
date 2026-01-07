using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.DTO.Response
{
    public class AuthPayload
    {
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public string AccessToken { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }

}