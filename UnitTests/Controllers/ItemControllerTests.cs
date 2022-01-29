﻿using DotNetNuke.Entities.Users;
using Eraware.Modules.MyModule.Controllers;
using Eraware.Modules.MyModule.DTO;
using Eraware.Modules.MyModule.Services;
using Eraware.Modules.MyModule.ViewModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Web.Http.Results;
using Xunit;

namespace UnitTests.Controllers
{
    public class ItemControllerTests
    {
        private readonly Mock<IItemService> itemService;
        private readonly ItemController itemController;


        public ItemControllerTests()
        {
            this.itemService = new Mock<IItemService>();
            this.itemController = new FakeItemController(this.itemService.Object);
        }

        [Fact]
        public void CreateItem_Creates()
        {
            var name = "Name";
            var description = "Description";
            var dto = new CreateItemDTO()
            {
                Name = name,
                Description = description,
            };
            var viewModel = new ItemViewModel() { Id = 1, Name = name, Description = description };
            this.itemService.Setup(i => i.CreateItem(It.IsAny<CreateItemDTO>(), It.IsAny<int>()))
                .Returns(new ItemViewModel() { Id = 1, Name = name, Description = description });

            var result = this.itemController.CreateItem(dto);

            var response = Assert.IsType<OkNegotiatedContentResult<ItemViewModel>>(result);
            Assert.Equal(1, response.Content.Id);
            Assert.Equal(name, response.Content.Name);
            Assert.Equal(description, response.Content.Description);
        }

        [Fact]
        public void GetItemsPage_GetsProperPages()
        {
            var items = new List<ItemViewModel>();
            for (int i = 0; i < 100; i++)
            {
                var item = new ItemViewModel() { Id = i, Name = $"Name {i}", Description = $"Description {1}" };
                items.Add(item);
            }
            var itemsPageViewModel = new ItemsPageViewModel() { Items = items, Page = 1, PageCount = 10, ResultCount = 100 };
            this.itemService.Setup(i => i.GetItemsPage(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(itemsPageViewModel);
            var dto = new GetItemsPageDTO
            {
                Query = "Name",
                Page = 1,
                PageSize = 10,
                Descending = true,
            };

            var result = this.itemController.GetItemsPage(dto);

            var response = Assert.IsType<OkNegotiatedContentResult<ItemsPageViewModel>>(result);
            Assert.Equal(100, response.Content.Items.Count);
            Assert.Equal(1, response.Content.Page);
            Assert.Equal(10, response.Content.PageCount);
            Assert.Equal(100, response.Content.ResultCount);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        public void DeleteItem_Deletes(int itemId)
        {
            var result = this.itemController.DeleteItem(itemId);

            Assert.IsType<OkResult>(result);
            this.itemService.Verify(i => i.DeleteItem(itemId), Times.Once);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void UserCanEdit_ReturnsBool(bool canEdit)
        {
            this.itemController.CanEdit = canEdit;

            var result = this.itemController.UserCanEdit();

            var response = Assert.IsType<OkNegotiatedContentResult<bool>>(result);
            Assert.Equal(canEdit, response.Content);
        }

        [Fact]
        public void UpdateItem_Updates()
        {
            var item = new UpdateItemDTO
            {
                Id = 123,
                Name = "Edited Item",
                Description = "This item was edited",
            };

            this.itemController.UpdateItem(item);

            this.itemService.Verify(s => s.UpdateItem(item, It.IsAny<int>()), Times.Once);
        }

        public class FakeItemController : ItemController
        {
            private bool canEdit = false;

            public readonly IItemService itemService;
            public FakeItemController(IItemService itemService)
                : base(itemService)
            {
                this.itemService = itemService;
            }
            public override UserInfo UserInfo => new UserInfo() { UserID = 123 };

            public override bool CanEdit
            {
                get { return this.canEdit; }
                set { this.canEdit = value; }
            }
        }

    }
}
