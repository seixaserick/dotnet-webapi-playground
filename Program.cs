var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
            {
                // this is necessary when you want Swagger read the  XmlComments (of functions and parameters) to auto-generate Swagger UI help and descriptions. Very useful.
                var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

//builder.Services.AddDistributedMemoryCache();
builder.Services.AddCors();
builder.Services.AddResponseCompression(opt => { opt.EnableForHttps = true; });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseStaticFiles(); 
app.UseDefaultFiles();
app.UseResponseCompression();
app.UseCors(options => { options.AllowAnyOrigin(); options.AllowAnyHeader(); options.AllowAnyMethod(); });
app.UseAuthorization();
app.MapControllers();
app.Run();
