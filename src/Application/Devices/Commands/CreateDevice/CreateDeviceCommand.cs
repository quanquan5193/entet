using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Devices.Commands.CreateDevice
{
    public class CreateDeviceCommand : IRequest<int>
    {
        public string DeviceCode { get; set; }

        public string CompanyCode { get; set; }

        public string StoreCode { get; set; }

        public bool DeviceStatus { get; set; }
        public bool IsAutoLock { get; set; }

        public string Lat { get; set; }

        public string Long { get; set; }
    }

    public class CreateDeviceCommandHandler : IRequestHandler<CreateDeviceCommand, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        const string companyCodeChangeMessage = "変更内容にエラーがあります。";
        const string storeCodeChangeMessage = "入力内容を確認してください。";

        public CreateDeviceCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateDeviceCommand request, CancellationToken cancellationToken)
        {
            Store storeEntity = _context.Stores.Include(n => n.Company).FirstOrDefault(x => x.StoreCode.Equals(request.StoreCode) && x.Company.CompanyCode.Equals(request.CompanyCode));

            if (storeEntity == null)
            {
                throw new NotFoundException(storeCodeChangeMessage);
            }

            if (storeEntity.IsDeleted || !storeEntity.IsActive)
            {
                throw new EntityDeletedException("EntityDeleted");
            }

            if (!storeEntity.Company.CompanyCode.Equals(request.CompanyCode))
            {
                throw new NotFoundException(companyCodeChangeMessage);
            }

            if (storeEntity.Company.IsDeleted || !storeEntity.IsActive)
            {
                throw new EntityDeletedException("EntityDeleted");
            }

            var isExistDeviceCode = _context.Devices.Any(x => x.DeviceCode.Equals(request.DeviceCode));

            if (isExistDeviceCode)
            {
                return -1;
            }

            var deviceEntity = new Device();
            deviceEntity.DeviceCode = request.DeviceCode;
            deviceEntity.StoreId = storeEntity.Id;
            deviceEntity.IsActive = request.DeviceStatus;
            deviceEntity.Lat = request.Lat;
            deviceEntity.Long = request.Long;
            deviceEntity.IsAutoLock = request.IsAutoLock;

            _context.Devices.Add(deviceEntity);
            await _context.SaveChangesAsync(cancellationToken);

            return deviceEntity.Id;
        }
    }
}
