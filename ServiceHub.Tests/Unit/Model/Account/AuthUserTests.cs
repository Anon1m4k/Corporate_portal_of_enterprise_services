using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceHub.Models.Account;
using System;

namespace ServiceHub.Tests.Unit.Model.Account
{
    [TestClass]
    public class AuthUserTests
    {
        [TestMethod]
        public void AuthUser_EmailRequired_ShouldFailWhenMissing()
        {
            var user = new AuthUser { Email = null };
            var results = ModelValidator.ValidateModel(user);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Email")));
        }

        [TestMethod]
        public void AuthUser_EmailFormat_ShouldFailWhenInvalid()
        {
            var user = new AuthUser { Email = "invalid" };
            var results = ModelValidator.ValidateModel(user);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Email") && r.ErrorMessage.Contains("Некорректный формат Email")));
        }

        [TestMethod]
        public void AuthUser_PasswordRequired_ShouldFailWhenMissing()
        {
            var user = new AuthUser { Password = null };
            var results = ModelValidator.ValidateModel(user);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Password")));
        }

        [TestMethod]
        public void AuthUser_Role_DefaultUser()
        {
            var user = new AuthUser();
            Assert.AreEqual("User", user.Role);
        }

        [TestMethod]
        public void AuthUser_IsActive_DefaultTrue()
        {
            var user = new AuthUser();
            Assert.IsTrue(user.IsActive);
        }

        [TestMethod]
        public void AuthUser_CreatedAt_DefaultUtcNow()
        {
            var user = new AuthUser();
            Assert.IsTrue((DateTime.UtcNow - user.CreatedAt).TotalSeconds < 2);
        }

        [TestMethod]
        public void AuthUser_WithValidData_ShouldBeValid()
        {
            var user = new AuthUser
            {
                Email = "test@example.com",
                Password = "Admin123!",
                FirstName = "Иван",
                LastName = "Иванов",
                Department = "IT",
                Role = "User",
                IsActive = true
            };
            var results = ModelValidator.ValidateModel(user);
            Assert.AreEqual(0, results.Count);
        }
    }
}