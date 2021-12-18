using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using mrs.Application.PICStores.Queries.GetPICStoresWithPagination;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.ReferPrepaidCard.Commands.GetPrepaidCardDetail
{
    public class GetPrepaidCardDetailCommand : IRequest<PrepaidCardDetailDto>
    {
        public int Id { get; set; }
    }

    public class GetPrepaidCardDetailCommandHandler : IRequestHandler<GetPrepaidCardDetailCommand, PrepaidCardDetailDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetPrepaidCardDetailCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PrepaidCardDetailDto> Handle(GetPrepaidCardDetailCommand request, CancellationToken cancellationToken)
        {
            var rawQuery = from card in _context.Cards
                           where card.Id == request.Id && !card.IsDeleted
                           select new { 
                               Card = card,
                               RequestsReceipted = _context.RequestsReceipteds.Where(x => x.CardId == card.Id).OrderByDescending(c => c.Id).Include(x => x.Member).Include(x => x.Member.PICStore).FirstOrDefault()
                           };

            var entity = rawQuery.FirstOrDefault();

            if (entity == null || entity.Card == null)
                throw new NotFoundException("Card", request.Id);


            var prepaidCardDetail = new PrepaidCardDetailDto();

            prepaidCardDetail.Id = entity.Card.Id;
            prepaidCardDetail.CustomerNo = entity.Card.MemberNo;
            prepaidCardDetail.RegisteredDate = entity.Card.CreatedAt;
            prepaidCardDetail.ExpiratedAt = entity.Card.ExpiredAt;
            prepaidCardDetail.Status = ((CardStatus)entity.Card.Status).GetStringValue();
            prepaidCardDetail.RequestCode = entity.RequestsReceipted?.RequestCode;
            prepaidCardDetail.ReceiptedDatetime = entity.RequestsReceipted?.ReceiptedDatetime;
            prepaidCardDetail.PicStore = entity.RequestsReceipted?.Member?.PICStoreId;
            if (entity.RequestsReceipted?.Member != null && entity.RequestsReceipted?.Member.PICStore != null)
            {
                var pic = entity.RequestsReceipted?.Member.PICStore;
                prepaidCardDetail.PicStoreDto = new PICStoreDto
                {
                    Id = pic.Id,
                    Index = 1,
                    PICCode = pic.PICCode,
                    RegistrationDate = pic.CreatedAt,
                    Company = "",
                    NormalizedCompanyName = "",
                    Store = "",
                    NormalizedStoreName = "",
                    PICName = pic.PICName
                };
            }

            prepaidCardDetail.RequestType = (entity.RequestsReceipted != null && Enum.IsDefined(typeof(RequestTypeEnum), entity.RequestsReceipted?.ReceiptedTypeId)) ? ((RequestTypeEnum)entity.RequestsReceipted?.ReceiptedTypeId).GetStringValue() : null;

            prepaidCardDetail.Remark = entity.RequestsReceipted?.Member?.Remark;

            return prepaidCardDetail;
        }
    }
}
