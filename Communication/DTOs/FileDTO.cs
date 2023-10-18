using Communication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.DTOs
{
    public class FileDTO
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string FileName { get; set; }
        public MUser? User { get; set; }
        public required string UserId { get; set; }
        public required string Type { get; set; }
    }
}
