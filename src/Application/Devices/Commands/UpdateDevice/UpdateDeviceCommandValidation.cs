using FluentValidation;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Devices.Commands.UpdateDevice
{
    public class UpdateDeviceCommandValidation : AbstractValidator<UpdateDeviceCommand>
    {
        private readonly IApplicationDbContext _context;
        public UpdateDeviceCommandValidation(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x)
                .MustAsync(CheckCompanyFlag).WithMessage("hiddenFlagCompany");
        }

        private async Task<bool> CheckCompanyFlag(UpdateDeviceCommand model, CancellationToken arg2)
        {
            var store = await _context.Stores.Include(n => n.Company).AnyAsync(x => x.StoreCode.Equals(model.StoreCode) && x.Company.CompanyCode.Equals(model.CompanyCode) && x.IsActive);
            return await _context.Companies.AnyAsync(x => x.CompanyCode.Equals(model.CompanyCode) && x.IsActive) && store;
        }
    }
}
