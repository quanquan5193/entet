using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mrs.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Devices.Commands.UpdateDevice
{
    public class UpdateDeviceCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public string CompanyCode { get; set; }

        public string StoreCode { get; set; }

        public bool DeviceStatus { get; set; }
        public bool IsAutoLock { get; set; }

        public string Lat { get; set; }

        public string Long { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

    public class UpdateDeviceHandler : IRequestHandler<UpdateDeviceCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        const string companyCodeChangeMessage = "変更内容にエラーがあります。";
        const string storeCodeChangeMessage = "入力内容を確認してください。";

        public UpdateDeviceHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
        {
            Store storeEntity = _context.Stores.Include(n => n.Company).FirstOrDefault(x => x.StoreCode.Equals(request.StoreCode) && x.Company.CompanyCode.Equals(request.CompanyCode));

            if (storeEntity == null)
            {
                throw new NotFoundException(storeCodeChangeMessage);
            }

            if (storeEntity.IsDeleted)
            {
                throw new EntityDeletedException("EntityDeleted");
            }

            if (!storeEntity.Company.CompanyCode.Equals(request.CompanyCode))
            {
                throw new NotFoundException(companyCodeChangeMessage);
            }

            if (storeEntity.Company.IsDeleted)
            {
                throw new EntityDeletedException("EntityDeleted");
            }

            var deviceEntity = _context.Devices.FirstOrDefault(x => x.Id == request.Id && !x.IsDeleted);

            if (deviceEntity == null)
            {
                throw new NotFoundException();
            }

            var requestUpdateAt = request.UpdatedAt.HasValue ? ((DateTime)request.UpdatedAt).ToString("F") : null;
            var databaseUpdateAt = deviceEntity.UpdatedAt.HasValue ? ((DateTime)deviceEntity.UpdatedAt).ToString("F") : null;

            if (requestUpdateAt != databaseUpdateAt)
            {
                throw new DataChangedException("DataChanged");
            }

            deviceEntity.StoreId = storeEntity.Id;
            deviceEntity.IsActive = request.DeviceStatus;
            deviceEntity.Lat = request.Lat;
            deviceEntity.Long = request.Long;
            deviceEntity.IsAutoLock = request.IsAutoLock;

            _context.Devices.Update(deviceEntity);
            await _context.SaveChangesAsync(cancellationToken);

            return await Task.FromResult(true);
        }
    }
}
