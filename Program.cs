using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Web.DataAccess;
using ProjectManagement.Web.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddDbContext<ProjectManagementDbContext>(options => 
//    options.UseSqlServer(builder.Configuration.GetConnectionString("FinalExamDb123")));

//builder.Services.AddIdentity<User, IdentityRole>()
//        .AddEntityFrameworkStores<ProjectManagementDbContext>()
//        .AddDefaultTokenProviders();


string connStr = builder.Configuration.GetConnectionString("FinalExamDb123");
builder.Services.AddDbContext<ProjectManagementDbContext>(options => options.UseSqlServer(connStr));


builder.Services.AddIdentity<User, IdentityRole>(options => {
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireDigit = true;
}).AddEntityFrameworkStores<ProjectManagementDbContext>().AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    await ProjectManagementDbContext.CreateAdminUser(scope.ServiceProvider);
}

app.Run();
