﻿using ProspectManagement.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProspectManagement.Core.Models;
using MvvmCross.Plugins.Sqlite;
using SQLite;

namespace ProspectManagement.Core.Repositories
{
    public class DefaultCommunityRepository : IDefaultCommunityRepository
    {        
		private readonly IMvxSqliteConnectionFactory _sqlFactory;
		private readonly SQLiteAsyncConnection _connection;

		public DefaultCommunityRepository(IMvxSqliteConnectionFactory sqlFactory)
		{
			_sqlFactory = sqlFactory;
			_connection = _sqlFactory.GetAsyncConnection("defaultCommunity");
			_connection.CreateTableAsync<Community>();
		}

		public Task<Community> GetDefaultCommunity()
        {
			return _connection.Table<Community>().FirstOrDefaultAsync(); // Task.FromResult(_defaultCommunity);
        }

        public void SaveDefaultCommunity(Community community)
        {
            _connection.ExecuteAsync("DELETE FROM Community");
			_connection.InsertAsync(community); //Task.Run(() => _defaultCommunity = community);
        }
    }
}
