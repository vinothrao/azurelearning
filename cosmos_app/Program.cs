using Microsoft.Azure.Cosmos;
using System;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace cosmos_app
{
    public class Program
    {
        static CosmosClient client;

        static Program()
        {
            client = new CosmosClient("h", ""); ;
        }


        static async Task Main(string[] args)
        {


            Console.WriteLine("1.List");
            Console.WriteLine("2.Insert");
            Console.WriteLine("3.Update");
            Console.WriteLine("4.Delete");
            Console.WriteLine("5.Update Index");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await List();
                    break;
                case "2":
                    await Insert();
                    break;
                case "3":
                    await Update();
                    break;
                case "4":
                    await Delete();
                    break;
                case "5":
                    await UpdateIndex();
                    break;

            }


        }

        private static async Task List()
        {
            var container = client.GetContainer("Volcanos", "volcano");
            var sql = "select * from c where c.Region =\"India\"";
            var page = container.GetItemQueryIterator<dynamic>(sql);
            var records = await page.ReadNextAsync();

            foreach (var record in records)
            {
                Console.WriteLine(record);
            }
            Console.ReadLine();
        }

        private static async Task Insert()
        {
            dynamic volcano = new ExpandoObject();
            volcano.name = "New India Test";
            volcano.Region = "India";
            volcano.id = Guid.NewGuid();
            var container = client.GetContainer("Volcanos", "volcano");
            var result = await container.CreateItemAsync(volcano, new PartitionKey("India"));
            Console.WriteLine(result);
            Console.ReadLine();
        }


        private static async Task Update()
        {
            var container = client.GetContainer("Volcanos", "volcano");
            var sql = "select * from c where c.Region=\"India\"";

            QueryRequestOptions options = new QueryRequestOptions()
            {
                PartitionKey = new PartitionKey("India"),
                MaxItemCount = 1
            };

            var page = container.GetItemQueryIterator<dynamic>(sql, requestOptions: options);
            var result = (await page.ReadNextAsync()).ToList();
            foreach (var item in result)
            {
                item.Country = "New India Updated";
                var updateditem =await container.ReplaceItemAsync(item,(string) item.id, new PartitionKey("India"));
                Console.WriteLine(updateditem);
            }

            Console.ReadLine();

        }

        private static async Task Delete()
        {
            var container = client.GetContainer("Volcanos", "volcano");

            await container.DeleteItemAsync<dynamic>("735291bf-6e75-4db6-86af-5c48442baeeb", new PartitionKey("India"));
        }

        private static async Task UpdateIndex()
        {
            var database = client.GetDatabase("volcano");
            var container = await client.GetContainer("Volcanos", "volcano").ReadContainerAsync();
            //container.Resource.IndexingPolicy.IndexingMode = IndexingMode.Consistent;
            //container.Resource.IndexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/Type/*" });

            var compositeKeys = new Collection<CompositePath>();
            compositeKeys.Add(new CompositePath() { Path = "/Region", Order = CompositePathSortOrder.Ascending });
            compositeKeys.Add(new CompositePath() { Path = "/Country", Order = CompositePathSortOrder.Ascending });
            container.Resource.IndexingPolicy.CompositeIndexes.Add(compositeKeys);

            await client.GetContainer("Volcanos", "volcano").ReplaceContainerAsync(container.Resource);
        }
    }

}

