
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NanoHealthLoggingSystem.Configrations;
using NanoHealthLoggingSystem.Context;
using NanoHealthLoggingSystem.Entities;
using NanoHealthLoggingSystem.IRepositories;
using NanoHealthLoggingSystem.Reposatories;
using NanoHealthLoggingSystem.Repositories;
using NanoHealthLoggingSystem.Statics;


namespace NanoHealthLoggingSystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //builder.Services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("UserOrAdmin", policy =>
            //        policy.RequireRole("User", "Admin"));
            //});
    //        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //.AddJwtBearer(options =>
    //{
    //    options.TokenValidationParameters = new TokenValidationParameters
    //    {
    //        ValidateIssuer = true,
    //        ValidateAudience = true,
    //        ValidateLifetime = true,
    //        ValidateIssuerSigningKey = true,
    //        ValidIssuer = builder.Configuration["Jwt:Issuer"],
    //        ValidAudience = builder.Configuration["Jwt:Audience"],
    //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    //    };
    //});
    //        builder.Services.AddAuthorization();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(
                 opt =>
                 {
                     #region For AutherizationSwagger
                     opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                     {
                         In = ParameterLocation.Header,
                         Description = "Please enter token",
                         Name = "Authorization",
                         Type = SecuritySchemeType.Http,
                         BearerFormat = "JWT",
                         Scheme = "bearer"
                     });
                     opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                                     {
                                        {
                                            new OpenApiSecurityScheme
                                            {
                                                Reference = new OpenApiReference
                                                {
                                                    Type=ReferenceType.SecurityScheme,
                                                    Id="Bearer"
                                                }
                                            },
                                            new string[]{}
                                        }
                                     }
                     );
                 }
                 #endregion
            );

            var con = builder.Configuration.GetConnectionString("sama");
            builder.Services.AddDbContext<LoggingContext>(options => options.UseSqlServer(con));
            builder.Services.AddIdentity<User, IdentityRole>()
          .AddEntityFrameworkStores<LoggingContext>()
          .AddDefaultTokenProviders();
            builder.Services.Configure<JWTConfiguration>(builder.Configuration.GetSection("Jwt"));
            builder.Services.AddScoped<IUserReposatory, UserReposatory>();
            builder.Services.AddScoped<ILoggingRepository, LoggingRepository>();
            builder.Services.AddScoped<IStorageRepository, StorageRepository>();

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                await RolesIdentity.EnsureRolesAsync(serviceProvider);
            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
