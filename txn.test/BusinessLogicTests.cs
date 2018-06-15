using System;
using System.Threading.Tasks;
using FluentAssertions;
using txn.Models;
using Xunit;

namespace txn.test {
    public class BusinessLogicTests : IntegrationTest {
        public BusinessLogicTests (LocalDbFixture fixture) : base (fixture) { }

        [Fact]
        public async Task Bulk_Create_Must_Create_3_Items () {
            await WithinTestContext (async (ctx) => {
                await ctx.BusinessLogic.BulkCreate ();

                var itemsInDb = await ctx.ItemsRepository.GetAll ();

                itemsInDb.Should ().HaveCount (3);
            });
        }

        [Fact]
        public async Task Bulk_Delete_Must_Delete_Correct_Items () {
            await WithinTestContext (async (ctx) => {
                await ctx.BusinessLogic.BulkCreate ();
                await ctx.ItemsRepository.AddOrUpdate (new MyItem { Description = "outside of business logic" });
                await ctx.BusinessLogic.BulkDelete ();

                var itemsInDb = await ctx.ItemsRepository.GetAll ();

                itemsInDb.Should ().HaveCount (1);
            });
        }
    }
}