using Concepts.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concepts.Services.Helpers
{
    public enum RequestType
    {
        POST,
        PUT,
        DELETE
    }

    public class ResponseData
    {
        public RequestType RequestType { get; set; }
        public string Message { get; set; }
        public int? EntityId { get; set; }
        public UserDto? UserData { get; set; }
        public ProductDto? ProductData { get; set; }
        public bool? IsValid { get; set; }
    }
}
