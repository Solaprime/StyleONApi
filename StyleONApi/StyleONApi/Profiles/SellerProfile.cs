using AutoMapper;
using StyleONApi.Entities;
using StyleONApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Profiles
{
    public class SellerProfile : Profile
    {
        public SellerProfile()
        {
            CreateMap<Seller, SellerDto>().
                ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateMap<SellerForCreationDto, Seller>();
            //One of them is for Patch
            CreateMap<SellerForUpdate, Seller>();
            CreateMap<Seller, SellerForUpdate>();
        }
    }
}
