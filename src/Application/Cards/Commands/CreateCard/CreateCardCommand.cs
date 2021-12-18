using MediatR;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Cards.Commands.CreateCard
{
    public class CreateCardCommand : IRequest<int>
    {
        public string MemberNo { get; set; }
        public bool IsActive { get; set; }
        public bool IsBlock { get; set; }
        public string Point { get; set; }
    }

    public class CreateCardCommandHandler : IRequestHandler<CreateCardCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreateCardCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateCardCommand request, CancellationToken cancellationToken)
        {
            var entity = new Card()
            {
                MemberNo = request.MemberNo,
                Point = request.Point
            };

            _context.Cards.Add(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}
