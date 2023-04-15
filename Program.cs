using Microsoft.EntityFrameworkCore;
using movie_tracker_website.Data;
using Microsoft.AspNetCore.Identity;
using movie_tracker_website.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using movie_tracker_website.Utilities;
using movie_tracker_website.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddAuthentication();
//services
builder.Services.AddScoped<IImageUpload, ImageUpload>();
builder.Services.AddScoped<IMoviesList, MoviesList>();
//db contexts
builder.Services.AddDbContext<AuthDBContext>(options =>
   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//identity
builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AuthDBContext>();
//identity options
builder.Services.Configure<IdentityOptions>(options => options.User.RequireUniqueEmail = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Landing}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();