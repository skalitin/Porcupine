using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Porcupine.Agent
{
    public class StorageService : Porcupine.StorageService.StorageServiceBase
    {
        private readonly ILogger<StorageService> _logger;
        public StorageService(ILogger<StorageService> logger)
        {
            _logger = logger;
        }

        public override Task<CopyFileResponse> CopyFile(CopyFileRequest request, ServerCallContext context)
        {
            _logger.LogDebug($"Copying file {request.Source} to {request.Target}...");

            return Task.Run(() => 
            {
                var file = new FileInfo(request.Source);
                file.CopyTo(request.Target, true);
                return new CopyFileResponse() { Path = request.Target};
            });
        }

        public override async Task CopyFolder(CopyFolderRequest request, IServerStreamWriter<CopyFolderResponse> responseStream, ServerCallContext context)
        {
            _logger.LogDebug($"Copying folder {request.Source} to {request.Target}...");

            var directory = new DirectoryInfo(request.Source);
            var files = directory.GetFiles();

            if (!Directory.Exists(request.Target))
                Directory.CreateDirectory(request.Target);

            foreach (var file in files)
            {
                var newPath = Path.Combine(request.Target, file.Name);

                _logger.LogDebug($"Copying file {file.Name} to {request.Target}...");
                file.CopyTo(newPath, true);

                // Simulate long copy operation
                await Task.Delay(TimeSpan.FromSeconds(1));

                if(context.CancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning($"Copy folder cancelled.");
                    break;
                }

                var creationTimestamp = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(file.CreationTime, DateTimeKind.Utc));
                await responseStream.WriteAsync(new CopyFolderResponse {
                    FileName = file.Name,
                    CreationTimestamp = creationTimestamp
                });
            }
        }
    }
}
