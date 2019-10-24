using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lab5
{
    class Program
    {
        static CloudStorageAccount storageAccount;
        static CloudTableClient tableClient;
        static CloudTable table;
        static void Main(string[] args)
        {
            try
            {
                CreateAzureStorageTable();
                AddGuestEntity();
                RetrieveGuestEntity();
                UpdateGuestEntity();
                DeleteGuestEntity();
                DeleteAzureStorageTable();
            }
            catch (StorageException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }
        //Method to create AzureStorageTable
        private static void CreateAzureStorageTable()
        {
            //RETRIEVE THE STORAGE ACCOUNT FROM THE CONNECTION STRING
            storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            //CREATE THE TABLE CLIENT
            tableClient = storageAccount.CreateCloudTableClient();

            //CREATE THE CLOUDTABLE OBJECT THAT REPRESENTS THE "guests" table
            table = tableClient.GetTableReference("guests");

            //CREATE THE TABLE IF IT DOES NOT EXIST
            table.CreateIfNotExists();
            Console.WriteLine("Table Created");
        }

        private static void DeleteAzureStorageTable()
        {
            table.DeleteIfExists();
            Console.WriteLine("Table Deleted");
        }

        private static void DeleteGuestEntity()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<GuestEntity>("IND", "K001");
            TableResult retrievedResult = table.Execute(retrieveOperation);
            if (retrievedResult.Result != null)
            {
                var guest = retrievedResult.Result as GuestEntity;
                TableOperation deleteOperation = TableOperation.Delete(guest);
                table.Execute(deleteOperation);
                Console.WriteLine("Entity Deleted");
            }
            else
            {
                Console.WriteLine("Details could not be retrieved.");

            }
        }

        private static void UpdateGuestEntity()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<GuestEntity>("IND", "K001");
            TableResult retrievedResult = table.Execute(retrieveOperation);
            if (retrievedResult.Result != null)
            {
                var guest = retrievedResult.Result as GuestEntity;
                guest.ContactNumber = "7894561230";
                TableOperation updateOperation = TableOperation.Replace(guest);
                table.Execute(updateOperation);
                Console.WriteLine("Entity Updated");
            }
            else
            {
                Console.WriteLine("Details could not be retrieved.");
            }
        }

        private static void RetrieveGuestEntity()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<GuestEntity>("IND","K001");
            TableResult retrievedResult = table.Execute(retrieveOperation);
            if (retrievedResult.Result != null)
            {
                var guest = retrievedResult.Result as GuestEntity;
                Console.WriteLine($"Name:{guest.Name} ContactNumber:{guest.ContactNumber}");
            }
            else
            {
                Console.WriteLine("Details could not be retrieved.");
            }
        }

        private static void AddGuestEntity()
        {
            //Create a new guest Entity
            GuestEntity guestEntity = new GuestEntity("IND","K001");
            guestEntity.Name = "karthik";
            guestEntity.ContactNumber = "9874561230";
            TableOperation insertOperation = TableOperation.Insert(guestEntity);
            table.Execute(insertOperation);
            Console.WriteLine("Entity Added");
        }
    }
    class GuestEntity:TableEntity
    {
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public GuestEntity() { }
        public GuestEntity(string partitionKey,string rowKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }
    }
}
