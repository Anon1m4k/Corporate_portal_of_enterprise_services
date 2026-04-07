using ServiceHub.Models.Transport;

namespace ServiceHub.Tests.Unit.Model
{
    [TestClass]
    public class TransferStopTests
    {
        [TestMethod]
        public void TransferStop_TransferRouteIdRequired_ShouldFailWhenZero()
        {
            var stop = new TransferStop { TransferRouteId = 0 };
            var results = ModelValidator.ValidateModel(stop);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("TransferRouteId")));
        }

        [TestMethod]
        public void TransferStop_OrderRange_ShouldFailWhenOutOfRange()
        {
            var stop = new TransferStop { Order = 0 };
            var results = ModelValidator.ValidateModel(stop);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Order") && r.ErrorMessage.Contains("Порядковый номер должен быть от 1 до 100")));
        }

        [TestMethod]
        public void TransferStop_AddressRequired_ShouldFailWhenMissing()
        {
            var stop = new TransferStop { Address = null };
            var results = ModelValidator.ValidateModel(stop);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Address")));
        }

        [TestMethod]
        public void TransferStop_ArrivalTimeRequired_ShouldFailWhenDefault()
        {
            var stop = new TransferStop
            {
                TransferRouteId = 1, 
                Order = 1,           
                Address = "Test",   
                ArrivalTime = default
            };
            var results = ModelValidator.ValidateModel(stop);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("ArrivalTime")));
        }

        [TestMethod]
        public void TransferStop_WithValidData_ShouldBeValid()
        {
            var stop = new TransferStop
            {
                TransferRouteId = 1,
                Order = 2,
                Address = "ул. Ленина, 5",
                ArrivalTime = new TimeSpan(8, 30, 0)
            };
            var results = ModelValidator.ValidateModel(stop);
            Assert.AreEqual(0, results.Count);
        }
    }
}