using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Helpers.AzureKeyVaults;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.RequestsReceipteds.Queries.ExportRequestReceipted
{
    public class ExportRequestReceiptedQuery : IRequest<ExportFileVm>
    {
        public string MemberNo { get; set; }
        public int? RequestType { get; set; }
        public string Company { get; set; }
        public string Store { get; set; }
        public string Device { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string CreatedBy { get; set; }
        public bool IsMobileRequest { get; set; }
        public int? PICStoreId { get; set; }
    }

    public class ExportRequestReceiptedProfile : Profile
    {
        public ExportRequestReceiptedProfile()
        {
            CreateMap<RequestsReceipted, RequestReceiptedRecord>()
            .ForMember(a => a.RequestType, b => b.MapFrom(c => c.ReceiptedTypeId == (int)RequestTypeEnum.New ? "新規" : (c.ReceiptedTypeId == (int)RequestTypeEnum.Switch ? "切替" : (c.ReceiptedTypeId == (int)RequestTypeEnum.ReIssued ? "再発行" : (c.ReceiptedTypeId == (int)RequestTypeEnum.ChangeCard ? "変更" : (c.ReceiptedTypeId == (int)RequestTypeEnum.LeaveGroup ? "退会" : (c.ReceiptedTypeId == (int)RequestTypeEnum.PMigrate ? "Ｐ移行" : "キッズ")))))))
            .ForMember(a => a.ReceiptedDatetime, b => b.MapFrom(c => c.ReceiptedDatetime.ToString("yyyyMMdd")))
            .ForMember(a => a.ChangeInfomation, b => b.MapFrom(c => c.Member.IsUpdateInformation ? "有り" : "無し"))
            .ForMember(a => a.MemberNo, b => b.MapFrom(c => c.Member.MemberNo))
            .ForMember(a => a.OldMemberNo, b => b.MapFrom(c => c.Member.OldMemberNo))
            .ForMember(a => a.CompanyCode, b => b.MapFrom(c => c.Store.Company.CompanyCode))
            .ForMember(a => a.StoreCode, b => b.MapFrom(c => c.Store.StoreCode))
            .ForMember(a => a.PICStore, b => b.MapFrom(c => c.Member.PICStore.PICName))
            .ForMember(a => a.DeviceoCode, b => b.MapFrom(c => c.Device.DeviceCode))
            //.ForMember(a => a.KanjiName, b => b.MapFrom(c => c.Member.FirstName + "　" + c.Member.LastName))
            //.ForMember(a => a.KanaName, b => b.MapFrom(c => c.Member.FuriganaFirstName + "　" + c.Member.FuriganaLastName))
            .ForMember(a => a.FirstName, b => b.MapFrom(c => c.Member.FirstName))
            .ForMember(a => a.LastName, b => b.MapFrom(c => c.Member.LastName))
            .ForMember(a => a.FuriganaFirstName, b => b.MapFrom(c => c.Member.FuriganaFirstName))
            .ForMember(a => a.FuriganaLastName, b => b.MapFrom(c => c.Member.FuriganaLastName))

            .ForMember(a => a.Sex, b => b.MapFrom(c => c.Member.Sex == SexType.Male ? "男" : (c.Member.Sex == SexType.Female ? "女" : "その他")))
            .ForMember(a => a.DateOfBirth, b => b.MapFrom(c => c.Member.DateOfBirth.HasValue ? c.Member.DateOfBirth.Value.ToString("yyyyMMdd") : null))
            .ForMember(a => a.FixedNumber, b => b.MapFrom(c => c.Member.FixedPhone))
            .ForMember(a => a.MobileNumber, b => b.MapFrom(c => c.Member.MobilePhone))
            .ForMember(a => a.ZipCode, b => b.MapFrom(c => c.Member.ZipcodeId))
            //.ForMember(a => a.Address, b => b.MapFrom(c => c.Member.Province + c.Member.District + "　"))
            .ForMember(a => a.Province, b => b.MapFrom(c => c.Member.Province))
            .ForMember(a => a.District, b => b.MapFrom(c => c.Member.District))
            .ForMember(a => a.Street, b => b.MapFrom(c => c.Member.Street))
            .ForMember(a => a.BuildingName, b => b.MapFrom(c => c.Member.BuildingName))

            .ForMember(a => a.IsRegisterAdvertisement, b => b.MapFrom(c => c.Member.IsRegisterAdvertisement ? "受取希望" : "受取拒否"))
            .ForMember(a => a.Email, b => b.MapFrom(c => c.Member.Email))
            .ForMember(a => a.IsNetMember, b => b.MapFrom(c => c.Member.IsNetMember ? "登録" : "未登録"))
            .ForMember(a => a.IsRegisterKidClub, b => b.MapFrom(c => c.Member.IsRegisterKidClub ? "登録" : "未登録"))
            .ForMember(a => a.IsAgreeGetOutMember, b => b.MapFrom(c => c.Member.IsAgreeGetOutMember ? "同意" : "未同意"))
            .ForMember(a => a.Remark, b => b.MapFrom(c => c.Member.Remark));
        }
    }

    public class ExportRequestReceiptedHandler : IRequestHandler<ExportRequestReceiptedQuery, ExportFileVm>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICsvFileBuilder _fileBuilder;
        private readonly IConfiguration _configuration;

        public ExportRequestReceiptedHandler(IApplicationDbContext context, IMapper mapper, IConfiguration configuration, ICsvFileBuilder fileBuilder)
        {
            _context = context;
            _mapper = mapper;
            _fileBuilder = fileBuilder;
            _configuration = configuration;
        }
        public async Task<ExportFileVm> Handle(ExportRequestReceiptedQuery request, CancellationToken cancellationToken)
        {
            var query = _context.RequestsReceipteds.Where(x => !x.IsDeleted).OrderByDescending(x => x.ReceiptedDatetime).AsQueryable();
            if (!string.IsNullOrEmpty(request.MemberNo))
            {
                query = query.Where(r => r.Member.MemberNo.Contains(request.MemberNo.Trim()));
            }
            if (request.RequestType.HasValue)
            {
                query = query.Where(r => r.RequestType.Id == request.RequestType);
            }
            if (!string.IsNullOrEmpty(request.Company))
            {
                query = query.Where(r => r.Device.Store.Company.CompanyCode.ToLower().Contains(request.Company.ToLower().Trim()));
            }
            if (!string.IsNullOrEmpty(request.Store))
            {
                query = request.IsMobileRequest
                    ? query.Where(r => r.Device.Store.StoreCode.ToLower() == request.Store.ToLower().Trim())
                    : query.Where(r => r.Device.Store.StoreCode.ToLower().Contains(request.Store.ToLower().Trim()));
            }
            if (!string.IsNullOrEmpty(request.Device))
            {
                query = query.Where(r => r.Device.DeviceCode.ToLower().Contains(request.Device.ToLower().Trim()));
            }
            if (request.FromDate.HasValue)
            {
                query = query.Where(r => r.ReceiptedDatetime >= request.FromDate.Value);
            }
            if (request.ToDate.HasValue)
            {
                query = query.Where(r => r.ReceiptedDatetime <= request.ToDate.Value.AddDays(1).AddMilliseconds(-1));
            }
            if (!string.IsNullOrEmpty(request.CreatedBy))
            {
                query = query.Where(r => r.Member.PICStore.PICName.ToLower().Contains(request.CreatedBy.ToLower().Trim()));
            }
            if (request.PICStoreId.HasValue)
            {
                query = query.Where(r => r.Member.PICStoreId == request.PICStoreId);
            }

            int maxDataCount = 0;
            if (int.TryParse(_configuration["ApplicationSetting:MaxRecordExport"], out maxDataCount))
                if (query.Count() > maxDataCount) throw new ValidationException(new List<ValidationFailure>() { new ValidationFailure("MoreThanRecord", maxDataCount.ToString()) });

            var records = await query.Include(x => x.Store).Include(x => x.Store.Company).Include(x => x.Member.PICStore).Include(x => x.Device).Take(maxDataCount).ProjectTo<RequestReceiptedRecord>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);

            ExportFileVm vm = new ExportFileVm();

            int index = 1;
            foreach (var item in records)
            {
                item.No = index;

                //decrypt data
                item.KanjiName = await item.LastName.ToDecryptStringAsync() + " " + await item.FirstName.ToDecryptStringAsync();
                item.KanaName = await item.FuriganaLastName.ToDecryptStringAsync() + " " + await item.FuriganaFirstName.ToDecryptStringAsync();
                item.Email = await item.Email.ToDecryptStringAsync();
                item.FixedNumber = await item.FixedNumber.ToDecryptStringAsync();
                item.MobileNumber = await item.MobileNumber.ToDecryptStringAsync();
                item.Address = await item.Province.ToDecryptStringAsync() + await item.District.ToDecryptStringAsync() + await item.Street.ToDecryptStringAsync() + await item.BuildingName.ToDecryptStringAsync();

                index++;
            }

            vm.Content = _fileBuilder.BuilRequestReceiptedFile(records);
            vm.ContentType = "text/csv";
            vm.FileName = $"{DateTime.Now.ToString("yyyymmdd_HHMMss")}.csv";
            return await Task.FromResult(vm);
        }
    }
}
