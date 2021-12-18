using AutoMapper;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Cards.Queries.GetCards;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Cards.Commands.UpdateCards
{
    public class UpdateCardsCommand : IRequest<Tuple<bool, string[]>>
    {
        public CardDto[] CardsUpdate { get; set; }
    }

    public class EditCardsCommandHandler : IRequestHandler<UpdateCardsCommand, Tuple<bool, string[]>>
    {
        private readonly IApplicationDbContext _context;

        public EditCardsCommandHandler(IApplicationDbContext context, IMapper mapper, ICsvFileBuilder fileBuilder)
        {
            _context = context;
        }

        public async Task<Tuple<bool, string[]>> Handle(UpdateCardsCommand request, CancellationToken cancellationToken)
        {
            List<Card> listCardValid = new List<Card>();
            List<Card> listCardFromDb = _context.Cards.Include(x => x.Store).Include(x => x.Company).Where(n => !n.IsDeleted).ToList();
            foreach (var card in request.CardsUpdate)
            {
                Card item = listCardFromDb.FirstOrDefault(n => n.Id == card.Id);
                if (item == null) throw new EntityDeletedException("EntityDeleted");
                if (item.UpdatedAt != null)
                {
                    DateTime dateServer = (DateTime)item.UpdatedAt;
                    DateTime dateClient = (DateTime)card.UpdatedAt;
                    if (!dateServer.ToString("F").Equals(dateClient.ToString("F"))) throw new DataChangedException("DataChanged");
                }
                if (!ValidateCardStatus(card.Status, item.Status)) throw new ValidationException(new List<ValidationFailure>() { new ValidationFailure("Status", "updateFailStatusInvalid") });

                if (card.StoreId.HasValue && card.CompanyId.HasValue)
                {
                    Store store = await _context.Stores.FindAsync(card.StoreId.Value);
                    if (store.CompanyId != card.CompanyId)
                    {
                        throw new ValidationException(new List<ValidationFailure>() { new ValidationFailure("Store", "dataChangedStore") });
                    }
                }

                if (!item.Store.IsActive || !item.Company.IsActive)
                {
                    throw new ValidationException(new List<ValidationFailure>() { new ValidationFailure("Store", "dataChangedFlag") });
                }
                listCardValid.Add(item);
            }
            foreach (var card in request.CardsUpdate)
            {
                Card item = listCardValid.FirstOrDefault(n => n.Id == card.Id);
                if (item != null)
                {
                    item.CompanyId = card.CompanyId;
                    item.StoreId = card.StoreId;
                    item.Status = card.Status;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Tuple.Create(true, listCardValid.Select(c => c.MemberNo).ToArray());
        }

        private bool ValidateCardStatus (CardStatus destination, CardStatus source)
        {
            if (source == destination) return true;
            if (source == CardStatus.Issued && destination != CardStatus.Disposal) return false;
            if (source == CardStatus.Withdrawal && destination != CardStatus.Disposal) return false;
            if (source == CardStatus.Missing && destination != CardStatus.Disposal) return false;
            if (source == CardStatus.Disposal && destination != CardStatus.Disposal) return false;
            return true;
        }
    }
}
