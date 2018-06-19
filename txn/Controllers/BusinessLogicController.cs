using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using txn.Business;
using txn.Models;

namespace txn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessLogicController : ControllerBase
    {
        private readonly BusinessLogic logic;

        public BusinessLogicController(BusinessLogic logic)
        {
            this.logic = logic;
        }

        [HttpPost("bulk-create")]
        public Task<IEnumerable<MyItem>> Post()
        {
            return logic.BulkCreate();
        }

        [HttpDelete("bulk-delete")]
        public Task Delete()
        {
            return logic.BulkDelete();
        }
    }
}