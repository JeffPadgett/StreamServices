using AutoMapper;
using StreamServices.Core.Extensions;
using StreamServices.Dto;
using System;

namespace StreamServices.Core
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Subscription, SubscriptionDto>()
                .ForMember(d => d.BroadcasterUserId, opt => opt.MapFrom(src => src.Condition.BroadcasterUserId ?? src.Condition.FromBroadcasterUserId.NullIfEmpty() ?? src.Condition.ToBroadcasterUserId))
                .ForMember(d => d.IsFromRaid, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Condition.FromBroadcasterUserId) ? false : true));
        }
    }
}
