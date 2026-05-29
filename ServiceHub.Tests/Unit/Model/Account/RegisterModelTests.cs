using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceHub.Models.Account;

namespace ServiceHub.Tests.Unit.Model.Account
{
    [TestClass]
    public class RegisterModelTests
    {
        [TestMethod]
        public void RegisterModel_EmailRequired_ShouldFailWhenMissing()
        {
            var model = new RegisterModel { Email = null };
            var results = ModelValidator.ValidateModel(model);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Email") && r.ErrorMessage == "Не указан Email"));
        }

        [TestMethod]
        public void RegisterModel_EmailFormat_ShouldFailWhenInvalid()
        {
            var model = new RegisterModel { Email = "invalid" };
            var results = ModelValidator.ValidateModel(model);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Email") && r.ErrorMessage.Contains("Некорректный формат Email")));
        }

        [TestMethod]
        public void RegisterModel_EmailLengthExceedsMax_ShouldFail()
        {
            var model = new RegisterModel { Email = new string('a', 51) + "@test.com" };
            var results = ModelValidator.ValidateModel(model);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Email") && r.ErrorMessage.Contains("не должен превышать 50 символов")));
        }

        [TestMethod]
        public void RegisterModel_PasswordComplexity_ShouldFailWhenTooSimple()
        {
            var model = new RegisterModel
            {
                Email = "test@test.com",
                Password = "simple",
                ConfirmPassword = "simple",
                FirstName = "Иван",
                LastName = "Иванов",
                Department = "IT"
            };
            var results = ModelValidator.ValidateModel(model);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Password") && r.ErrorMessage.Contains("заглавные и строчные буквы, цифры и специальные символы")));
        }

        [TestMethod]
        public void RegisterModel_PasswordLength_ShouldFailWhenTooShort()
        {
            var model = new RegisterModel
            {
                Email = "test@test.com",
                Password = "Pass1!",
                ConfirmPassword = "Pass1!",
                FirstName = "Иван",
                LastName = "Иванов",
                Department = "IT"
            };
            var results = ModelValidator.ValidateModel(model);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Password") && r.ErrorMessage.Contains("от 8 до 25 символов")));
        }

        [TestMethod]
        public void RegisterModel_PasswordMismatch_ShouldFail()
        {
            var model = new RegisterModel
            {
                Email = "test@test.com",
                Password = "ValidPass1!",
                ConfirmPassword = "DifferentPass1!",
                FirstName = "Иван",
                LastName = "Иванов",
                Department = "IT"
            };
            var results = ModelValidator.ValidateModel(model);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("ConfirmPassword") && r.ErrorMessage == "Пароли не совпадают"));
        }

        [TestMethod]
        public void RegisterModel_FirstNameRequired_ShouldFailWhenMissing()
        {
            var model = new RegisterModel { FirstName = null };
            var results = ModelValidator.ValidateModel(model);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("FirstName") && r.ErrorMessage == "Не указано имя"));
        }

        [TestMethod]
        public void RegisterModel_LastNameRequired_ShouldFailWhenMissing()
        {
            var model = new RegisterModel { LastName = null };
            var results = ModelValidator.ValidateModel(model);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("LastName") && r.ErrorMessage == "Не указана фамилия"));
        }

        [TestMethod]
        public void RegisterModel_DepartmentRequired_ShouldFailWhenMissing()
        {
            var model = new RegisterModel { Department = null };
            var results = ModelValidator.ValidateModel(model);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Department") && r.ErrorMessage == "Не указан отдел"));
        }

        [TestMethod]
        public void RegisterModel_WithValidData_ShouldBeValid()
        {
            var model = new RegisterModel
            {
                Email = "test@example.com",
                Password = "ValidPass1!",
                ConfirmPassword = "ValidPass1!",
                FirstName = "Иван",
                LastName = "Иванов",
                Department = "IT"
            };
            var results = ModelValidator.ValidateModel(model);
            Assert.AreEqual(0, results.Count);
        }
    }
}