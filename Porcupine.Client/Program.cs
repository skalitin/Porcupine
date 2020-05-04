using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Grpc.Net.Client;
using Grpc.Core;

namespace Porcupine.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, certificate, chain, errors) =>
            {
                return certificate.Thumbprint == "0F99713078174F9954576BA45E76DAF520EF0B98";
            };

            var httpClient = new HttpClient(httpClientHandler);
            using var channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions { HttpClient = httpClient });
            var storageServiceClient = new StorageService.StorageServiceClient(channel);

            await CopyFileTest(storageServiceClient);
            await CopyFolderTest(storageServiceClient);
        }

        private static async Task CopyFileTest(StorageService.StorageServiceClient client)
        {
            var request = new CopyFileRequest() { Source = @"C:\Temp\data.bin", Target = @"C:\Temp\data_copy.bin"};
            var result = await client.CopyFileAsync(request);

            Console.WriteLine($"File {request.Source} copied to {result.Path}");
        }

        private static async Task CopyFolderTest(StorageService.StorageServiceClient client)
        {
            var request = new CopyFolderRequest() { Source = @"C:\Temp\Data", Target = @"C:\Temp\Data_Temp"};
            
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            using var streamingCall = client.CopyFolder(request, cancellationToken: cancellationTokenSource.Token);

            try
            {
                await foreach (var copyItem in streamingCall.ResponseStream.ReadAllAsync(cancellationTokenSource.Token))
                {
                    Console.WriteLine($"File copied: {copyItem.Name}");
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Stream cancelled.");
            }
        }
    }
}
