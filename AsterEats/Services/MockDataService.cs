using AsterEats.Models;

namespace AsterEats.Services;

/// <summary>
/// Mock backend for the AsterEats rider prototype.
/// This will later be replaced by the ASP.NET Core API.
/// </summary>
public class MockDataService
{
    private readonly Rider _currentRider;
    private readonly List<Earning> _earnings;
    private readonly List<DeliveryOrder> _history;
    private readonly List<Notification> _notifications;

    private DeliveryOrder? _currentDelivery;

    public MockDataService()
    {
        _currentRider = new Rider
        {
            Id = 1,
            Name = "MD. Samirul Islam",
            Phone = "017XXXXXXXX",
            Email = "samiru123@gmail.com",
            VehicleType = "Motorcycle",
            VehicleNumber = "DHAKA-METRO-1234",
            Rating = 4.8,
            CompletedDeliveries = 152,
            IsOnline = false
        };

        var today = DateTime.UtcNow.Date;

        _earnings = new List<Earning>
        {
            new() { Id = 1, DeliveryOrderId = 101, RestaurantName = "Aster Kitchen", AreaLabel = "Dhanmondi", Amount = 80, DateUtc = today.AddHours(9) },
            new() { Id = 2, DeliveryOrderId = 102, RestaurantName = "Spice Route", AreaLabel = "Mohammadpur", Amount = 110, DateUtc = today.AddHours(11) },
            new() { Id = 3, DeliveryOrderId = 103, RestaurantName = "Green Bowl", AreaLabel = "Dhanmondi", Amount = 95, DateUtc = today.AddHours(13) },
            new() { Id = 4, DeliveryOrderId = 104, RestaurantName = "Curry House", AreaLabel = "Jigatola", Amount = 120, DateUtc = today.AddHours(14) },
            new() { Id = 5, DeliveryOrderId = 105, RestaurantName = "Aster Kitchen", AreaLabel = "Dhanmondi", Amount = 85, DateUtc = today.AddHours(16) },
            new() { Id = 6, DeliveryOrderId = 106, RestaurantName = "Noodle Bar", AreaLabel = "Elephant Rd", Amount = 100, DateUtc = today.AddHours(18) },
            new() { Id = 7, DeliveryOrderId = 107, RestaurantName = "Spice Route", AreaLabel = "Mohammadpur", Amount = 130, DateUtc = today.AddHours(19) },
            new() { Id = 8, DeliveryOrderId = 108, RestaurantName = "Green Bowl", AreaLabel = "Dhanmondi", Amount = 130, DateUtc = today.AddHours(20) },

            new() { Id = 9, DeliveryOrderId = 109, RestaurantName = "Curry House", AreaLabel = "Jigatola", Amount = 640, DateUtc = today.AddDays(-1) },
            new() { Id = 10, DeliveryOrderId = 110, RestaurantName = "Noodle Bar", AreaLabel = "Elephant Rd", Amount = 590, DateUtc = today.AddDays(-2) },
            new() { Id = 11, DeliveryOrderId = 111, RestaurantName = "Aster Kitchen", AreaLabel = "Dhanmondi", Amount = 705, DateUtc = today.AddDays(-3) }
        };

        _history = new List<DeliveryOrder>
        {
            CreateHistoryOrder(
                151,
                "Aster Kitchen",
                "House 21, Road 7, Dhanmondi, Dhaka",
                "Karim Hasan",
                "House 14, Road 5, Dhanmondi, Dhaka",
                80,
                today.AddHours(-2)),

            CreateHistoryOrder(
                150,
                "Spice Route",
                "Road 4, Mohammadpur, Dhaka",
                "Nusrat Jahan",
                "House 8, Road 12, Mohammadpur, Dhaka",
                110,
                today.AddHours(-4)),

            CreateHistoryOrder(
                149,
                "Green Bowl",
                "Satmasjid Road, Dhanmondi, Dhaka",
                "Tanvir Ahmed",
                "House 5, Road 9, Dhanmondi, Dhaka",
                95,
                today.AddDays(-1).AddHours(4))
        };

        _notifications = new List<Notification>
        {
            new()
            {
                Id = 1,
                Title = "Welcome to AsterEats",
                Message = "Your rider account is ready. You can now start accepting deliveries.",
                DateUtc = today.AddHours(-1),
                IsRead = false
            },

            new()
            {
                Id = 2,
                Title = "Earnings Updated",
                Message = "Your recent delivery earnings have been added to your account.",
                DateUtc = today.AddHours(-3),
                IsRead = false
            },

            new()
            {
                Id = 3,
                Title = "Profile Verified",
                Message = "Your rider profile and vehicle information have been verified.",
                DateUtc = today.AddDays(-1),
                IsRead = true
            }
        };
    }

