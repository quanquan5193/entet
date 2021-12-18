using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Members.Queries.UpdateRemarkPIC
{
    public class UpdateRemarkPICQuery : IRequest<int>
    {
        public int RequestId { get; set; }
        public int PICStoreId { get; set; }
        public string Remark { get; set; }
    }

    public class UpdateRemarkPICQueryHandler : IRequestHandler<UpdateRemarkPICQuery, int>
    {
        private readonly IApplicationDbContext _context;

        public UpdateRemarkPICQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(UpdateRemarkPICQuery request, CancellationToken cancellationToken)
        {
            Member member = _context.Members.Include(c => c.MemberKids).FirstOrDefault(x => x.Id == request.RequestId);

            if (member == null)
            {
                throw new NotFoundException(nameof(Member), request.RequestId);
            }

            PICStore pICStore = _context.PICStores.FirstOrDefault(x => x.Id == request.PICStoreId && !x.IsDeleted);
            if(pICStore == null)
            {
                throw new NotFoundException(nameof(PICStore), request.PICStoreId);
            }    

            member.PICStoreId = request.PICStoreId;
            member.Remark = request.Remark;

            foreach (MemberKid item in member.MemberKids)
            {
                item.Remark = request.Remark;
            }

            _context.Members.Update(member);
            int result = await _context.SaveChangesAsync(cancellationToken);

            return member.Id;
        }
    }
}
