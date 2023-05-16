using Microsoft.EntityFrameworkCore;
using movie_tracker_website.Data;
using Microsoft.AspNetCore.Identity;
using movie_tracker_website.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using movie_tracker_website.Utilities;
using movie_tracker_website.Services;
using movie_tracker_website.Services.common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddAuthentication();
//sessions
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".MovieTracker.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(180);
    options.Cookie.IsEssential = true;
});
//services
builder.Services.AddScoped<IImageUpload, ImageUpload>();
builder.Services.AddScoped<IMoviesList, MoviesList>();
builder.Services.AddScoped<IMoviePageService, MoviePageService>();
builder.Services.AddScoped<IMovieSessionListService, MovieSessionListService>();
builder.Services.AddScoped<IPersonalMoviesService, PersonalMoviesService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IStatisticService, StatisticService>();
builder.Services.AddScoped<ITagService, TagService>();

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

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Landing}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();