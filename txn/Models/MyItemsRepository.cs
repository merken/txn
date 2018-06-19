using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace txn.Models
{
    public class MyItemsRepository : IService
    {
        private readonly IDbConnection connection;
        private readonly IDbTransaction transaction;
        public MyItemsRepository(IDbTransaction transaction)
        {
            this.transaction = transaction;
            this.connection = transaction.Connection;
        }

        public Task<IEnumerable<MyItem>> GetAll()
        {
            return Query($"SELECT * FROM MyTable");
        }

        public async Task<MyItem> GetById(Guid id)
        {
            return (await Query($"SELECT * FROM MyTable WHERE Id = @id", new { id })).First();
        }

        public Task<IEnumerable<MyItem>> GetBetweenCreatedDateRange(DateTimeOffset start, DateTimeOffset end)
        {
            return Query($"SELECT * FROM MyTable WHERE @start <= CreatedTimeStamp AND CreatedTimeStamp <= @end", new { start, end });
        }

        public Task<IEnumerable<MyItem>> Search(string searchTerm)
        {
            return Query($"SELECT * FROM MyTable WHERE Description LIKE '%' + @searchTerm + '%'", new { searchTerm });
        }

        public async Task<MyItem> AddOrUpdate(MyItem item)
        {
            var sqlStatement = "UPDATE MyTable SET Description = @Description WHERE Id = @Id;";

            var isAdd = IsAdd(item);
            if (isAdd)
            {
                item.Id = Guid.NewGuid();
                item.CreatedTimeStamp = DateTimeOffset.Now;
                sqlStatement = "INSERT INTO MyTable(Id, CreatedTimeStamp, Description) VALUES (@Id, @CreatedTimeStamp, @Description);";
            }

            var changes = await connection.ExecuteAsync(sqlStatement, item, transaction);
            if (changes == -1)
            {
                var message = isAdd ? "create new item" : "save item";
                throw new InvalidOperationException($"Could {message} with id {item.Id}");
            }

            return item;
        }

        public async Task Delete(Guid id)
        {
            var changes = await connection.ExecuteAsync("DELETE FROM MyTable WHERE Id = @id;", new { id }, transaction);
            if (changes == -1)
            {
                throw new InvalidOperationException($"Could delete item with id {id}");
            }
        }

        private bool IsAdd(MyItem item)
        {
            return item.Id == Guid.Empty;
        }

        private Task<IEnumerable<MyItem>> Query(string query, object param = null)
        {
            return connection.QueryAsync<MyItem>(query, param, transaction);
        }
    }
}