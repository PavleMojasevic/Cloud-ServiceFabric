﻿using Common.Models;
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
    public abstract class Repository<T> where T : TableEntity
    {
        protected CloudStorageAccount _storageAccount;
        protected CloudTable _table;
        protected string _tableName;
        public Repository(string tableName, string connString="UseDevelopmentStorage=true")
        {
            _tableName = tableName;
            
            _storageAccount =
                CloudStorageAccount.Parse(connString);
            CloudTableClient tableClient = new CloudTableClient(new
                Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference(_tableName + "Table");
            _table.CreateIfNotExists(); 

        }
        public abstract List<T> RetrieveAll();
        
        public void AddOrUpdate(T newBankAccount)
        {
            TableOperation insertOperation = TableOperation.InsertOrReplace(newBankAccount as TableEntity);
            var result = _table.Execute(insertOperation);

        }
        public abstract Task SyncTable();
    }
}
