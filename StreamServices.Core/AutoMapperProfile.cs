using AutoMapper;
using StreamServices.Core;
using StreamServices.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamServices.Core
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Subscription, SubscriptionDto>()
                .ForMember(d => d.BroadcasterUserId, opt => opt.MapFrom(src => src.Condition.BroadcasterUserId));
        }
    }
}
