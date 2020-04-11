using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;

using Microsoft.Azure.Storage.Blob;

namespace blob_app
{
    class Program
    {
        static CloudStorageAccount StorageAccount;

        static Program()
        {
            StorageAccount = CloudStorageAccount.Parse("t");
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("1.Create Contaier");
            Console.WriteLine("2.Upload Blob");
            Console.WriteLine("3.List Container");
            Console.WriteLine("4.List Blobs");

            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    await CreateBlobContainer();
                    break;
                case "2":
                    await UploadBlockBlob();
                    break;
                case "3":
                    ListContainers();
                    break;
                case "4":
                  await  ListBlobs();
                    break;

            }

        }

        static async Task<CloudBlobContainer> CreateBlobContainer()
        {
            var blobClient = StorageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("estimatorimages");
            await container.CreateIfNotExistsAsync();
            return container;
        }

        static async Task UploadBlockBlob()
        {
            var container = await CreateBlobContainer();
            var blockBlob = container.GetBlockBlobReference("12-cta-buttons.psd");
            using (var fs = new FileStream("D:/1Ps/12-cta-buttons.psd", FileMode.Open, FileAccess.Read))
            {
                await blockBlob.UploadFromStreamAsync(fs);
                
            }
            Console.WriteLine(blockBlob.Uri);
            Console.ReadLine();
        }

        static async Task DeleteBlob()
        {
            var container = await CreateBlobContainer();
            var blockBlob = container.GetBlockBlobReference("12-cta-buttons.psd");
            await blockBlob.DeleteIfExistsAsync();
        }

        static  void ListContainers()
        {
            var blobClient = StorageAccount.CreateCloudBlobClient();

            foreach(var container in blobClient.ListContainers())
            {
                Console.WriteLine(container.Uri);
            }

        }

        static async Task ListBlobs()
        {
            var container = await CreateBlobContainer();

            foreach(var blob in container.ListBlobs())
            {
                Console.WriteLine(blob.Uri);
            }
        }

        static async Task AddContainerMetadataandList()
        {
            var container = await CreateBlobContainer();
            container.Metadata.Add("name", "sample data");
            container.Metadata["year"] = "2020";
            await container.SetMetadataAsync();

            await container.FetchAttributesAsync();

            foreach(var data in container.Metadata)
            {
                Console.WriteLine($"{data.Key}-{data.Value}");
            }

        }

    }
}
