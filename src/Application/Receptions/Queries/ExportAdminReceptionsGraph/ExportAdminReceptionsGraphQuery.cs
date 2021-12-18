using AutoMapper;
using mrs.Application.Common.Interfaces;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;
using mrs.Domain.Enums;
using System.Collections.Generic;
using mrs.Application.Common.Exceptions;
using mrs.Application.Receptions.Queries.Dto;
using mrs.Application.Cards.Queries.ExportReceptionsTable;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using System.IO;
using iTextSharp.text.pdf;
using System.Drawing;
using iTextSharp.text;
using Microsoft.AspNetCore.Hosting;

namespace mrs.Application.Cards.Queries.ExportAdminReceptionsGraphQuery
{
    public class ExportAdminReceptionsGraphQueryQuery : IRequest<ExportReceptionsTableVm>
    {
        [JsonIgnore]
        public IFormFile File { get; set; }
    }

    public class ExportAdminReceptionsGraphQueryQueryHandler : IRequestHandler<ExportAdminReceptionsGraphQueryQuery, ExportReceptionsTableVm>
    {
        public ExportAdminReceptionsGraphQueryQueryHandler()
        {
        }

        public async Task<ExportReceptionsTableVm> Handle(ExportAdminReceptionsGraphQueryQuery request, CancellationToken cancellationToken)
        {
            ExportReceptionsTableVm vm = new ExportReceptionsTableVm();
            using (var ms = new MemoryStream())
            {
                var image = request.File.OpenReadStream();
                iTextSharp.text.Image imgPdf = iTextSharp.text.Image.GetInstance(image);
                Document pdfDoc = new Document();
                pdfDoc.SetPageSize(PageSize.A4.Rotate());
                imgPdf.SetAbsolutePosition(0, 0);
                imgPdf.ScaleAbsolute(PageSize.A4.Height, PageSize.A4.Width);
                PdfWriter.GetInstance(pdfDoc, ms);
                pdfDoc.Open();
                pdfDoc.NewPage();
                pdfDoc.Add(imgPdf);
                pdfDoc.Close();
                vm.Content = ms.ToArray();
            }
           
            vm.ContentType = "application/pdf";
            vm.FileName = $"{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.pdf";

            return await Task.FromResult(vm);
        }
    }
}
