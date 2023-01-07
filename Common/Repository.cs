using Common.Models;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using System.Threading;

namespace Common
{
    public abstract class Repository<T>
    {
        protected CloudStorageAccount _storageAccount;
        protected CloudTable _table;
        protected string _tableName;
        public Repository(string tableName)
        {
            _tableName = tableName;
            _storageAccount =
                CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new
                Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference(_tableName + "Table");
            _table.CreateIfNotExists();
           // SyncService();

        }
        public abstract List<T> RetrieveAll(); 
         
        public void AddOrUpdate(T newBankAccount)
        {
            TableOperation insertOperation = TableOperation.InsertOrReplace(newBankAccount as TableEntity);
            _table.Execute(insertOperation);

        }
        public abstract Task SyncService();
    }
}
