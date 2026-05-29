using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceHub.Models.Account;

namespace ServiceHub.Tests.Unit.Model.Account
{
    [TestClass]
    public class LoginModelTests
    {
        [TestMethod]
        public void LoginModel_EmailRequired_ShouldFailWhenMissing()
        {
            var model = new LoginModel { Email = null };
            var results = ModelValidator.ValidateModel(model);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Email") && r.ErrorMessage == "Не указан Email"));
        }

        [TestMethod]
        public void LoginModel_EmailFormat_ShouldFailWhenInvalid()
        {
            var model = new LoginModel { Email = "invalid" };
            var results = ModelValidator.ValidateModel(model);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Email") && r.ErrorMessage.Contains("Некорректный формат Email")));
        }

        [TestMethod]
        public void LoginModel_PasswordRequired_ShouldFailWhenMissing()
        {
            var model = new LoginModel { Password = null };
            var results = ModelValidator.ValidateModel(model);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Password") && r.ErrorMessage == "Не указан пароль"));
        }

        [TestMethod]
        public void LoginModel_WithValidData_ShouldBeValid()
        {
            var model = new LoginModel
            {
                Email = "test@example.com",
                Password = "password123"
            };
            var results = ModelValidator.ValidateModel(model);
            Assert.AreEqual(0, results.Count);
        }
    }
}