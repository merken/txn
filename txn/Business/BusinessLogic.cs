using System.Collections.Generic;
using System.Threading.Tasks;
using txn.Models;

namespace txn.Business
{
    public class BusinessLogic : IService
    {
        private readonly MyItemsRepository repository;

        public BusinessLogic(MyItemsRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<MyItem>> BulkCreate()
        {
            var item1 = await repository.AddOrUpdate(new MyItem
            {
                Description = "Item 1"
            });

            var item2 = await repository.AddOrUpdate(new MyItem
            {
                Description = "Item 2"
            });

            var item3 = await repository.AddOrUpdate(new MyItem
            {
                Description = "Item 3"
            });

            return await repository.Search("Item ");
        }

        public async Task BulkDelete()
        {
            var items = await repository.Search("Item ");
            foreach (var item in items)
            {
                await repository.Delete(item.Id);
            }
        }
    }
}