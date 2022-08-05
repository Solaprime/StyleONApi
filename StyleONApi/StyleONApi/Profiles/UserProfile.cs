using AutoMapper;
using StyleONApi.Entities;
using StyleONApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserDto>().
                ForMember(dest => dest.FullName, opt=>opt.MapFrom(src=> $"{src.FirstName} {src.LastName}"));
        }
    }

}
