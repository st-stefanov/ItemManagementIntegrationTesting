using NUnit.Framework;
using Moq;
using ItemManagementApp.Services;
using ItemManagementLib.Repositories;
using ItemManagementLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace ItemManagement.Tests
{
    [TestFixture]
    public class ItemServiceTests
    {
        private ItemService _itemService;
        private Mock<IItemRepository> _mockItemRepository;

        [SetUp]
        public void Setup()
        {
            // Arrange: Create a mock instance of IItemRepository
            _mockItemRepository = new Mock<IItemRepository>();
            // Instantiate ItemService with the mocked repository
            _itemService = new ItemService(_mockItemRepository.Object);
        }

        [Test]
        public void AddItem_ShouldAddItem_IfNameIsValid()
        {
            //Arrange
            var item = new Item { Name = "TestName" };
            _mockItemRepository.Setup(x => x.AddItem(It.IsAny<Item>()));
            //Act
            _itemService.AddItem(item.Name);
            //Assert
            _mockItemRepository.Verify(x => x.AddItem(It.IsAny<Item>()), Times.Once);
        }

        [Test]
        public void AddItem_ShouldThrowError_IfNameIsInvalid()
        {
            //Arrange
            string invalidName = "";
            _mockItemRepository.Setup(x => x.AddItem(It.IsAny<Item>())).Throws<ArgumentException>();
            //Act and Assert
            Assert.Throws<ArgumentException>(() => _itemService.AddItem(invalidName));           
            _mockItemRepository.Verify(x => x.AddItem(It.IsAny<Item>()), Times.Once);
        }

        [Test]
        public void GetAllItems_ShouldReturnAllItems()
        {
            //Arrange
            var items = new List<Item>() { new Item { Id = 1, Name = "SampleItem"} };

            //Act
            _mockItemRepository.Setup(x => x.GetAllItems()).Returns(items);

            //Assert
            var result = _itemService.GetAllItems();
            Assert.NotNull(result);
            Assert.That(result.Count, Is.EqualTo(1));
            _mockItemRepository.Verify(x => x.GetAllItems(), Times.Once());
        }

        [Test]
        public void GetById_ShouldReturnItemById_IfItemExists()
        {
            var item = new Item() { Id = 1, Name = "SingleName" };
            _mockItemRepository.Setup(x => x.GetItemById(item.Id)).Returns(item);

            var result = _itemService.GetItemById(item.Id);

            Assert.NotNull(result);
            Assert.That(result.Name, Is.EqualTo(item.Name));
            _mockItemRepository.Verify(x => x.GetItemById(item.Id), Times.Once());
        }

        [Test]
        public void GetById_ShouldReturnNull_IfItemDoesNotExists()
        {
            Item item = null;
            _mockItemRepository.Setup(x => x.GetItemById(It.IsAny<int>())).Returns(item);

            var result = _itemService.GetItemById(123);

            Assert.Null(result);
            _mockItemRepository.Verify(x => x.GetItemById(It.IsAny<int>()), Times.Once());

        }

        [Test]
        public void UpdateItem_ShouldNotUpdateItem_IfDoesNotExist()
        {
            var nonExistingId = 1;
            _mockItemRepository.Setup(x => x.GetItemById(nonExistingId)).Returns<Item>(null);
            _mockItemRepository.Setup(x => x.UpdateItem(It.IsAny<Item>()));

            _itemService.UpdateItem(nonExistingId, "Does Not Matter");

            _mockItemRepository.Verify(x => x.GetItemById(nonExistingId), Times.Once());
            _mockItemRepository.Verify(x => x.UpdateItem(It.IsAny<Item>()), Times.Never());

        }

        [Test]
        public void UpdateItem_ShouldThrowException_IfItemNameIsInvalid()
        {
            var item = new Item { Name = "SampleItem", Id = 1} ;
            _mockItemRepository.Setup(x => x.GetItemById(item.Id)).Returns(item);
            _mockItemRepository.Setup(x => x.UpdateItem(It.IsAny<Item>())).Throws<ArgumentException>();

            Assert.Throws<ArgumentException>(() => _itemService.UpdateItem(item.Id, ""));
            
            _mockItemRepository.Verify(x => x.GetItemById(item.Id), Times.Once());
            _mockItemRepository.Verify(x => x.UpdateItem(It.IsAny<Item>()), Times.Once());

        }

        [Test]
        public void UpdateItem_ShouldUpdateItem_IfItemNameIfValid()
        {
            var item = new Item { Name = "SampleItem", Id = 1 };
            _mockItemRepository.Setup(x => x.GetItemById(item.Id)).Returns(item);
            _mockItemRepository.Setup(x => x.UpdateItem(It.IsAny<Item>()));

            _itemService.UpdateItem(item.Id, "SampleItem UPDATED");

            _mockItemRepository.Verify(x => x.GetItemById(item.Id), Times.Once());
            _mockItemRepository.Verify(x => x.UpdateItem(It.IsAny<Item>()), Times.Once());

        }

        [Test]
        public void DeleteItem_ShouldDeleteItem()
        {
            var itemId = 123;
            _mockItemRepository.Setup(x => x.DeleteItem(itemId));

            _itemService.DeleteItem(itemId);

            _mockItemRepository.Verify(x => x.DeleteItem(itemId), Times.Once);

        }

        [TestCase("", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase("aaaaaaaaaaaaaaaaaa", false)]
        [TestCase("A", true)]
        [TestCase("SampleName", true)]
        [TestCase("Sample", true)]
        public void ValidateItemName_ShouldReturnCorrectAnswer_IfItemNameIsValid(string itemName, bool isValid) 
        {
            var result = _itemService.ValidateItemName(itemName);

            Assert.That(result, Is.EqualTo(isValid));

        }
    }
}