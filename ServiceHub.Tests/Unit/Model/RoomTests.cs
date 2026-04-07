using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceHub.Models.Rooms;
using System.Linq;

namespace ServiceHub.Tests.Unit.Model
{
    [TestClass]
    public class RoomTests
    {
        [TestMethod]
        public void Room_WithValidData_ShouldBeValid()
        {
            var room = new Room
            {
                Name = "Переговорная 101",
                Location = "3 этаж, корпус А",
                Capacity = 10,
                Equipment = "Проектор, доска",
                IsActive = true
            };
            var results = ModelValidator.ValidateModel(room);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void Room_NameRequired_ShouldFailWhenMissing()
        {
            var room = new Room { Name = null };
            var results = ModelValidator.ValidateModel(room);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Name") && r.ErrorMessage == "Укажите название помещения"));
        }

        [TestMethod]
        public void Room_NameEmptyString_ShouldFail()
        {
            var room = new Room { Name = "" };
            var results = ModelValidator.ValidateModel(room);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Name") && r.ErrorMessage == "Укажите название помещения"));
        }

        [TestMethod]
        public void Room_CapacityOutOfRange_LessThanMin_ShouldFail()
        {
            var room = new Room { Capacity = 0 };
            var results = ModelValidator.ValidateModel(room);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Capacity") && r.ErrorMessage.Contains("от 1 до 100")));
        }

        [TestMethod]
        public void Room_CapacityOutOfRange_GreaterThanMax_ShouldFail()
        {
            var room = new Room { Capacity = 101 };
            var results = ModelValidator.ValidateModel(room);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Capacity") && r.ErrorMessage.Contains("от 1 до 100")));
        }

        [TestMethod]
        public void Room_LocationIsOptional_EmptyStringShouldBeValid()
        {
            var room = new Room
            {
                Name = "Актовый зал",
                Capacity = 50,
                Location = ""
            };
            var results = ModelValidator.ValidateModel(room);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void Room_EquipmentIsOptional_EmptyStringShouldBeValid()
        {
            var room = new Room
            {
                Name = "Комната отдыха",
                Capacity = 5,
                Equipment = ""
            };
            var results = ModelValidator.ValidateModel(room);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void Room_IsActive_DefaultTrue()
        {
            var room = new Room();
            Assert.IsTrue(room.IsActive);
        }

        [TestMethod]
        public void Room_RoomRequests_InitializedAsEmptyList()
        {
            var room = new Room();
            Assert.IsNotNull(room.RoomRequests);
            Assert.AreEqual(0, room.RoomRequests.Count);
        }
    }
}