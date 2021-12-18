using mrs.Application.Common.Exceptions;
//using mrs.Application.TodoLists.Commands.CreateTodoList;
//using mrs.Application.TodoLists.Commands.UpdateTodoList;
using mrs.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace mrs.Application.IntegrationTests.TodoLists.Commands
{
    using static Testing;

    public class UpdateTodoListTests : TestBase
    {
        [Test]
        public void ShouldRequireValidTodoListId()
        {
            //var command = new UpdateTodoListCommand
            //{
            //    Id = 99,
            //    Title = "New Title"
            //};

            //FluentActions.Invoking(() =>
            //    SendAsync(command)).Should().Throw<NotFoundException>();
        }

        [Test]
        public async Task ShouldRequireUniqueTitle()
        {
            //var listId = await SendAsync(new CreateTodoListCommand
            //{
            //    Title = "New List"
            //});

            //await SendAsync(new CreateTodoListCommand
            //{
            //    Title = "Other List"
            //});

            //var command = new UpdateTodoListCommand
            //{
            //    Id = listId,
            //    Title = "Other List"
            //};

            //FluentActions.Invoking(() =>
            //    SendAsync(command))
            //        .Should().Throw<ValidationException>().Where(ex => ex.Errors.ContainsKey("Title"))
            //        .And.Errors["Title"].Should().Contain("The specified title already exists.");
        }

        [Test]
        public async Task ShouldUpdateTodoList()
        {
            //var userId = await RunAsDefaultUserAsync();

            //var listId = await SendAsync(new CreateTodoListCommand
            //{
            //    Title = "New List"
            //});

            //var command = new UpdateTodoListCommand
            //{
            //    Id = listId,
            //    Title = "Updated List Title"
            //};

            //await SendAsync(command);

            //var list = await FindAsync<TodoList>(listId);

            //list.Title.Should().Be(command.Title);
            //list.UpdatedBy.Should().NotBeNull();
            //list.UpdatedBy.Should().Be(userId);
            //list.UpdatedAt.Should().NotBeNull();
            //list.UpdatedAt.Should().BeCloseTo(DateTime.Now, 1000);
        }
    }
}
