using DevDigest.Data.Data;
using DevDigest.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<RssFeedService>();
builder.Services.AddScoped<AiSummaryService>();
builder.Services.AddHttpClient<ArticleContentService>();
builder.Services.AddHttpClient<DailyDigestService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();

app.Run();