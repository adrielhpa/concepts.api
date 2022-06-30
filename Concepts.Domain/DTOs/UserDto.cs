using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concepts.Domain.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Adress { get; set; }
        public string City { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
