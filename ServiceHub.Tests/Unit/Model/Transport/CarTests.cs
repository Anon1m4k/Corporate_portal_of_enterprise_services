using ServiceHub.Models.Transport;

namespace ServiceHub.Tests.Unit.Model.Transport
{
    [TestClass]
    public class CarTests
    {
        [TestMethod]
        public void Car_WithValidData_ShouldBeValid()
        {
            var car = new Car
            {
                Brand = "Toyota",
                Model = "Camry",
                LicensePlate = "А123ВС777",
                VehicleType = "Легковой",
                PassengerCapacity = 4,
                IsAvailable = true
            };
            var results = ModelValidator.ValidateModel(car);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void Car_BrandRequired_ShouldFailWhenMissing()
        {
            var car = new Car { Brand = null };
            var results = ModelValidator.ValidateModel(car);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Brand") && r.ErrorMessage == "Укажите марку автомобиля"));
        }

        [TestMethod]
        public void Car_ModelRequired_ShouldFailWhenMissing()
        {
            var car = new Car { Model = null };
            var results = ModelValidator.ValidateModel(car);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Model") && r.ErrorMessage == "Укажите модель автомобиля"));
        }

        [TestMethod]
        public void Car_LicensePlate_LengthExceedsMax_ShouldFail()
        {
            var car = new Car { LicensePlate = "СлишкомДлинныйНомер" };
            var results = ModelValidator.ValidateModel(car);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("LicensePlate") && r.ErrorMessage.Contains("не должен превышать 9 символов")));
        }

        [TestMethod]
        public void Car_VehicleTypeRequired_ShouldFailWhenMissing()
        {
            var car = new Car { VehicleType = null };
            var results = ModelValidator.ValidateModel(car);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("VehicleType") && r.ErrorMessage == "Укажите тип автомобиля"));
        }

        [TestMethod]
        public void Car_PassengerCapacity_OutOfRange_ShouldFail()
        {
            var car = new Car { PassengerCapacity = 0 };
            var results = ModelValidator.ValidateModel(car);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("PassengerCapacity") && r.ErrorMessage.Contains("от 1 до 30")));
        }

        [TestMethod]
        public void Car_IsAvailable_DefaultValue_ShouldBeTrue()
        {
            var car = new Car();
            Assert.IsTrue(car.IsAvailable);
        }
    }
}