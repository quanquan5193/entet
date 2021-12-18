using AutoMapper;
using mrs.Application.Common.Interfaces;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using mrs.Domain.Entities;
using mrs.Application.Common.Exceptions;
using mrs.Application.Kids.Queries.GetKidsWithPagination;
using System;

namespace mrs.Application.Cards.Commands.DeleteCards
{
    public class DeleteKidsCommand : IRequest<int[]>
    {
        public MemberKidDeleteCommandDto[] KidsDelete { get; set; }
    }

    public class DeleteKidsCommandHandler : IRequestHandler<DeleteKidsCommand, int[]>
    {
        private readonly IApplicationDbContext _context;

        public DeleteKidsCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int[]> Handle(DeleteKidsCommand request, CancellationToken cancellationToken)
        {
            List<MemberKid> listMemberKidValid = new List<MemberKid>();
            List<MemberKid> listMemberKidFromDB = _context.MemberKids.Where(n => !n.IsDeleted).ToList();
            foreach (var kid in request.KidsDelete)
            {
                var item = listMemberKidFromDB.FirstOrDefault(n => n.Id == kid.Id);
                if (item == null) throw new EntityDeletedException("EntityDeleted");
                if (item.UpdatedAt != null)
                {
                    DateTime dateServer = (DateTime)item.UpdatedAt;
                    DateTime dateClient = (DateTime)kid.UpdatedAt;
                    if (!dateServer.ToString("F").Equals(dateClient.ToString("F"))) throw new DataChangedException("DataChanged");
                }
                listMemberKidValid.Add(item);
            }

            _context.MemberKids.RemoveRange(listMemberKidValid);
            await _context.SaveChangesAsync(cancellationToken);
            var deletedIds = listMemberKidValid.Select(k => k.Id);
            return deletedIds.ToArray();
        }
    }
}
