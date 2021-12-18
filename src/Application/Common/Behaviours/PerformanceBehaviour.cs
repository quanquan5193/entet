using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using mrs.Application.Common.Helpers.AzureStorage;
using mrs.Application.Common.Interfaces;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Common.Behaviours
{
    public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly Stopwatch _timer;
        private readonly ILogger<TRequest> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;
        private readonly IAzureStorageHelper _azureStorageHelpers;

        public PerformanceBehaviour(
            ILogger<TRequest> logger, 
            ICurrentUserService currentUserService,
            IIdentityService identityService,
            IConfiguration configuration)
        {
            _timer = new Stopwatch();

            _logger = logger;
            _currentUserService = currentUserService;
            _identityService = identityService;
            _azureStorageHelpers = new AzureStorageHelper(currentUserService, identityService, configuration);
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            var requestName = typeof(TRequest).Name;

            string message = string.Format("Requested: {0} ({1} milliseconds) {2}",
                requestName, elapsedMilliseconds, request);

            _logger.LogWarning(message);

            await _azureStorageHelpers.SaveLogPerformanceToBlob(message);

            return response;
        }
    }
}
