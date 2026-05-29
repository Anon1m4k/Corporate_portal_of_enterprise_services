using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceHub.Models.Rooms;
using System;
using System.Linq;

namespace ServiceHub.Tests.Unit.Model.Rooms
{
    [TestClass]
    public class RoomBookingTimeTests
    {
        private DateTime _today = DateTime.Today;
        private DateTime _validFutureDate = DateTime.Today.AddDays(5);
        private TimeSpan _morning = new TimeSpan(10, 0, 0);
        private TimeSpan _noon = new TimeSpan(12, 0, 0);

        [TestMethod]
        public void RoomBookingTime_WithValidData_ShouldBeValid()
        {
            var booking = new RoomBookingTime
            {
                Date = _validFutureDate,
                StartTime = _morning,
                EndTime = _noon
            };
            var results = ModelValidator.ValidateModel(booking);
            Assert.AreEqual(0, results.Count);
        }

        // Date валидация
        [TestMethod]
        public void RoomBookingTime_DateInPast_ShouldFail()
        {
            var booking = new RoomBookingTime
            {
                Date = _today.AddDays(-1),
                StartTime = _morning,
                EndTime = _noon
            };
            var results = ModelValidator.ValidateModel(booking);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Date") &&
                                          r.ErrorMessage == "Дата не может быть раньше сегодняшнего дня"));
        }

        [TestMethod]
        public void RoomBookingTime_DateMoreThan30DaysAhead_ShouldFail()
        {
            var booking = new RoomBookingTime
            {
                Date = _today.AddDays(31),
                StartTime = _morning,
                EndTime = _noon
            };
            var results = ModelValidator.ValidateModel(booking);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Date") &&
                                          r.ErrorMessage == "Бронирование возможно не более чем на 30 дней вперёд"));
        }

        [TestMethod]
        public void RoomBookingTime_DateToday_ShouldBeValid()
        {
            var booking = new RoomBookingTime
            {
                Date = _today,
                StartTime = DateTime.Now.TimeOfDay.Add(TimeSpan.FromHours(1)),
                EndTime = DateTime.Now.TimeOfDay.Add(TimeSpan.FromHours(2))
            };
            var results = ModelValidator.ValidateModel(booking);
            Assert.AreEqual(0, results.Count);
        }

        // EndTime vs StartTime
        [TestMethod]
        public void RoomBookingTime_EndTimeBeforeStartTime_ShouldFail()
        {
            var booking = new RoomBookingTime
            {
                Date = _validFutureDate,
                StartTime = _noon,
                EndTime = _morning
            };
            var results = ModelValidator.ValidateModel(booking);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("EndTime") &&
                                          r.ErrorMessage == "Время окончания должно быть позже времени начала"));
        }

        [TestMethod]
        public void RoomBookingTime_EndTimeEqualsStartTime_ShouldFail()
        {
            var booking = new RoomBookingTime
            {
                Date = _validFutureDate,
                StartTime = _morning,
                EndTime = _morning
            };
            var results = ModelValidator.ValidateModel(booking);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("EndTime") &&
                                          r.ErrorMessage == "Время окончания должно быть позже времени начала"));
        }

        // Минимальная длительность 30 минут
        [TestMethod]
        public void RoomBookingTime_DurationLessThan30Minutes_ShouldFail()
        {
            var booking = new RoomBookingTime
            {
                Date = _validFutureDate,
                StartTime = _morning,
                EndTime = _morning.Add(TimeSpan.FromMinutes(25))
            };
            var results = ModelValidator.ValidateModel(booking);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("EndTime") &&
                                          r.ErrorMessage == "Минимальная длительность бронирования — 30 минут"));
        }

        [TestMethod]
        public void RoomBookingTime_DurationExactly30Minutes_ShouldBeValid()
        {
            var booking = new RoomBookingTime
            {
                Date = _validFutureDate,
                StartTime = _morning,
                EndTime = _morning.Add(TimeSpan.FromMinutes(30))
            };
            var results = ModelValidator.ValidateModel(booking);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void RoomBookingTime_DurationMoreThan30Minutes_ShouldBeValid()
        {
            var booking = new RoomBookingTime
            {
                Date = _validFutureDate,
                StartTime = _morning,
                EndTime = _noon
            };
            var results = ModelValidator.ValidateModel(booking);
            Assert.AreEqual(0, results.Count);
        }

        // Комбинация ошибок
        [TestMethod]
        public void RoomBookingTime_MultipleErrors_ShouldReturnAll()
        {
            var booking = new RoomBookingTime
            {
                Date = _today.AddDays(-1),
                StartTime = _noon,
                EndTime = _morning
            };
            var results = ModelValidator.ValidateModel(booking);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Date")));
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("EndTime") &&
                                          r.ErrorMessage.Contains("позже времени начала")));
        }
    }
}