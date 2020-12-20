﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jerrycurl.Data;
using Jerrycurl.Relations.Metadata;
using Jerrycurl.Relations;
using Jerrycurl.Data.Commands;
using Microsoft.Data.Sqlite;
using Shouldly;
using Jerrycurl.Data.Metadata;
using Jerrycurl.Data.Queries;
using Jerrycurl.Data.Filters;
using Jerrycurl.Test;

namespace Jerrycurl.Data.Test
{
    public class TransactionTests
    {
        public void Test_Inserts_WithoutTransaction()
        {
            this.CreateTable();
            this.ClearTable();

            CommandData command = new CommandData()
            {
                CommandText = @"INSERT INTO MyValues VALUES(1);
                                INSERT INTO MyValues VALUES(2);
                                INSERT INTO MyValues VALUES(NULL);
                                INSERT INTO MyValues VALUES(3);"
            };

            try
            {
                DatabaseHelper.Default.Execute(command);
            }
            catch (AdoException) { }

            this.GetCurrentValues().ShouldBe(new[] { 1, 2 });
        }

        public void Test_Inserts_WithTransaction()
        {
            this.CreateTable();
            this.ClearTable();

            CommandData command = new CommandData()
            {
                CommandText = @"INSERT INTO MyValues VALUES(1);
                                INSERT INTO MyValues VALUES(2);
                                INSERT INTO MyValues VALUES(NULL);
                                INSERT INTO MyValues VALUES(3);"
            };

            CommandHandler handler = new CommandHandler(DatabaseHelper.Default.GetCommandOptions(new TransactionFilter()));

            try
            {
                handler.Execute(command);
            }
            catch (AdoException) { }

            this.GetCurrentValues().ShouldBeEmpty();
        }

        private IList<int> GetCurrentValues()
        {
            QueryData query = new QueryData()
            {
                QueryText = "SELECT Value AS `Item` FROM MyValues",
            };

            return DatabaseHelper.Default.Query<int>(query);
        }

        private void ClearTable()
        {
            CommandData command = new CommandData()
            {
                CommandText = @"DELETE FROM MyValues;",
            };

            DatabaseHelper.Default.Execute(command);
        }

        private void CreateTable()
        {
            CommandData command = new CommandData()
            {
                CommandText = @"CREATE TABLE IF NOT EXISTS MyValues ( Value int NOT NULL );",
            };

            DatabaseHelper.Default.Execute(command);
        }
    }
}