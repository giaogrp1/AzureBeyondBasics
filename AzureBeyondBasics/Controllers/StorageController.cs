using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AzureBeyondBasics.Controllers
{
    public class StorageController : Controller
    {
        // GET: Storage
        public ActionResult Index()
        {
            // Retrive Connection String 
            var storageConnectionString = ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;

            // Retrive Storage Account from Connection String 
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create table client
            var tableClient = storageAccount.CreateCloudTableClient();

            // Creat CloudTable Object that represent the "Customer" table
            var table = tableClient.GetTableReference("customer");

            // Crete the table if it doesn't exist 
            table.CreateIfNotExists();

            var customer = new CustomerEntiry(Guid.NewGuid())
            {
                FirstName = "John",
                LastName = "Berry",
                Email = "JBerry@msn.com",
                PhoneNumber = "416-909-0011"
            };

            // Create The TableOperation Object that insert the customer entity
            var insertOperation = TableOperation.Insert(customer);

            // Execute the Insert operation
            table.Execute(insertOperation);

            // Create the queue client
            var queueClient = storageAccount.CreateCloudQueueClient();

            // Retrueve a referece to a container.
            var queue = queueClient.GetQueueReference("customerqueue");

            // Create the queue if it doesn't already exsit.
            queue.CreateIfNotExists();

            // Create a message and add it to the queue.
            var message = new CloudQueueMessage(customer.RowKey);
            queue.AddMessage(message);

            return View(customer);
        }
    }

    public class CustomerEntiry : TableEntity
    {
        public CustomerEntiry(Guid employeeId)
        {
            PartitionKey = "customer";
            RowKey = employeeId.ToString();
        }

        public CustomerEntiry() { }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

    }

}