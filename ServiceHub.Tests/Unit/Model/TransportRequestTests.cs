using ServiceHub.Models.Transport;

namespace ServiceHub.Tests.Unit.Model
{
    [TestClass]
    public class TransportRequestTests
    {
        [TestMethod]
        public void TransportRequest_DefaultStatus_ShouldBeOnApproval()
        {
            var request = new TransportRequest();
            Assert.AreEqual("На согласовании", request.Status);
        }

        [TestMethod]
        public void TransportRequest_DefaultCreatedAt_ShouldBeUtcNow()
        {
            var request = new TransportRequest();
            Assert.IsTrue((DateTime.UtcNow - request.CreatedAt).TotalSeconds < 2);
        }

        [TestMethod]
        public void TransportRequest_TripDateTimeRequired_ShouldFailWhenDefault()
        {
            var request = new TransportRequest
            {
                StartPoint = "Start",
                EndPoint = "End",
                PassengerCount = 1,
                Purpose = "Test",
                TripDateTime = default
            };
            var results = ModelValidator.ValidateModel(request);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("TripDateTime")));
        }

        [TestMethod]
        public void TransportRequest_StartPointRequired_ShouldFailWhenMissing()
        {
            var request = new TransportRequest { StartPoint = null };
            var results = ModelValidator.ValidateModel(request);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("StartPoint") && r.ErrorMessage == "Укажите пункт отправления"));
        }

        [TestMethod]
        public void TransportRequest_EndPointRequired_ShouldFailWhenMissing()
        {
            var request = new TransportRequest { EndPoint = null };
            var results = ModelValidator.ValidateModel(request);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("EndPoint") && r.ErrorMessage == "Укажите пункт назначения"));
        }

        [TestMethod]
        public void TransportRequest_PassengerCount_OutOfRange_ShouldFail()
        {
            var request = new TransportRequest { PassengerCount = 0 };
            var results = ModelValidator.ValidateModel(request);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("PassengerCount") && r.ErrorMessage.Contains("от 1 до 30")));
        }

        [TestMethod]
        public void TransportRequest_PurposeRequired_ShouldFailWhenMissing()
        {
            var request = new TransportRequest { Purpose = null };
            var results = ModelValidator.ValidateModel(request);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Purpose") && r.ErrorMessage == "Укажите цель поездки"));
        }

        [TestMethod]
        public void TransportRequest_WithValidData_ShouldBeValid()
        {
            var request = new TransportRequest
            {
                UserId = 1,
                TripDateTime = DateTime.Now.AddDays(1),
                StartPoint = "Офис",
                EndPoint = "Склад",
                PassengerCount = 2,
                Purpose = "Доставка документов",
                CarId = 1,
                Status = "На согласовании"
            };
            var results = ModelValidator.ValidateModel(request);
            Assert.AreEqual(0, results.Count);
        }
    }
}