using AutoMapper;
using mrs.Application.Common.Behaviours;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using mrs.Application.Members.Commands.RegisterMember;
using mrs.Application.MissingCard.Commands.AddPendingRequestList;
using mrs.Application.MissingCard.Queries.SearchPendingRequestListWithPagination;
using mrs.Application.MissingCard.Commands.GetPendingRequestDetail;
using mrs.Application.MissingCard.Queries.GetRequestTypes;
using mrs.Application.Devices.Queries.SearchDevicesWithPagination;
using mrs.Application.Devices.Commands.GetDevice;
using mrs.Application.Stores.Queries.GetStoresWithPagination;
using mrs.Application.Companies.Commands.GetDetailCompanyWithCode;

namespace mrs.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(GetClassifyRequestProfile).Assembly);
            services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(RegisterMemberProfile).Assembly);
            services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(GetPendingCardDetailProfile).Assembly);
            services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(SearchPendingListProfile).Assembly);
            services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(RegisterLostCardProfile).Assembly);
            services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(SearchDevicesWithPaginationProfile).Assembly);
            services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(GetDeviceCommandProfile).Assembly); 
            services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(GetStoresWithPaginationQueryProfile).Assembly);
            services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(GetDetailCompanyWithCodeCommandProfile).Assembly);


            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

            return services;
        }
    }
}
