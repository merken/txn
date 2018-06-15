using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace txn {
    public class UnitOfWorkFilter : ActionFilterAttribute {
        private readonly IDbTransaction transaction;

        public UnitOfWorkFilter (IDbTransaction transaction) {
            this.transaction = transaction;
        }

        public override async Task OnActionExecutionAsync (ActionExecutingContext executingContext, ActionExecutionDelegate next) {
            var connection = transaction.Connection;
            if (connection.State != ConnectionState.Open)
                throw new NotSupportedException ("The provided connection was not open!");

            var executedContext = await next.Invoke ();
            if (executedContext.Exception == null) {
                transaction.Commit ();
            } else {
                transaction.Rollback ();
            }
        }
    }
}