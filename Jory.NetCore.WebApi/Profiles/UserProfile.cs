using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Jory.NetCore.Model.Entities;

namespace Jory.NetCore.WebApi.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ADUserT, ADUserT>();
        }
    }
}
