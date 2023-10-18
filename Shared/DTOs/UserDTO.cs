using Microsoft.AspNetCore.Identity;
using Communication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.DTOs
{
    public class UserDTO
    {
        public virtual string Id { get; set; } = default!;
        public virtual string? UserName { get; set; }
        public virtual string? Email { get; set; }
        public virtual string? Name { get; set; }
        public List<MFile>? Files { get; set; }
    }
}
