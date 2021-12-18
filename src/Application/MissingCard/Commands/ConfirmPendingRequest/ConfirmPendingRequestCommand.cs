using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.MissingCard.Commands.ConfirmPendingRequest
{
    public class ConfirmPendingRequestCommand : IRequest<bool>
    {
        public int PendingId { get; set; }

        public string OldMemberNo { get; set; }

        public DateTime ReceiptedDatetime { get; set; }

        public int? StoreId { get; set; }

        public int? DeviceId { get; set; }

        public int? PICStoreId { get; set; }
    }

    public class ConfirmPendingCardCommandHandler : IRequestHandler<ConfirmPendingRequestCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ConfirmPendingCardCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> Handle(ConfirmPendingRequestCommand request, CancellationToken cancellationToken)
        {
            var requestPendingEntity = _context.RequestsPendings.Include(x => x.Member).ThenInclude(x => x.RequestsReceipteds).FirstOrDefault(x => x.Id == request.PendingId && !x.IsDeleted);

            if (requestPendingEntity == null)
            {
                throw new NotFoundException(nameof(RequestsPending), request.PendingId);
            }

            if (!ValidCard(request.OldMemberNo))
            {
                return false;
            }

            requestPendingEntity.Member.OldMemberNo = request.OldMemberNo;

            Card cardEntity = _context.Cards.FirstOrDefault(n => n.MemberNo == requestPendingEntity.Member.MemberNo);
            if (cardEntity == null)
            {
                throw new NotFoundException(nameof(Card), requestPendingEntity.Member.MemberNo);
            }

            RequestsReceipted requestsReceiptedEntity = new RequestsReceipted();
            requestsReceiptedEntity.ReceiptedDatetime = requestPendingEntity.ReceiptedDatetime;
            requestsReceiptedEntity.ReceiptedTypeId = (int)RequestTypeEnum.ReIssued;
            requestsReceiptedEntity.DeviceId = requestPendingEntity.DeviceId;
            requestsReceiptedEntity.CardId = cardEntity.Id;
            requestsReceiptedEntity.RequestCode = requestPendingEntity.RequestCode;
            requestsReceiptedEntity.StoreId = requestPendingEntity.StoreId;
            requestsReceiptedEntity.Member = requestPendingEntity.Member;

            var existRequestReceiped = requestPendingEntity.Member.RequestsReceipteds.FirstOrDefault();
            _context.RequestsReceipteds.Remove(existRequestReceiped);

            _context.RequestsReceipteds.Add(requestsReceiptedEntity);
            _context.RequestsPendings.Remove(requestPendingEntity);
           
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private bool ValidCard(string memberNo)
        {
            Card cardEntity = _context.Cards.FirstOrDefault(x => x.MemberNo == memberNo && !x.IsDeleted);
            if (cardEntity == null)
            {
                return true;
            }
            else if (cardEntity.Status != CardStatus.Unissued)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
