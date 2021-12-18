using AutoMapper;
using mrs.Application.Common.Mappings;
using mrs.Domain.Entities;

namespace mrs.Application.Cards.Queries.GetCards
{
    public class MemberCardDto : IMapFrom<CardDto>
    {
        public string UserId { get; set; }
        public int CardId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Card, MemberCardDto>()
                .ForMember(d => d.CardId, opt => opt.MapFrom(s => s.Id));
        }
    }
}
