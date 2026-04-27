using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceHub.Models.Account; // <-- ChangePasswordInput
using ServiceHub.Tests;          // <-- ModelValidator
using System.Linq;

namespace ServiceHub.Tests.Unit.Model
{
    [TestClass]
    public class ChangePasswordModelTests
    {
        [TestMethod]
        public void ChangePasswordInput_CurrentPasswordRequired_ShouldFailWhenMissing()
        {
            var input = new ChangePasswordModel { CurrentPassword = null };
            var results = ModelValidator.ValidateModel(input);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("CurrentPassword") && r.ErrorMessage == "Введите текущий пароль"));
        }

        [TestMethod]
        public void ChangePasswordInput_NewPasswordRequired_ShouldFailWhenMissing()
        {
            var input = new ChangePasswordModel { NewPassword = null };
            var results = ModelValidator.ValidateModel(input);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("NewPassword") && r.ErrorMessage == "Введите новый пароль"));
        }

        [TestMethod]
        public void ChangePasswordInput_NewPasswordLength_ShouldFailWhenTooShort()
        {
            var input = new ChangePasswordModel
            {
                NewPassword = "short"
            };
            var results = ModelValidator.ValidateModel(input);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("NewPassword") && r.ErrorMessage.Contains("от 8 до 25 символов")));
        }

        [TestMethod]
        public void ChangePasswordInput_ConfirmPasswordRequired_ShouldFailWhenMissing()
        {
            var input = new ChangePasswordModel { ConfirmPassword = null };
            var results = ModelValidator.ValidateModel(input);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("ConfirmPassword") && r.ErrorMessage == "Подтвердите новый пароль"));
        }

        [TestMethod]
        public void ChangePasswordInput_ConfirmPasswordMismatch_ShouldFail()
        {
            var input = new ChangePasswordModel
            {
                NewPassword = "ValidPass1!",
                ConfirmPassword = "DifferentPass1!"
            };
            var results = ModelValidator.ValidateModel(input);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("ConfirmPassword") && r.ErrorMessage == "Пароли не совпадают"));
        }

        [TestMethod]
        public void ChangePasswordInput_WithValidData_ShouldBeValid()
        {
            var input = new ChangePasswordModel
            {
                CurrentPassword = "Admin123!",
                NewPassword = "NewValidPass1!",
                ConfirmPassword = "NewValidPass1!"
            };
            var results = ModelValidator.ValidateModel(input);
            Assert.AreEqual(0, results.Count);
        }
    }
}