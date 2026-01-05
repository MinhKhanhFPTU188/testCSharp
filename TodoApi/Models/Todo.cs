using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class Todo
    {
        public string Id { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}