using CyclingTourismAndFishing.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CyclingTourismAndFishing.Data;
using CyclingTourismAndFishing.Models;

namespace CyclingTourismAndFishing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
            });
            builder.Services.AddAuthorization();

            var app = builder.Build();

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            
            Database.Initialize(app);

            app.MapGet("/", async (ApplicationDbContext db) =>
            {
                var html = await File.ReadAllTextAsync("Pages/index.html");
                return Results.Content(html, "text/html");
            });

            app.MapGet("/api/categories", async (ApplicationDbContext db) =>
            {
                var categories = await db.Categories.ToListAsync();
                return Results.Json(categories);
            });

            app.MapGet("/api/menu-items", async (ApplicationDbContext db) =>
            {
                var menuItems = await db.Items.Include(m => m.Category).Where(m => m.IsAvailable).ToListAsync();
                return Results.Json(menuItems);
            });

            app.MapGet("/login", async () =>
            {
                var html = await File.ReadAllTextAsync("Pages/login.html");
                return Results.Content(html, "text/html");
            });

            app.MapPost("/login", async (HttpContext context, ApplicationDbContext db) =>
            {
                var form = await context.Request.ReadFormAsync();
                var username = form["username"].ToString();
                var email = form["email"].ToString();
                var password = form["password"].ToString();

                var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);

                if (user != null && user.PasswordHash == password && user.IsAdmin)
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, username),
                        new Claim(ClaimTypes.Role, "Admin")
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return Results.Redirect("/admin");
                }

                return Results.Redirect("/login");
            });

            app.MapGet("/logout", async (HttpContext context) =>
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Results.Redirect("/");
            });

            app.MapGet("/admin", async (HttpContext context) =>
            {
                if (!context.User.Identity?.IsAuthenticated ?? true)
                    return Results.Redirect("/login");

                var html = await File.ReadAllTextAsync("Pages/admin.html");
                return Results.Content(html, "text/html");
            });

            app.MapGet("/api/admin/menu-items", async (ApplicationDbContext db) =>
            {
                var menuItems = await db.Items.Include(m => m.Category).ToListAsync();
                return Results.Json(menuItems);
            });

            app.MapGet("/admin/add-item", async (HttpContext context) =>
            {
                if (!context.User.Identity?.IsAuthenticated ?? true)
                    return Results.Redirect("/login");
                var html = await File.ReadAllTextAsync("Pages/add-item.html");
                return Results.Content(html, "text/html");
            }).RequireAuthorization();

            app.MapPost("/admin/add-item", async (HttpContext context, ApplicationDbContext db) =>
            {
                if (!context.User.Identity?.IsAuthenticated ?? true)
                    return Results.Redirect("/login");

                var form = await context.Request.ReadFormAsync();

                var menuItem = new Item
                {
                    Name = form["name"],
                    Description = form["description"],
                    Price = decimal.Parse(form["price"]),
                    ImageUrl = form["imageUrl"],
                    CategoryId = int.Parse(form["categoryId"]),
                    IsAvailable = true
                };

                db.Items.Add(menuItem);

                return Results.Redirect("/admin");
            }).RequireAuthorization();

            app.MapGet("/admin/edit-item/{id}", async (int id, HttpContext context) =>
            {
                if (!context.User.Identity?.IsAuthenticated ?? true)
                    return Results.Redirect("/login");

                var html = await File.ReadAllTextAsync("Pages/edit-item.html");
                return Results.Content(html, "text/html");
            }).RequireAuthorization();

            app.MapGet("/api/menu-items/{id}", async (int id, ApplicationDbContext db) =>
            {
                var menuItem = await db.Items.FindAsync(id);
                return Results.Json(menuItem);
            });

            app.MapPost("/admin/edit-item/{id}", async (int id, HttpContext context, ApplicationDbContext db) =>
            {
                if (!context.User.Identity?.IsAuthenticated ?? true)
                    return Results.Redirect("/login");

                var item = await db.Items.FindAsync(id);
                if (item == null)
                    return Results.Redirect("/admin");

                var form = await context.Request.ReadFormAsync();

                item.Name = form["name"];
                item.Description = form["description"];
                item.Price = decimal.Parse(form["price"]);
                item.ImageUrl = form["imageUrl"];
                item.CategoryId = int.Parse(form["categoryId"]);
                await db.SaveChangesAsync();
                return Results.Redirect("/admin");
            }).RequireAuthorization();

            app.MapPost("/admin/delete-item{id}", async (int id, HttpContext context, ApplicationDbContext db) =>
            {
                if (!context.User.Identity?.IsAuthenticated ?? true)
                    return Results.Redirect("/login");

                var item = await db.Items.FindAsync(id);
                if (item != null)
                {
                    db.Items.Remove(item);
                    await db.SaveChangesAsync();
                }
                return Results.Redirect("/admin");

            }).RequireAuthorization();

            app.MapGet("/admin/add-category", async (HttpContext context) =>

            {

                if (!context.User.Identity?.IsAuthenticated ?? true)

                    return Results.Redirect("/login");

                var html = await File.ReadAllTextAsync("Pages/add-category.html");

                return Results.Content(html, "text/html");

            }).RequireAuthorization();

            app.MapPost("/admin/add-category", async (HttpContext context, ApplicationDbContext db) =>
            {
                if (!context.User.Identity?.IsAuthenticated ?? true)
                    return Results.Redirect("/login");

                var form = await context.Request.ReadFormAsync();

                var category = new Category
                {
                    Name = form["name"],
                    Description = form["description"],
                    ImageUrl = form["imageUrl"]
                };

                db.Categories.Add(category);
                await db.SaveChangesAsync();

                return Results.Redirect("/admin");
            }).RequireAuthorization();


            app.MapGet("/userlogin", async () =>
            {
                var html = await File.ReadAllTextAsync("Pages/userlogin.html");
                return Results.Content(html, "text/html");
            });

            // GET: /api/user/items
            app.MapGet("/api/userlogin/items", async (ApplicationDbContext db) =>
            {
                // Вибираємо тільки доступні товари
                var items = await db.Items
                    .Where(i => i.IsAvailable)
                    .Select(i => new
                    {
                        i.Id,
                        i.Name,
                        i.Description,
                        i.Price,
                        i.ImageUrl,
                        CategoryName = i.Category.Name
                    })
                    .ToListAsync();

                return Results.Ok(items);
            });

            app.MapPost("/api/cart/add", async (CartItemDto dto, ApplicationDbContext db) =>
            {
                var cartItem = new CartItem
                {
                    ItemId = dto.ItemId,
                    Quantity = dto.Quantity,
                    UserId = "test-user" // пізніше заміниш на реального користувача
                };

                db.CartItems.Add(cartItem);
                await db.SaveChangesAsync();

                return Results.Ok(cartItem);
            });

            app.Run();
        }
    }
}
