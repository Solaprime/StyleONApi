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

          


            // CreateMap<SellerDto, Seller>();


            CreateMap<DispatchForUpdateDto, Dispatch>();
            CreateMap<OrderDto, Order>();
            CreateMap<Order, OrderDtoToReturn>();
            CreateMap<OrderForUpdate, Order>();
          

        }
    }
}
