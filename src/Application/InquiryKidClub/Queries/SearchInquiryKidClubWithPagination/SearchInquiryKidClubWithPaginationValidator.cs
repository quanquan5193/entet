using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.InquiryKidClub.Queries.SearchInquiryKidClubWithPagination
{
    public class SearchInquiryKidClubWithPaginationValidator : AbstractValidator<SearchInquiryKidClubWithPaginationQuery>
    {
        public SearchInquiryKidClubWithPaginationValidator()
        {
            RuleFor(x => x.StartDate)
                .GreaterThan(new DateTime(1900, 1, 1))
                    .WithMessage("Start date must be greater than 01/01/1900");
            RuleFor(x => x.EndDate)
                .LessThanOrEqualTo(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59))
                    .WithMessage("End date must be less than or equal today");
        }
    }
}
