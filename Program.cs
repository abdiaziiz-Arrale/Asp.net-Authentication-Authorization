using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TodoApp.Data;
using TodoApp.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JwtConfig>(
    builder.Configuration.GetSection("JwtConfig")
);
builder.Services.AddAuthentication(option=>{
 option.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;   
 option.DefaultScheme=JwtBearerDefaults.AuthenticationScheme;   
 option.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;   
}).AddJwtBearer(config=>{
config.RequireHttpsMetadata = false;

		var keyInput = "random_text_with_at_least_32_chars";

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyInput));

		config.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidIssuer = "MyAPI",
			ValidateAudience = true,
			ValidAudience = "MyFrontendApp",
			ValidateLifetime = true,
            RequireExpirationTime=false,
			IssuerSigningKey = key
		};
});
builder.Services.AddIdentity<IdentityUser,IdentityRole>(option => option.SignIn.RequireConfirmedAccount=true)
.AddEntityFrameworkStores<ApiDbContext>();

builder.Services.AddDbContext<ApiDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                ));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
      app.UseSwagger();
      app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
