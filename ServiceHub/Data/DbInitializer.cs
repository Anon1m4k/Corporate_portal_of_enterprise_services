using ServiceHub.Models.Account;
using ServiceHub.Models.Transport;

namespace ServiceHub.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Если база уже содержит данные, ничего не делаем
            if (context.Users.Any() || context.Cars.Any())
                return;

            // --- Пользователи ---
            var users = new AuthUser[]
            {
                new AuthUser
                {
                    Email = "admin@servicehub.local",
                    Password = "Admin123!",
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
                    Password = "User123!",
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
                    Password = "User123!",
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
                    Password = "User123!",
                    FirstName = "Сергей",
                    LastName = "Кузнецов",
                    Department = "Транспортный отдел",
                    Role = "Chief",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };
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
                }
            };
            context.Cars.AddRange(cars);
            context.SaveChanges();

            // --- Транспортные заявки (TransportRequest) ---
            var transportRequests = new TransportRequest[]
            {
                // Существующие заявки
                new TransportRequest
                {
                    UserId = users[1].Id, // Иванов
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
                    UserId = users[0].Id, // Админ
                    TripDateTime = DateTime.Now.AddDays(3).AddHours(8),
                    StartPoint = "ИТ-отдел, ул. Луначарского",
                    EndPoint = "Центральный склад на ул. Степана Разина",
                    PassengerCount = 2,
                    Purpose = "Инвентаризация оборудования",
                    Status = "Подтверждена",
                    CarId = cars[3].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                // Новые заявки с участием ЛПУМГ
                new TransportRequest
                {
                    UserId = users[2].Id, // Петров
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
                    UserId = users[1].Id, // Иванов
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
                    UserId = users[0].Id, // Админ
                    TripDateTime = DateTime.Now.AddDays(-2).AddHours(15),
                    StartPoint = "ЛПУМГ, Калининское шоссе, 51",
                    EndPoint = "Автостанция",
                    PassengerCount = 3,
                    Purpose = "Встреча гостей",
                    Status = "Выполнена",
                    CarId = cars[1].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-4)
                }
            };
            context.TransportRequests.AddRange(transportRequests);
            context.SaveChanges();         

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

            context.SaveChanges();
        }
    }
}