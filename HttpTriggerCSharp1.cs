using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;

namespace AzureFunctionsCSharp
{
    using Model;

    public static class HttpTriggerCSharp1
    {
        [FunctionName("HttpTriggerCSharp1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference("SampleTable");
            await table.CreateIfNotExistsAsync();

            // 1åècreate
            CustomerEntity customer1 = new CustomerEntity("taro", "yamada")
            {
                Email = "taro.yamada@testmail.co.jp",
                PhoneNumber = "090-1111-2222"
            };
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(customer1);
            TableResult result1 = await table.ExecuteAsync(insertOrMergeOperation);
            //CustomerEntity insertedCustomer = result1.Result as CustomerEntity;

            // 1åèread
            TableOperation retrieveOperation = TableOperation.Retrieve<CustomerEntity>("taro", "yamada");
            TableResult result2 = await table.ExecuteAsync(retrieveOperation);
            CustomerEntity customer2 = result2.Result as CustomerEntity;

            // ï°êîåèreadÇµÇΩÇ¢èÍçáÇÕTableQueryÇégóp

            // 1åèupdate
            customer2.PhoneNumber = "080-9999-8888";
            insertOrMergeOperation = TableOperation.InsertOrMerge(customer2);
            TableResult result3 = await table.ExecuteAsync(insertOrMergeOperation);

            // 1åèdelete
            //TableOperation deleteOperation = TableOperation.Delete(customer2);
            //TableResult result4 = await table.ExecuteAsync(deleteOperation);

            return new OkObjectResult("This HTTP triggered function executed successfully.");
        }
    }
}
