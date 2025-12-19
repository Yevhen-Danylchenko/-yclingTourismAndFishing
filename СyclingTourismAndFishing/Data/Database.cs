using CyclingTourismAndFishing.Models;

namespace CyclingTourismAndFishing.Data
{
    public static class Database
    {
        public static void Initialize(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();
                if (!db.Users.Any())
                {
                    db.Users.Add(new User
                    {
                        Username = "admin",
                        Email = "admin@gmail.com",
                        PasswordHash = "admin123",
                        IsAdmin = true
                    });
                    db.SaveChanges();
                }
                if (!db.Categories.Any())
                {
                    db.Categories.AddRange(
                        new Category
                        {
                            Name = "Велосипед МТБ",
                            Description = "Надійний гірський велосипед для тривалих подорожей",
                            ImageUrl = "/images/1.jpg"
                        },
                        new Category
                        {
                            Name = "Велосумка",
                            Description = "Велосумки дозволяють перевозити додатковий багаж",
                            ImageUrl = "/images/2.webp"
                        },
                        new Category
                        {
                            Name = "Палатка для кемпінгу",
                            Description = "Затишна палатка для кемпінгу для відпочинку на природі",
                            ImageUrl = "/images/3.jpg"
                        },
                        new Category
                        {
                            Name = "Риболовні снасті",
                            Description = "Для поціновувачів риболовлі та відпочинку біля річки та водойм",
                            ImageUrl = "/images/4.jpg"
                        }
                    );
                    db.SaveChanges();
                }
                if (!db.Items.Any())
                {
                    db.Items.AddRange(
                        new Item
                        {
                            Name = "Велосипед МТБ",
                            Description = "Підсилена рама та колеса для бездоріжжя. Дискові тормоза. ",
                            Price = 9273,
                            ImageUrl = "/images/1.jpg",
                            CategoryId = 1,
                            IsAvailable = true
                        },
                        new Item
                        {
                            Name = "Велосумка",
                            Description = "Велосумка на велосипед для перевезення речей об'єм 50 л.",
                            Price = 1397,
                            ImageUrl = "/images/2.webp",
                            CategoryId = 2,
                            IsAvailable = true
                        },
                        new Item
                        {
                            Name = "Палатка",
                            Description = "Затишна палатка для кемпінгу для відпочинку на природі",
                            Price = 2899,
                            ImageUrl = "/images/3.jpg",
                            CategoryId = 3,
                            IsAvailable = true
                        },
                        new Item
                        {
                            Name = "Риболовні снасті",
                            Description = "Спінінг з котушкою та кормаком для дальнього забросу з берега",
                            Price = 1217,
                            ImageUrl = "/images/4.jpg",
                            CategoryId = 4,
                            IsAvailable = true
                        }
                    );
                    db.SaveChanges();
                }
            }
        }
    }
}
