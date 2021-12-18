using AutoMapper;
using mrs.Application.Common.Interfaces;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System;
using mrs.Domain.Entities;
using CsvHelper;
using mrs.Application.ApplicationUser.Queries.GetToken;
using mrs.Domain.Enums;
using mrs.Application.Cards.Queries.GetCards;
using mrs.Application.Common.Exceptions;

namespace mrs.Application.Cards.Commands.DeleteCards
{
    public class DeleteCardsCommand : IRequest<Tuple<bool, List<string>>>
    {
        public CardDto[] CardsDelete { get; set; }
    }

    public class DeleteCardsCommandHandler : IRequestHandler<DeleteCardsCommand, Tuple<bool, List<string>>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteCardsCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Tuple<bool, List<string>>> Handle(DeleteCardsCommand request, CancellationToken cancellationToken)
        {
            List<Card> listCardValid = new List<Card>();
            List<Card> listCardFromDb = _context.Cards.Where(n => !n.IsDeleted).ToList();
            foreach (var card in request.CardsDelete)
            {
                Card item = listCardFromDb.FirstOrDefault(n => n.Id == card.Id);
                if (item == null || item.IsDeleted) throw new EntityDeletedException("EntityDeleted");
                listCardValid.Add(item);
            }

            _context.Cards.RemoveRange(listCardValid);
            await _context.SaveChangesAsync(cancellationToken);
            var listMemberNo = listCardValid.Select(x => x.MemberNo).ToList();
            return await Task.FromResult(new Tuple<bool,List<string>>(true,listMemberNo));
        }
    }
}
