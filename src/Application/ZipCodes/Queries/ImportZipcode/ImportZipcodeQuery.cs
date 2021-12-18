using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.ZipCodes.Queries.ImportZipcode
{
    public class ImportZipcodeQuery : IRequest<bool>
    {
        public string ZipcodeDirectory { get; set; }
        public string Name { get; set; }
    }

    public class ImportZipcode : IRequestHandler<ImportZipcodeQuery, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private const int MaximumZipcodeLength = 7;
        private const int LastFourZipcodeNumber = 4;
        private const string SpecialZipcodeCase = "0000";
        private const string ZipcodeStringFormat = "D7";

        public ImportZipcode(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> Handle(ImportZipcodeQuery request, CancellationToken cancellationToken)
        {
            if (_context.ZipCodes.Any())
            {
                return await Task.FromResult(false);
            }

            if (!Directory.Exists(request.ZipcodeDirectory))
            {
                return await Task.FromResult(false);
            }

            var di = new DirectoryInfo(request.ZipcodeDirectory);
            var listFileCsvImport = Directory.GetFiles(request.ZipcodeDirectory, "*.csv");

            if (listFileCsvImport.Any(x => string.IsNullOrWhiteSpace(x)))
            {
                return await Task.FromResult(false);
            }

            List<ZipcodeCsvImportDto> listZipcodes = new List<ZipcodeCsvImportDto>();

            // Read data zipcode from csv file and return list zipcode objects
            foreach (var filePath in listFileCsvImport)
            {
                using (TextReader reader = new StreamReader(filePath))
                {
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        IgnoreBlankLines = true,
                        HasHeaderRecord = false
                    };

                    var csvReader = new CsvReader(reader, config);

                    var listZipcodeDto = csvReader.GetRecords<ZipcodeCsvImportDto>().ToList();
                    if (listZipcodeDto != null)
                    {
                        listZipcodes.AddRange(listZipcodeDto);
                    }
                };
            }

            // Check length zipcode is greater than "maximumZipcodeLength"
            bool isNotValidDataImport = listZipcodes.Any(g => g.Zipcode.ToString().Length > MaximumZipcodeLength);
            // Select the first zipcode info if it is dupplicated
            listZipcodes = listZipcodes.GroupBy(x => x.Zipcode).Select(x => x.First()).ToList();

            if (isNotValidDataImport)
            {
                return await Task.FromResult(false);
            }
            else
            {
                foreach (var item in listZipcodes)
                {
                    var zipcodeEntity = new ZipCode();
                    var zipcodeFormated = item.Zipcode.ToString(ZipcodeStringFormat);
                    var isImportAddress = zipcodeFormated.Substring(zipcodeFormated.Length - LastFourZipcodeNumber).Equals(SpecialZipcodeCase) ? true : false;

                    zipcodeEntity.Zipcode = zipcodeFormated;
                    zipcodeEntity.Province = item.Province;
                    zipcodeEntity.District = item.District;
                    zipcodeEntity.Street = isImportAddress ? null : item.Street;
                    zipcodeEntity.CreatedAt = DateTime.Now;
                    zipcodeEntity.CreatedBy = request.Name;
                    zipcodeEntity.UpdatedAt = DateTime.Now;
                    zipcodeEntity.UpdatedBy = request.Name;
                    zipcodeEntity.IsDeleted = false;

                    _context.ZipCodes.Add(zipcodeEntity);
                }
                await _context.SaveChangesAsync(cancellationToken);
                return await Task.FromResult(true);
            }
        }

    }
}
