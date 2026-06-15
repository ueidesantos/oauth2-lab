using OAuth2Lab.Web.Features.Auth.Login;
using OAuth2Lab.Web.Features.Auth.Logout;
using OAuth2Lab.Web.Infrastructure.Auth;
using OAuth2Lab.Web.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        options.RootDirectory = "/Features";
        options.Conventions.AddPageRoute("/Home/Index", "/");
    });

builder.Services.AddSsoAuthentication(builder.Configuration);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "oauth2lab.session.data";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.IdleTimeout = TimeSpan.FromHours(8);
});

builder.Services.AddMongoDb(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/auth/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapLoginEndpoint();
app.MapLogoutEndpoint();
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();