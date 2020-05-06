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

            var server = "localhost";
            if(args.Length == 1)
            {
                server = args[0];
            }
            var url = $"https://{server}:5001";

            try
            {
                Console.WriteLine($"Server {url}");

                using var channel = GrpcChannel.ForAddress($"https://{server}:5001", new GrpcChannelOptions { HttpClient = httpClient });

                var storageServiceClient = new StorageService.StorageServiceClient(channel);
                await CopyFileTest(storageServiceClient);
                await CopyFolderTest(storageServiceClient);

                var antimalwareServiceClient = new AntimalwareService.AntimalwareServiceClient(channel);
                await ScanFolderTest(antimalwareServiceClient);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task CopyFileTest(StorageService.StorageServiceClient client)
        {
            Console.WriteLine($"\nCalling 'CopyFile'...");

            var request = new CopyFileRequest() { Source = @"C:\Temp\data.bin", Target = @"C:\Temp\data_copy.bin"};
            var result = await client.CopyFileAsync(request);

            Console.WriteLine($"\tFile {request.Source} copied to {result.Path}");
        }

        private static async Task CopyFolderTest(StorageService.StorageServiceClient client)
        {
            Console.WriteLine($"\nCalling 'CopyFolder'...");

            var request = new CopyFolderRequest() { Source = @"C:\Temp\Data", Target = @"C:\Temp\Data_Temp"};
            
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            using var streamingCall = client.CopyFolder(request, cancellationToken: cancellationTokenSource.Token);

            try
            {
                await foreach (var copyItem in streamingCall.ResponseStream.ReadAllAsync(cancellationTokenSource.Token))
                {
                    Console.WriteLine($"\tFile copied: {copyItem.FileName}, created at {copyItem.CreationTimestamp.ToDateTime()}");
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Stream cancelled.");
            }
        }

        private static async Task ScanFolderTest(AntimalwareService.AntimalwareServiceClient client)
        {
            Console.WriteLine($"\nCalling 'ScanFolder'...");

            var request = new ScanFolderRequest() { Path = @"C:\Temp\Data" };
            var result = await client.ScanFolderAsync(request);

            Console.WriteLine($"Scanned by {result.AntivirusInfo}");
            foreach(var infectedFile in result.InfectedFiles) {
                Console.WriteLine($"\tFile {infectedFile.FileName} is infected by {infectedFile.MalwareName}");
            }
        }
    }
}