    private DeliveryOrder CreateHistoryOrder(
        int id,
        string restaurantName,
        string restaurantAddress,
        string customerName,
        string deliveryAddress,
        decimal deliveryFee,
        DateTime completedAt)
    {
        return new DeliveryOrder
        {
            Id = id,

            Restaurant = new Restaurant
            {
                Id = id,
                Name = restaurantName,
                Address = restaurantAddress,
                Phone = "018XXXXXXXX"
            },

            CustomerName = customerName,
            CustomerPhone = "019XXXXXXXX",
            DeliveryAddress = deliveryAddress,

            Items = new List<OrderItem>(),

            OrderAmount = 500,
            DeliveryFee = deliveryFee,
            PaymentMethod = "Cash",
            DistanceKm = 2.5,
            Status = DeliveryStatus.Delivered,
            CompletedAtUtc = completedAt
        };
    }

    public Rider GetCurrentRider()
    {
        return _currentRider;
    }

    public List<Earning> GetEarnings()
    {
        return _earnings
            .OrderByDescending(e => e.DateUtc)
            .ToList();
    }

    public List<Earning> GetTodaysEarnings()
    {
        var today = DateTime.UtcNow.Date;

        return _earnings
            .Where(e => e.DateUtc.Date == today)
            .ToList();
    }

    public List<DeliveryOrder> GetDeliveryHistory()
    {
        return _history
            .OrderByDescending(o => o.CompletedAtUtc)
            .ToList();
    }

    public List<Notification> GetNotifications()
    {
        return _notifications
            .OrderByDescending(n => n.DateUtc)
            .ToList();
    }

    public int GetUnreadNotificationCount()
    {
        return _notifications.Count(n => !n.IsRead);
    }

    public void MarkAllNotificationsAsRead()
    {
        foreach (var notification in _notifications)
        {
            notification.IsRead = true;
        }
    }

    public DeliveryOrder? GetCurrentDelivery()
    {
        return _currentDelivery;
    }

    public DeliveryOrder CreateDeliveryRequest()
    {
        var order = new DeliveryOrder
        {
            Id = 201,

            Restaurant = new Restaurant
            {
                Id = 1,
                Name = "Aster Kitchen",
                Address = "House 27, Road 8, Dhanmondi, Dhaka",
                Phone = "018XXXXXXXX"
            },

            CustomerName = "Rahim Ahmed",
            CustomerPhone = "019XXXXXXXX",
            DeliveryAddress = "House 14, Road 5, Dhanmondi, Dhaka",

            Items = new List<OrderItem>
            {
                new()
                {
                    Id = 1,
                    Name = "Chicken Burger",
                    Quantity = 2,
                    Price = 220
                },

                new()
                {
                    Id = 2,
                    Name = "French Fries",
                    Quantity = 1,
                    Price = 120
                },

                new()
                {
                    Id = 3,
                    Name = "Coke",
                    Quantity = 1,
                    Price = 90
                }
            },

            OrderAmount = 650,
            DeliveryFee = 80,
            PaymentMethod = "Cash",
            DistanceKm = 2.4,
            Status = DeliveryStatus.Requested
        };

        _currentDelivery = order;

        return order;
    }

    public void SetCurrentDelivery(DeliveryOrder? order)
    {
        _currentDelivery = order;
    }

    public void CompleteDelivery(DeliveryOrder order)
    {
        order.Status = DeliveryStatus.Delivered;
        order.CompletedAtUtc = DateTime.UtcNow;

        _history.Add(order);

        var earning = new Earning
        {
            Id = _earnings.Count + 1,
            DeliveryOrderId = order.Id,
            RestaurantName = order.Restaurant.Name,
            AreaLabel = "Dhanmondi",
            Amount = order.DeliveryFee,
            DateUtc = DateTime.UtcNow
        };

        _earnings.Add(earning);

        _currentDelivery = null;

        _currentRider.CompletedDeliveries++;
    }
}