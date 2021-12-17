using AutoMapper;
using StreamServices.Core.Extensions;
using StreamServices.Dto;

namespace StreamServices.Core
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //if they equal null or an empty string. 
            CreateMap<Subscription, SubscriptionDto>()
                .ForMember(d => d.BroadcasterUserId, opt => opt.MapFrom(src => src.Condition.BroadcasterUserId ?? src.Condition.FromBroadcasterUserId.NullIfEmpty() ?? src.Condition.ToBroadcasterUserId));
        }
    }
}
