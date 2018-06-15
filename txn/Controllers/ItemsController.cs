using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using txn.Models;

namespace txn.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase {
        private readonly MyItemsRepository repository;

        public ItemsController (MyItemsRepository repository) {
            this.repository = repository;
        }

        [HttpGet]
        public Task<IEnumerable<MyItem>> Get () {
            return repository.GetAll ();
        }

        [HttpGet ("{id}")]
        public Task<MyItem> Get (Guid id) {
            return repository.GetById (id);
        }

        [HttpPost]
        public Task<MyItem> Post ([FromBody] MyItem item) {
            return repository.AddOrUpdate (item);
        }

        [HttpPost ("search")]
        public Task<IEnumerable<MyItem>> Search (string query) {
            return repository.Search (query);
        }

        [HttpPut ("{id}")]
        public Task<MyItem> Put (Guid id, [FromBody] MyItem item) {
            if (id != item.Id)
                throw new ArgumentException ("Id for the request must match the Id of the item");
            return repository.AddOrUpdate (item);
        }

        [HttpDelete ("{id}")]
        public Task Delete (Guid id) {
            return repository.Delete (id);
        }
    }
}