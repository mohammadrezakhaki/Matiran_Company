/* 
    In the name of Allah
    Author: Mohammad Reza Khaki
    email : khaki,mohammad@gmail.com - mohammadreza.khaki.14012@gmail.com
    Mobile: 09126474851
    This is a test project for MetIran company
*/


using Matiran.Library.Data.Contracts;
using Matiran.Library.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

//---------------------------------------------------------------------------------
// Add injections.
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IMemberService, MemberService>();

builder.Services.AddScoped<IRentRepository, RentRepository>();
builder.Services.AddScoped<IRentService, RentService>();
builder.Services.AddScoped<IRentValidator, RentValidator>();
//---------------------------------------------------------------------------------


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

