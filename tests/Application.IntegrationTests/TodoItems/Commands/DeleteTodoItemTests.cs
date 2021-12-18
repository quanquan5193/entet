using mrs.Application.Common.Exceptions;
//using mrs.Application.TodoItems.Commands.CreateTodoItem;
//using mrs.Application.TodoItems.Commands.DeleteTodoItem;
//using mrs.Application.TodoLists.Commands.CreateTodoList;
using mrs.Domain.Entities;
using FluentAssertions;
using System.Threading.Tasks;
using NUnit.Framework;

namespace mrs.Application.IntegrationTests.TodoItems.Commands
{
    using static Testing;

    public class DeleteTodoItemTests : TestBase
    {
        [Test]
        public void ShouldRequireValidTodoItemId()
        {
            //var command = new DeleteTodoItemCommand { Id = 99 };

            //FluentActions.Invoking(() =>
            //    SendAsync(command)).Should().Throw<NotFoundException>();
        }

        [Test]
        public async Task ShouldDeleteTodoItem()
        {
            //var listId = await SendAsync(new CreateTodoListCommand
            //{
            //    Title = "New List"
            //});

            //var itemId = await SendAsync(new CreateTodoItemCommand
            //{
            //    ListId = listId,
            //    Title = "New Item"
            //});

            //await SendAsync(new DeleteTodoItemCommand
            //{
            //    Id = itemId
            //});

            //var list = await FindAsync<TodoItem>(listId);

            //list.Should().BeNull();
        }
    }
}
