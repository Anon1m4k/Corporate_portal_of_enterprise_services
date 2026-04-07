using ServiceHub.Models.Transport;

namespace ServiceHub.Tests.Unit.Model
{
    [TestClass]
    public class TransferRouteTests
    {
        [TestMethod]
        public void TransferRoute_NameRequired_ShouldFailWhenMissing()
        {
            var route = new TransferRoute { Name = null };
            var results = ModelValidator.ValidateModel(route);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Name")));
        }

        [TestMethod]
        public void TransferRoute_CarIdRequired_ShouldFailWhenZero()
        {
            var route = new TransferRoute { CarId = 0 };
            var results = ModelValidator.ValidateModel(route);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("CarId")));
        }

        [TestMethod]
        public void TransferRoute_IsActive_DefaultTrue()
        {
            var route = new TransferRoute();
            Assert.IsTrue(route.IsActive);
        }

        [TestMethod]
        public void TransferRoute_WithValidData_ShouldBeValid()
        {
            var route = new TransferRoute
            {
                Name = "Утренний маршрут",
                CarId = 1,
                IsActive = true
            };
            var results = ModelValidator.ValidateModel(route);
            Assert.AreEqual(0, results.Count);
        }
    }
}