using AutoMapper;
using Concepts.Domain;
using Concepts.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concepts.Services.Helpers
{
    public class AutoMapper: Profile
    {
        public AutoMapper()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
