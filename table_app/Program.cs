using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;

namespace table_app
{
    class Program
    {

        static CloudStorageAccount StorageAccount;

        static Program()
        {
            StorageAccount = CloudStorageAccount.Parse("ConnectionString");
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("1.Create Table");
            Console.WriteLine("2.Insert Record");
            Console.WriteLine("3.List");
            Console.WriteLine("4.List By Partition");
            Console.WriteLine("5.Update");
            Console.WriteLine("6.Delete");
            Console.WriteLine("7.Generate SAS");
            var operation = Console.ReadLine();
            switch (operation)
            {
                case "1":
                    await CreateTableAsync();
                    break;
                case "2":
                    await InserData();
                    break;
                case "3":
                    await List();
                    break;
                case "4":
                    await ListByPartitionKey();
                    break;
                case "5":
                    await Update();
                    break;
                case "6":
                    await Delete();
                    break;
            }
            Console.ReadLine();
        }


        static async Task<CloudTable> CreateTableAsync()
        {
            var tableClient = StorageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("todo");
            await table.CreateIfNotExistsAsync();
            
            return table;
        }


        static async Task InserData()
        {
            var table = await CreateTableAsync();

            var record = new ToDO()
            {
                PartitionKey = "Vacation",
                RowKey = "Nothing Good",
                Name = "Nothing testes"
            };

            var createOperation = TableOperation.Insert(record);
            var result = await table.ExecuteAsync(createOperation);
        }

        static async Task List()
        {
            var table = await CreateTableAsync();
            var query = new TableQuery<ToDO>();

            var records = table.ExecuteQuery(query);

            foreach (var item in records)
            {
                Console.WriteLine(item.Name + " " + item.RowKey + " " + item.PartitionKey);

            }
        }

        static async Task ListByPartitionKey()
        {
            var table = await CreateTableAsync();
            var queryOperation = TableOperation.Retrieve<ToDO>("Vacation", "Nothing Good");
            var records = await table.ExecuteAsync(queryOperation);
            var results = records.Result as ToDO;
            Console.WriteLine(results.Name);
        }

        static async Task Update()
        {
            var table = await CreateTableAsync();

            var queryOperation = TableOperation.Retrieve<ToDO>("Vacation", "Nothing Good");
            var records = await table.ExecuteAsync(queryOperation);
            var results = records.Result as ToDO;
            results.Name = "to do list updated";

            var updateOperation = TableOperation.InsertOrReplace(results);
            await table.ExecuteAsync(updateOperation);
        }

        static async Task Delete()
        {
            var table = await CreateTableAsync();

            var queryOperation = TableOperation.Retrieve<ToDO>("Vacation", "Nothing Good");
            var records = await table.ExecuteAsync(queryOperation);
            var results = records.Result as ToDO;
            results.Name = "to do list updated";

            var updateOperation = TableOperation.Delete(results);
           
            await table.ExecuteAsync(updateOperation);
        }


        static async Task GenerateSAS()
        {
            var table = await CreateTableAsync();

            var sasToken = table.GetSharedAccessSignature(new SharedAccessTablePolicy
            {
                Permissions = SharedAccessTablePermissions.Query,
                SharedAccessExpiryTime = DateTime.Now.AddDays(1),
                SharedAccessStartTime = DateTime.Now
            });

        }
    }
}


public class ToDO : TableEntity
{
    public string Name { get; set; }
}