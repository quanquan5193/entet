using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.InquiryKidClub.Commands.GetInquiryKidClubDetail
{
    public class GetInquiryKidClubDetailCommand : IRequest<InquiryKidClubDetailDto>
    {
        public int Id { get; set; }
    }

    public class GetInquiryKidClubDetailHandler : IRequestHandler<GetInquiryKidClubDetailCommand, InquiryKidClubDetailDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetInquiryKidClubDetailHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<InquiryKidClubDetailDto> Handle(GetInquiryKidClubDetailCommand request, CancellationToken cancellationToken)
        {
            var kidEntity = _context.MemberKids.Include(x => x.Member.RequestsReceipteds).FirstOrDefault(x => x.Id == request.Id);

            var inquiryKidClubDetailDto = new InquiryKidClubDetailDto();
            inquiryKidClubDetailDto.ReceiptedDatetime = kidEntity.Member?.RequestsReceipteds?.LastOrDefault()?.ReceiptedDatetime;
            inquiryKidClubDetailDto.RelationshipMember = kidEntity.RelationshipMember;
            inquiryKidClubDetailDto.GuardianFirstName = kidEntity.ParentFirstName;
            inquiryKidClubDetailDto.GuardianLastName = kidEntity.ParentLastName;
            inquiryKidClubDetailDto.GuardianFuriganaFirstName = kidEntity.ParentFuriganaFirstName;
            inquiryKidClubDetailDto.GuardianFuriganaLastName = kidEntity.ParentFuriganaLastName;
            inquiryKidClubDetailDto.IsLivingWithParent = kidEntity.IsLivingWithParent;
            inquiryKidClubDetailDto.FirstName = kidEntity.FirstName;
            inquiryKidClubDetailDto.LastName = kidEntity.LastName;
            inquiryKidClubDetailDto.FuriganaFirstName = kidEntity.FuriganaFirstName;
            inquiryKidClubDetailDto.FuriganaLastName = kidEntity.FuriganaLastName;
            inquiryKidClubDetailDto.DateOfBirth = kidEntity.DateOfBirth;
            inquiryKidClubDetailDto.Remarks = kidEntity.Member?.Remark;
            inquiryKidClubDetailDto.KidGender = kidEntity.Sex == (int)SexType.Male ? "男" : (kidEntity.Sex == (int)SexType.Female ? "女" : "その他");

            return inquiryKidClubDetailDto;
        }
    }
}
