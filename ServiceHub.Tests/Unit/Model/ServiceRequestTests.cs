using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceHub.Models;
using System;

namespace ServiceHub.Tests.Unit.Model
{
    [TestClass]
    public class ServiceRequestTests
    {
        [TestMethod]
        public void ServiceRequest_DefaultStatus_ShouldBeWaiting()
        {
            var request = new ServiceRequest();
            Assert.AreEqual("Ожидание", request.Status);
        }

        [TestMethod]
        public void ServiceRequest_DefaultCreatedAt_ShouldBeUtcNow()
        {
            var request = new ServiceRequest();
            Assert.IsTrue((DateTime.UtcNow - request.CreatedAt).TotalSeconds < 2);
        }

        [TestMethod]
        public void ServiceRequest_ServiceTypeRequired_ShouldFailWhenMissing()
        {
            var request = new ServiceRequest { ServiceType = null };
            var results = ModelValidator.ValidateModel(request);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("ServiceType")));
        }

        [TestMethod]
        public void ServiceRequest_TitleRequired_ShouldFailWhenMissing()
        {
            var request = new ServiceRequest { Title = null };
            var results = ModelValidator.ValidateModel(request);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Title")));
        }

        [TestMethod]
        public void ServiceRequest_WithValidData_ShouldBeValid()
        {
            var request = new ServiceRequest
            {
                ServiceType = "Транспорт",
                Title = "Поездка",
                Description = "Описание",
                Status = "На согласовании",
                UserId = 1
            };
            var results = ModelValidator.ValidateModel(request);
            Assert.AreEqual(0, results.Count);
        }
    }
}