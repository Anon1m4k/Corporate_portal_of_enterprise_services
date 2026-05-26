using ServiceHub.Models.Account;
using ServiceHub.Models.Rooms;
using ServiceHub.Models.Transport;

namespace ServiceHub.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            if (context.Users.Any() || context.Cars.Any())
                return;

            // --- Пользователи ---
            var users = new AuthUser[]
            {
                new AuthUser
                {
                    Email = "admin@servicehub.local",
                    FirstName = "Админ",
                    LastName = "Админов",
                    Department = "АСУ",
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new AuthUser
                {
                    Email = "ivanov@servicehub.local",
                    FirstName = "Иван",
                    LastName = "Иванов",
                    Department = "Диспетчерская",
                    Role = "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new AuthUser
                {
                    Email = "petrov@servicehub.local",
                    FirstName = "Пётр",
                    LastName = "Петров",
                    Department = "Складское хозяйство",
                    Role = "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new AuthUser
                {
                    Email = "nachalnik@servicehub.local",
                    FirstName = "Сергей",
                    LastName = "Кузнецов",
                    Department = "Транспортный отдел",
                    Role = "Chief",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                // Дополнительные пользователи
                new AuthUser
                {
                    Email = "smirnova@servicehub.local",
                    FirstName = "Анна",
                    LastName = "Смирнова",
                    Department = "Бухгалтерия",
                    Role = "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new AuthUser
                {
                    Email = "egorov@servicehub.local",
                    FirstName = "Дмитрий",
                    LastName = "Егоров",
                    Department = "Диспетчерская",
                    Role = "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new AuthUser
                {
                    Email = "zueva@servicehub.local",
                    FirstName = "Елена",
                    LastName = "Зуева",
                    Department = "ИТ-отдел",
                    Role = "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };
            users[0].HashPassword("Admin123!");
            users[1].HashPassword("User123!");
            users[2].HashPassword("User123!");
            users[3].HashPassword("User123!");
            users[4].HashPassword("User123!");
            users[5].HashPassword("User123!");
            users[6].HashPassword("User123!");

            context.Users.AddRange(users);
            context.SaveChanges();

            // --- Автомобили ---
            var cars = new Car[]
            {
                new Car
                {
                    Brand = "Toyota",
                    Model = "Camry",
                    LicensePlate = "А123ВС777",
                    VehicleType = "Легковой",
                    PassengerCapacity = 4,
                    IsAvailable = true
                },
                new Car
                {
                    Brand = "Hyundai",
                    Model = "H-1",
                    LicensePlate = "В456УК178",
                    VehicleType = "Минивэн",
                    PassengerCapacity = 8,
                    IsAvailable = true
                },
                new Car
                {
                    Brand = "Ford",
                    Model = "Transit",
                    LicensePlate = "К789ЕН799",
                    VehicleType = "Автобус",
                    PassengerCapacity = 16,
                    IsAvailable = true
                },
                new Car
                {
                    Brand = "Kia",
                    Model = "Rio",
                    LicensePlate = "М012ХХ777",
                    VehicleType = "Легковой",
                    PassengerCapacity = 4,
                    IsAvailable = false
                },
                new Car
                {
                    Brand = "Mercedes",
                    Model = "Sprinter",
                    LicensePlate = "О345ТТ199",
                    VehicleType = "Автобус",
                    PassengerCapacity = 20,
                    IsAvailable = true
                },
                // Дополнительные автомобили
                new Car
                {
                    Brand = "Volkswagen",
                    Model = "Polo",
                    LicensePlate = "У567ВС178",
                    VehicleType = "Легковой",
                    PassengerCapacity = 4,
                    IsAvailable = true
                },
                new Car
                {
                    Brand = "Ford",
                    Model = "Transit Connect",
                    LicensePlate = "Т890КМ199",
                    VehicleType = "Минивэн",
                    PassengerCapacity = 7,
                    IsAvailable = true
                }
            };
            context.Cars.AddRange(cars);
            context.SaveChanges();

            // --- Транспортные заявки (TransportRequest) ---
            var transportRequests = new TransportRequest[]
            {
                // оригинальные 6 заявок
                new TransportRequest
                {
                    UserId = users[1].Id,
                    TripDateTime = DateTime.Now.AddDays(1).AddHours(9),
                    StartPoint = "ул. Дзержинского, 25",
                    EndPoint = "Автовокзал Торжок",
                    PassengerCount = 2,
                    Purpose = "Встреча партнёров",
                    Status = "Подтверждена",
                    CarId = cars[0].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new TransportRequest
                {
                    UserId = users[1].Id,
                    TripDateTime = DateTime.Now.AddDays(2).AddHours(14),
                    StartPoint = "Администрация г. Торжок",
                    EndPoint = "Ж/д вокзал",
                    PassengerCount = 1,
                    Purpose = "Отправка документов",
                    Status = "На согласовании",
                    CarId = null,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new TransportRequest
                {
                    UserId = users[0].Id,
                    TripDateTime = DateTime.Now.AddDays(3).AddHours(8),
                    StartPoint = "ИТ-отдел, ул. Луначарского",
                    EndPoint = "Центральный склад на ул. Степана Разина",
                    PassengerCount = 2,
                    Purpose = "Инвентаризация оборудования",
                    Status = "Подтверждена",
                    CarId = cars[3].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new TransportRequest
                {
                    UserId = users[2].Id,
                    TripDateTime = DateTime.Now.AddDays(4).AddHours(11),
                    StartPoint = "Калининское шоссе, 51 (ЛПУМГ)",
                    EndPoint = "Ж/д вокзал",
                    PassengerCount = 2,
                    Purpose = "Служебная командировка",
                    Status = "На согласовании",
                    CarId = cars[4].Id,
                    CreatedAt = DateTime.UtcNow
                },
                new TransportRequest
                {
                    UserId = users[1].Id,
                    TripDateTime = DateTime.Now.AddDays(5).AddHours(7),
                    StartPoint = "ул. Дзержинского, 25",
                    EndPoint = "Калининское шоссе, 51 (ЛПУМГ)",
                    PassengerCount = 1,
                    Purpose = "Участие в совещании",
                    Status = "Подтверждена",
                    CarId = cars[0].Id,
                    CreatedAt = DateTime.UtcNow.AddHours(-5)
                },
                new TransportRequest
                {
                    UserId = users[0].Id,
                    TripDateTime = DateTime.Now.AddDays(-2).AddHours(15),
                    StartPoint = "ЛПУМГ, Калининское шоссе, 51",
                    EndPoint = "Автостанция",
                    PassengerCount = 3,
                    Purpose = "Встреча гостей",
                    Status = "Выполнена",
                    CarId = cars[1].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-4)
                },
                // новые транспортные заявки
                new TransportRequest
                {
                    UserId = users[4].Id, // Смирнова
                    TripDateTime = DateTime.Now.AddDays(6).AddHours(10),
                    StartPoint = "Бухгалтерия, ул. Мира, 10",
                    EndPoint = "Налоговая инспекция",
                    PassengerCount = 1,
                    Purpose = "Сдача отчётности",
                    Status = "На согласовании",
                    CarId = null,
                    CreatedAt = DateTime.UtcNow
                },
                new TransportRequest
                {
                    UserId = users[5].Id, // Егоров
                    TripDateTime = DateTime.Now.AddDays(3).AddHours(9),
                    StartPoint = "Диспетчерская, Калининское шоссе, 51",
                    EndPoint = "Газпром газораспределение, ул. Героя России Василия Клещенко, 8А",
                    PassengerCount = 3,
                    Purpose = "Техническое совещание",
                    Status = "Подтверждена",
                    CarId = cars[5].Id, // Volkswagen Polo
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new TransportRequest
                {
                    UserId = users[6].Id, // Зуева
                    TripDateTime = DateTime.Now.AddDays(7).AddHours(14),
                    StartPoint = "ИТ-отдел, ул. Луначарского",
                    EndPoint = "Серверная, ул. Строителей, 5",
                    PassengerCount = 1,
                    Purpose = "Настройка оборудования",
                    Status = "На согласовании",
                    CarId = cars[2].Id, // Ford Transit
                    CreatedAt = DateTime.UtcNow.AddHours(-2)
                },
                new TransportRequest
                {
                    UserId = users[3].Id, // Кузнецов (Chief)
                    TripDateTime = DateTime.Now.AddDays(2).AddHours(8),
                    StartPoint = "Транспортный отдел",
                    EndPoint = "Автовокзал Торжок",
                    PassengerCount = 2,
                    Purpose = "Получение запчастей",
                    Status = "Выполнена",
                    CarId = cars[6].Id, // Ford Transit Connect
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new TransportRequest
                {
                    UserId = users[2].Id, // Петров
                    TripDateTime = DateTime.Now.AddDays(8).AddHours(16),
                    StartPoint = "Складское хозяйство, ул. Складская, 3",
                    EndPoint = "Магазин «Стройматериалы»",
                    PassengerCount = 2,
                    Purpose = "Закупка инвентаря",
                    Status = "На согласовании",
                    CarId = null,
                    CreatedAt = DateTime.UtcNow
                },
                new TransportRequest
                {
                    UserId = users[5].Id, // Егоров
                    TripDateTime = DateTime.Now.AddDays(-1).AddHours(11),
                    StartPoint = "Диспетчерская",
                    EndPoint = "ЛПУМГ, компрессорная станция",
                    PassengerCount = 1,
                    Purpose = "Проверка оборудования",
                    Status = "Отклонена",
                    CarId = cars[0].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                }
            };
            context.TransportRequests.AddRange(transportRequests);
            context.SaveChanges();

            // --- Маршруты трансфера (дополнительные) ---
            var route1 = new TransferRoute
            {
                Name = "Ильинская площадь → Газпром газораспределение (утренний)",
                Description = "Ежедневный трансфер для сотрудников к 8:00",
                CarId = cars[1].Id, // Минивэн
                IsActive = true
            };
            context.TransferRoutes.Add(route1);
            context.SaveChanges();

            var stops1 = new TransferStop[]
            {
                new TransferStop
                {
                    TransferRouteId = route1.Id,
                    Order = 1,
                    Address = "Ильинская площадь",
                    ArrivalTime = new TimeSpan(7, 30, 0)
                },
                new TransferStop
                {
                    TransferRouteId = route1.Id,
                    Order = 2,
                    Address = "Газпром газораспределение, ул. Героя России Василия Клещенко, 8А",
                    ArrivalTime = new TimeSpan(7, 50, 0)
                }
            };
            context.TransferStops.AddRange(stops1);

            var route2 = new TransferRoute
            {
                Name = "Газпром газораспределение → Ильинская площадь (вечерний)",
                Description = "Вечерний развоз сотрудников с Газпром газораспределение",
                CarId = cars[1].Id, // Минивэн
                IsActive = true
            };
            context.TransferRoutes.Add(route2);
            context.SaveChanges();

            var stops2 = new TransferStop[]
            {
                new TransferStop
                {
                    TransferRouteId = route2.Id,
                    Order = 1,
                    Address = "Газпром газораспределение, ул. Героя России Василия Клещенко, 8А",
                    ArrivalTime = new TimeSpan(17, 30, 0)
                },
                new TransferStop
                {
                    TransferRouteId = route2.Id,
                    Order = 2,
                    Address = "Ильинская площадь",
                    ArrivalTime = new TimeSpan(17, 50, 0)
                }
            };
            context.TransferStops.AddRange(stops2);

            var route3 = new TransferRoute
            {
                Name = "Площадь Пушкина → ЛПУМГ (утренний)",
                Description = "Ежедневный трансфер для сотрудников к 8:00",
                CarId = cars[4].Id, // Mercedes Sprinter (автобус)
                IsActive = true
            };
            context.TransferRoutes.Add(route3);
            context.SaveChanges();

            var stops3 = new TransferStop[]
            {
                new TransferStop
                {
                    TransferRouteId = route3.Id,
                    Order = 1,
                    Address = "Площадь Пушкина",
                    ArrivalTime = new TimeSpan(7, 30, 0)
                },
                new TransferStop
                {
                    TransferRouteId = route3.Id,
                    Order = 2,
                    Address = "ЛПУМГ, Калининское шоссе, 51",
                    ArrivalTime = new TimeSpan(7, 45, 0)
                }
            };
            context.TransferStops.AddRange(stops3);

            var route4 = new TransferRoute
            {
                Name = "ЛПУМГ → Площадь Пушкина (вечерний)",
                Description = "Вечерний развоз сотрудников с ЛПУМГ",
                CarId = cars[4].Id, // Mercedes Sprinter (автобус)
                IsActive = true
            };
            context.TransferRoutes.Add(route4);
            context.SaveChanges();

            var stops4 = new TransferStop[]
            {
                new TransferStop
                {
                    TransferRouteId = route4.Id,
                    Order = 1,
                    Address = "ЛПУМГ, Калининское шоссе, 51",
                    ArrivalTime = new TimeSpan(17, 30, 0)
                },
                new TransferStop
                {
                    TransferRouteId = route4.Id,
                    Order = 2,
                    Address = "Площадь Пушкина",
                    ArrivalTime = new TimeSpan(17, 45, 0)
                }
            };
            context.TransferStops.AddRange(stops4);

            // Дополнительный маршрут
            var route5 = new TransferRoute
            {
                Name = "Ж/д вокзал → Автовокзал → ЛПУМГ (утренний)",
                Description = "Трансфер для приезжающих сотрудников",
                CarId = cars[6].Id, // Ford Transit Connect
                IsActive = true
            };
            context.TransferRoutes.Add(route5);
            context.SaveChanges();

            var stops5 = new TransferStop[]
            {
                new TransferStop
                {
                    TransferRouteId = route5.Id,
                    Order = 1,
                    Address = "Ж/д вокзал, ул. Вокзальная, 1",
                    ArrivalTime = new TimeSpan(7, 15, 0)
                },
                new TransferStop
                {
                    TransferRouteId = route5.Id,
                    Order = 2,
                    Address = "Автовокзал Торжок",
                    ArrivalTime = new TimeSpan(7, 25, 0)
                },
                new TransferStop
                {
                    TransferRouteId = route5.Id,
                    Order = 3,
                    Address = "ЛПУМГ, Калининское шоссе, 51",
                    ArrivalTime = new TimeSpan(7, 50, 0)
                }
            };
            context.TransferStops.AddRange(stops5);
            context.SaveChanges();

            // --- Помещения ---
            var rooms = new Room[]
            {
                new Room
                {
                    Name = "Переговорная 1",
                    Location = "2 этаж, корпус А",
                    Capacity = 8,
                    Equipment = "Проектор, флипчарт",
                    IsActive = true
                },
                new Room
                {
                    Name = "Конференц-зал",
                    Location = "1 этаж, корпус Б",
                    Capacity = 30,
                    Equipment = "Проектор, ВКС",
                    IsActive = true
                },
                new Room
                {
                    Name = "Малый зал",
                    Location = "3 этаж",
                    Capacity = 4,
                    Equipment = "Телевизор",
                    IsActive = true
                },
                // Дополнительное помещение
                new Room
                {
                    Name = "Кабинет 404",
                    Location = "4 этаж, корпус А",
                    Capacity = 2,
                    Equipment = "Доска",
                    IsActive = true
                }
            };
            context.Rooms.AddRange(rooms);
            context.SaveChanges();

            // --- Бронирования помещений (RoomRequest) ---
            var roomRequests = new RoomRequest[]
            {
                new RoomRequest
                {
                    UserId = users[1].Id,
                    RoomId = rooms[0].Id,
                    StartTime = DateTime.Today.AddDays(1).AddHours(10),
                    EndTime = DateTime.Today.AddDays(1).AddHours(11),
                    ParticipantsCount = 4,
                    Topic = "Планирование проекта",
                    Description = "Нужен флипчарт",
                    Status = "На согласовании",
                    CreatedAt = DateTime.UtcNow
                },
                new RoomRequest
                {
                    UserId = users[2].Id,
                    RoomId = rooms[1].Id,
                    StartTime = DateTime.Today.AddDays(2).AddHours(14),
                    EndTime = DateTime.Today.AddDays(2).AddHours(16),
                    ParticipantsCount = 20,
                    Topic = "Общее собрание отдела",
                    Description = null,
                    Status = "Подтверждена",
                    ApprovedAt = DateTime.UtcNow,
                    ApproverId = users[0].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new RoomRequest
                {
                    UserId = users[4].Id,
                    RoomId = rooms[2].Id,
                    StartTime = DateTime.Today.AddDays(3).AddHours(9),
                    EndTime = DateTime.Today.AddDays(3).AddHours(10),
                    ParticipantsCount = 3,
                    Topic = "Созвон с партнёрами",
                    Description = "Тихое место",
                    Status = "Подтверждена",
                    ApprovedAt = DateTime.UtcNow,
                    ApproverId = users[0].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new RoomRequest
                {
                    UserId = users[5].Id,
                    RoomId = rooms[3].Id,
                    StartTime = DateTime.Today.AddDays(4).AddHours(11),
                    EndTime = DateTime.Today.AddDays(4).AddHours(12),
                    ParticipantsCount = 2,
                    Topic = "Собеседование",
                    Description = null,
                    Status = "На согласовании",
                    CreatedAt = DateTime.UtcNow
                },
                new RoomRequest
                {
                    UserId = users[6].Id,
                    RoomId = rooms[0].Id,
                    StartTime = DateTime.Today.AddDays(1).AddHours(15),
                    EndTime = DateTime.Today.AddDays(1).AddHours(16),
                    ParticipantsCount = 5,
                    Topic = "Тренинг по безопасности",
                    Description = "Проектор и образцы документов",
                    Status = "На согласовании",
                    CreatedAt = DateTime.UtcNow.AddHours(-1)
                },
                new RoomRequest
                {
                    UserId = users[3].Id,
                    RoomId = rooms[1].Id,
                    StartTime = DateTime.Today.AddDays(5).AddHours(13),
                    EndTime = DateTime.Today.AddDays(5).AddHours(15),
                    ParticipantsCount = 15,
                    Topic = "Совещание руководителей",
                    Description = "Срочно",
                    Status = "Подтверждена",
                    ApprovedAt = DateTime.UtcNow,
                    ApproverId = users[0].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                },
                new RoomRequest
                {
                    UserId = users[2].Id,
                    RoomId = rooms[2].Id,
                    StartTime = DateTime.Today.AddDays(3).AddHours(16),
                    EndTime = DateTime.Today.AddDays(3).AddHours(17),
                    ParticipantsCount = 2,
                    Topic = "Обсуждение закупок",
                    Description = "Без оборудования",
                    Status = "Отклонена",
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new RoomRequest
                {
                    UserId = users[4].Id,
                    RoomId = rooms[3].Id,
                    StartTime = DateTime.Today.AddDays(2).AddHours(10),
                    EndTime = DateTime.Today.AddDays(2).AddHours(11),
                    ParticipantsCount = 2,
                    Topic = "Встреча с соискателем",
                    Description = null,
                    Status = "Выполнена",
                    ApprovedAt = DateTime.UtcNow.AddDays(-5),
                    ApproverId = users[0].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-6)
                }
            };
            context.RoomRequests.AddRange(roomRequests);
            context.SaveChanges();
        }
    }
}