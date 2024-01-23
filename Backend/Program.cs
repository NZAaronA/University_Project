
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using A2.Data;
using A2.Handler;
using A2.Helper;
using Microsoft.AspNetCore.Authentication.OAuth;


public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, A2AuthHandler>("Authentication", null);

        builder.Services.AddDbContext<A2DbContext>(options => options.UseSqlite(builder.Configuration["WebAPIConnection"]));
        builder.Services.AddScoped<IA2Repo, A2Repo>();

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RegisteredUserOnly",
                policy => policy.RequireClaim("normalUser"));
            options.AddPolicy("OrganizerOnly",
                policy => policy.RequireClaim("Organizer"));
            options.AddPolicy("RegisteredUsersAndOrganizers", policy => {
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                    (c.Type == "normalUser" || c.Type == "Organizer")));
            });
        });
        builder.Services.AddMvc(options => options.OutputFormatters.Add(new CalendarOutputFormatter()));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}