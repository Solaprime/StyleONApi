﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using StyleONApi.Entities;
using StyleONApi.Model;

// Because i names the folder name as profile and i wish to imherrit 
// from automapper profile, erroe is beint thrown beacusde it is mistaking 
// automapper profile as the profilr namespace 
// so i changed to profiles
namespace StyleONApi.Profiles
{
    public class ProductProfile  : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>();
            //CreateMap<Product, ProductForCreationDto>();
            CreateMap<ProductForCreationDto, Product>();
            CreateMap<ProductForUpdate, Product>();
            // one of them is for Patch sake
            CreateMap<Product, ProductForUpdate>();


            /// Flow to test ProductDto Test
            CreateMap<Product, ProductDtoTest>().
                ForMember(dest => dest.sellerInfo, sour => sour.MapFrom(s => s.Seller));

        }
    }
}
