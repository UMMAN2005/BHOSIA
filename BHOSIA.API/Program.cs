using AutoMapper;
using BHOSIA.API.Filters;
using BHOSIA.API.Middlewares;
using BHOSIA.BUSINESS.Services.Implementations;
using BHOSIA.BUSINESS.Validators;
using BHOSIA.CORE.Entities;
using BHOSIA.DATA;
using BHOSIA.DATA.Repositories.Implementations;
using BHOSIA.DATA.Repositories.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Example of bypassing SSL certificate validation (use with caution)
ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

// Add services to the container.
builder.Services.AddControllers().ConfigureApiBehaviorOptions(opt => {
  opt.InvalidModelStateResponseFactory = context => {
    var errors = context.ModelState.Where(x => x.Value!.Errors.Count > 0)
    .Select(x => new RestExceptionError(x.Key, x.Value!.Errors.First().ErrorMessage)).ToList();
    return new BadRequestObjectResult(new { message = "", errors });
  };
});

builder.Services.AddDbContext<AppDbContext>(option => {
  option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<AppUser, IdentityRole<string>>(opt => {
  opt.Password.RequireNonAlphanumeric = false;
  opt.Password.RequiredLength = 8;
  opt.Password.RequireUppercase = false;
  opt.Password.RequireLowercase = false;
  opt.Password.RequireDigit = false;
  opt.User.RequireUniqueEmail = true;
})
  .AddDefaultTokenProviders()
  .AddEntityFrameworkStores<AppDbContext>()
  .AddUserManager<AppUserManager>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton(provider => new MapperConfiguration(cfg => {
  cfg.AddProfile(new MapProfile(provider.GetService<IHttpContextAccessor>()!));
}).CreateMapper());

//Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c => {
  c.SwaggerDoc("user", new OpenApiInfo {
    Title = "User API",
    Version = "v1"
  });

  c.SwaggerDoc("admin", new OpenApiInfo {
    Title = "Admin API",
    Version = "v1"
  });

  c.SwaggerDoc("trait", new OpenApiInfo {
    Title = "Trait API",
    Version = "v1"
  });

  c.SwaggerDoc("auth", new OpenApiInfo {
    Title = "Auth API",
    Version = "v1"
  });

  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
    In = ParameterLocation.Header,
    Description = "Please insert JWT with Bearer into field",
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey
  });

  c.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference
        {
          Type = ReferenceType.SecurityScheme,
          Id = "Bearer"
        }
      },
      Array.Empty<string>()
    }
  });

  // Apply the document filters
  c.DocumentFilter<AdminDocumentFilter>();
  c.DocumentFilter<UserDocumentFilter>();
  c.DocumentFilter<AuthDocumentFilter>();
  c.DocumentFilter<TraitDocumentFilter>();
});



//Fluent Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<UserLoginValidator>();

//Custom Services
builder.Services.AddScoped<AppUserManager>();

builder.Services.AddScoped<IUserAuthService, UserAuthService>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ILocationService, LocationService>();

builder.Services.AddScoped<RedisCacheFilter>();

builder.Services.AddLogging();

builder.Host.UseSerilog((hostingContext, loggerConfiguration) => {
  loggerConfiguration
  .ReadFrom.Configuration(hostingContext.Configuration);
});

// Micro-elements
builder.Services.AddFluentValidationRulesToSwagger();

// Cashing
builder.Services.AddStackExchangeRedisCache(opt => {
  opt.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
  opt.InstanceName = "BHOSIA_";
});

builder.Services.AddResponseCaching();


// Configure JWT and Google Authentication
//builder.Services.AddAuthentication(opt => {
//  opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//  opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//  opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(opt => {
//  opt.TokenValidationParameters = new TokenValidationParameters {
//    ValidAudience = builder.Configuration.GetSection("JWT:Audience").Value,
//    ValidIssuer = builder.Configuration.GetSection("JWT:Issuer").Value,
//    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT:Secret").Value!))
//  };
//})
//.AddCookie(opt => {
//  opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//})
//.AddGoogle(opt => {
//  opt.ClientId = builder.Configuration.GetSection("Google:ClientId").Value!;
//  opt.ClientSecret = builder.Configuration.GetSection("Google:ClientSecret").Value!;
//  opt.SaveTokens = true;
//  opt.Scope.Add("profile");
//  opt.Events.OnCreatingTicket = (context) => {
//    var picture = context.User.GetProperty("picture").GetString();
//    if (picture != null) context.Identity?.AddClaim(new Claim("picture", picture));
//    return Task.CompletedTask;
//  };
//  opt.Events.OnRedirectToAuthorizationEndpoint = context => {
//    context.HttpContext.Response.Redirect(context.RedirectUri);
//    return Task.CompletedTask;
//  };
//});

// Configure CORS
builder.Services.AddCors(opt => {
  opt.AddPolicy("AllowAll", conf => {
    conf.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
  });
});

var app = builder.Build();

await ApplyMigrationsAndSeedData(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/user/swagger.json", "User API V1");
    c.SwaggerEndpoint("/swagger/admin/swagger.json", "Admin API V1");
    c.SwaggerEndpoint("/swagger/auth/swagger.json", "Auth API V1");
    c.SwaggerEndpoint("/swagger/trait/swagger.json", "Trait API V1");

    // Customize the Swagger UI
    c.UseRequestInterceptor("(request) => { request.headers.Authorization = 'Bearer ' + request.headers.Authorization; return request; }");
  });
}

// app.UseHttpsRedirection();

app.UseCors();

app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

// Custom Middlewares
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.Run();
return;

static async Task ApplyMigrationsAndSeedData(IServiceProvider serviceProvider) {
  // Create a scope to resolve scoped services
  using var scope = serviceProvider.CreateScope();
  var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
  var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
  var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<string>>>();

  // Apply migrations
  await dbContext.Database.MigrateAsync();

  // Seed data
  await DbInitializer.SeedData(scope.ServiceProvider);
}

