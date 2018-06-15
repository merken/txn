using System;
using System.Threading.Tasks;
using FluentAssertions;
using txn.Controllers;
using txn.Models;
using Xunit;

namespace txn.test {
    public class ControllerTests : IntegrationTest {
        public ControllerTests (LocalDbFixture fixture) : base (fixture) { }

        [Fact]
        public async Task Create_Item_Via_Controller () {
            await WithinTestContext (async (ctx) => {
                var controller = new ItemsController (ctx.ItemsRepository);

                var itemsInDb = await controller.Get ();
                itemsInDb.Should ().HaveCount (0);
                await controller.Post (new MyItem { Description = "Test1" });
                itemsInDb = await controller.Get ();

                itemsInDb.Should ().HaveCount (1);
            });
        }

        [Fact]
        public async Task Update_Item_Via_Controller () {
            await WithinTestContext (async (ctx) => {
                var controller = new ItemsController (ctx.ItemsRepository);
                var itemInDb = await controller.Post (new MyItem { Description = "Test1" });
                itemInDb = await controller.Put (itemInDb.Id, new MyItem { Id = itemInDb.Id, Description = "Test2" });

                var itemsInDb = await controller.Get ();
                itemInDb.Description.Should ().Be ("Test2");
                itemsInDb.Should ().HaveCount (1);
            });
        }

        [Fact]
        public async Task Delete_Item_Via_Controller () {
            await WithinTestContext (async (ctx) => {
                var controller = new ItemsController (ctx.ItemsRepository);
                var itemInDb = await controller.Post (new MyItem { Description = "Test1" });
                await controller.Delete (itemInDb.Id);

                var itemsInDb = await controller.Get ();
                itemsInDb.Should ().HaveCount (0);
            });
        }

        [Fact]
        public async Task Search_Items_Via_Controller () {
            await WithinTestContext (async (ctx) => {
                var controller = new ItemsController (ctx.ItemsRepository);
                await controller.Post (new MyItem { Description = "Test 1" });
                await controller.Post (new MyItem { Description = "Test 2" });
                await controller.Post (new MyItem { Description = "Test 3" });
                await controller.Post (new MyItem { Description = "Test 4" });
                await controller.Post (new MyItem { Description = "Something else" });

                var itemsInDb = await controller.Search ("Test ");
                itemsInDb.Should ().HaveCount (4);
            });
        }
    }
}