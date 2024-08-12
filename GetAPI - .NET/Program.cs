var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient(); // HttpClient'ı ekle

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// /api/users dizinine yönlendirme
app.MapGet("/", (HttpContext context) =>
{
    context.Response.Redirect("/api/users");
    return Task.CompletedTask;
});
app.MapControllers();


app.Run("https://localhost:7060/");
