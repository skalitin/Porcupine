using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Porcupine.Agent
{
    public class RetentionService : Porcupine.RetentionService.RetentionServiceBase
    {
        private readonly ILogger<RetentionService> _logger;
        public RetentionService(ILogger<RetentionService> logger)
        {
            _logger = logger;
        }
    }
}
