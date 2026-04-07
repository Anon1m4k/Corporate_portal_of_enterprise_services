using ServiceHub.Models.Rooms;

namespace ServiceHub.Tests.Unit.Model
{
    [TestClass]
    public class RoomRequestTests
    {
        private DateTime _futureStart = DateTime.Now.AddDays(1).AddHours(10);
        private DateTime _futureEnd = DateTime.Now.AddDays(1).AddHours(12);

        [TestMethod]
        public void RoomRequest_WithValidData_ShouldBeValid()
        {
            var request = new RoomRequest
            {
                UserId = 1,
                RoomId = 5,
                StartTime = _futureStart,
                EndTime = _futureEnd,
                ParticipantsCount = 10,
                Topic = "Планирование спринта",
                Description = "Нужен проектор"
            };
            var results = ModelValidator.ValidateModel(request);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void RoomRequest_DefaultStatus_ShouldBeOnApproval()
        {
            var request = new RoomRequest();
            Assert.AreEqual("На согласовании", request.Status);
        }

        [TestMethod]
        public void RoomRequest_DefaultCreatedAt_ShouldBeUtcNow()
        {
            var request = new RoomRequest();
            Assert.IsTrue((DateTime.UtcNow - request.CreatedAt).TotalSeconds < 2);
        }       

        // StartTime: проверяем кастомную валидацию (прошлое время)
        [TestMethod]
        public void RoomRequest_StartTimeInPast_ShouldFail()
        {
            var request = new RoomRequest
            {
                StartTime = DateTime.Now.AddHours(-1),
                EndTime = DateTime.Now.AddHours(1),
                ParticipantsCount = 5,
                Topic = "Test"
            };
            var results = ModelValidator.ValidateModel(request);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("StartTime") && r.ErrorMessage == "Нельзя бронировать помещение на прошедшее время"));
        }

        // StartTime = default → тоже прошлое, ловим той же кастомной валидацией
        [TestMethod]
        public void RoomRequest_StartTimeDefault_ShouldFailAsPast()
        {
            var request = new RoomRequest
            {
                StartTime = default,
                EndTime = _futureEnd,
                ParticipantsCount = 5,
                Topic = "Test"
            };
            var results = ModelValidator.ValidateModel(request);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("StartTime") && r.ErrorMessage == "Нельзя бронировать помещение на прошедшее время"));
        }

        // EndTime: проверяем кастомную валидацию (должно быть позже StartTime)
        [TestMethod]
        public void RoomRequest_EndTimeBeforeStartTime_ShouldFail()
        {
            var request = new RoomRequest
            {
                StartTime = _futureEnd,
                EndTime = _futureStart,
                ParticipantsCount = 5,
                Topic = "Test"
            };
            var results = ModelValidator.ValidateModel(request);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("EndTime") && r.ErrorMessage == "Время окончания должно быть позже времени начала"));
        }

        [TestMethod]
        public void RoomRequest_EndTimeEqualsStartTime_ShouldFail()
        {
            var sameTime = _futureStart;
            var request = new RoomRequest
            {
                StartTime = sameTime,
                EndTime = sameTime,
                ParticipantsCount = 5,
                Topic = "Test"
            };
            var results = ModelValidator.ValidateModel(request);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("EndTime") && r.ErrorMessage == "Время окончания должно быть позже времени начала"));
        }

        // ParticipantsCount: проверяем [Range]
        [TestMethod]
        public void RoomRequest_ParticipantsCount_Zero_ShouldFailWithRange()
        {
            var request = new RoomRequest { ParticipantsCount = 0 };
            var results = ModelValidator.ValidateModel(request);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("ParticipantsCount") && r.ErrorMessage.Contains("от 1 до 100")));
        }

        [TestMethod]
        public void RoomRequest_ParticipantsCount_101_ShouldFailWithRange()
        {
            var request = new RoomRequest { ParticipantsCount = 101 };
            var results = ModelValidator.ValidateModel(request);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("ParticipantsCount") && r.ErrorMessage.Contains("от 1 до 100")));
        }

        // Topic: строка с [Required]
        [TestMethod]
        public void RoomRequest_TopicRequired_ShouldFailWhenMissing()
        {
            var request = new RoomRequest { Topic = null };
            var results = ModelValidator.ValidateModel(request);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Topic") && r.ErrorMessage == "Укажите тему встречи"));
        }

        [TestMethod]
        public void RoomRequest_TopicEmptyString_ShouldFail()
        {
            var request = new RoomRequest { Topic = "" };
            var results = ModelValidator.ValidateModel(request);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Topic") && r.ErrorMessage == "Укажите тему встречи"));
        }

        [TestMethod]
        public void RoomRequest_DescriptionIsOptional_NullShouldBeValid()
        {
            var request = new RoomRequest
            {
                UserId = 1,
                RoomId = 1,
                StartTime = _futureStart,
                EndTime = _futureEnd,
                ParticipantsCount = 5,
                Topic = "Совещание",
                Description = null
            };
            var results = ModelValidator.ValidateModel(request);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void RoomRequest_UpdatedAt_NullByDefault()
        {
            var request = new RoomRequest();
            Assert.IsNull(request.UpdatedAt);
        }

        [TestMethod]
        public void RoomRequest_ApprovedAt_NullByDefault()
        {
            var request = new RoomRequest();
            Assert.IsNull(request.ApprovedAt);
        }

        [TestMethod]
        public void RoomRequest_ApproverId_NullByDefault()
        {
            var request = new RoomRequest();
            Assert.IsNull(request.ApproverId);
        }
    }
}