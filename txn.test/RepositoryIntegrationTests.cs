using System;
using System.Threading.Tasks;
using FluentAssertions;
using txn.Models;
using Xunit;

namespace txn.test
{
    public class RepositoryIntegrationTests : IntegrationTest
    {
        public RepositoryIntegrationTests(LocalDbFixture fixture) : base(fixture) { }

        [Fact]
        public async Task Create_Data_Within_RollBacked_Transaction()
        {
            await WithinTestContext(async (ctx) =>
            {
                var itemsInDb = await ctx.ItemsRepository.GetAll();
                itemsInDb.Should().HaveCount(0);
                await ctx.ItemsRepository.AddOrUpdate(new MyItem { Description = "Test1" });
                itemsInDb = await ctx.ItemsRepository.GetAll();
                itemsInDb.Should().HaveCount(1);
            });

            await WithinTestContext(async (ctx) =>
            {
                var itemsInDb = await ctx.ItemsRepository.GetAll();
                itemsInDb.Should().HaveCount(0);
            });
        }

        [Fact]
        public async Task Search_Test()
        {
            await WithinTestContext(async (ctx) =>
            {
                await ctx.ItemsRepository.AddOrUpdate(new MyItem { Description = "Test 1" });
                await ctx.ItemsRepository.AddOrUpdate(new MyItem { Description = "Test 2" });
                await ctx.ItemsRepository.AddOrUpdate(new MyItem { Description = "Test 3" });
                await ctx.ItemsRepository.AddOrUpdate(new MyItem { Description = "Something else" });

                var itemsInDb = await ctx.ItemsRepository.Search("Test ");
                itemsInDb.Should().HaveCount(3);
            });
        }

        [Fact]
        public async Task Update_Test()
        {
            await WithinTestContext(async (ctx) =>
            {
                var item = await ctx.ItemsRepository.AddOrUpdate(new MyItem { Description = "Test 1" });
                await ctx.ItemsRepository.AddOrUpdate(new MyItem { Id = item.Id, Description = "Test 2" });
                item = await ctx.ItemsRepository.GetById(item.Id);

                item.Description.Should().Be("Test 2");
                var itemsInDb = await ctx.ItemsRepository.GetAll();
                itemsInDb.Should().HaveCount(1);
            });
        }
    }
}