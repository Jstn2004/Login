using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Diagnostics;
using Login_Backend.Models;
using System.Security.Cryptography;


//  Admin@1.com

var builder = WebApplication.CreateBuilder(args);

var secretKey = Encoding.UTF8.GetBytes("p3gn'K,d\"[!IV'o@?[N>J'KSpw},bI_7"); // Hier sollte ein sicherer Schlüssel verwendet werden

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
}).AddJwtBearer("JwtBearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Login test", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();



app.UseAuthentication();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Login Test");
    });
}

app.MapPost("/login", async (HttpContext context) =>
{
    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();                //lädt die Daten vom Body ind requestBody
    var model = JsonSerializer.Deserialize<LoginViewModel>(requestBody);                            //Wandelt das JSON Objekt in ein LoginViewModel Objekt 
    Debug.WriteLine(model.email);
    Debug.WriteLine(model.password);
    var hashpassword = GenerateHashseperated(model.password);
    var hashemail = GenerateHashseperated(model.email); 
    // Hier wird die Benutzerauthentifizierung durchgeführt (z. B. mit ASP.NET Core Identity)
    using(var dbcontext = new MyDashCraftContext())
    {
        var userexists = dbcontext.Benutzers.Any(u => u.Email == hashemail);
        var passwordexists = dbcontext.Benutzers.Any(u => u.Passwort == hashpassword);

        if (userexists && passwordexists)
        {
            var token = GenerateJwtToken(model.email);
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsJsonAsync(new { Token = token });
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var succesDetails = new
            {
                title = "Benutzerdaten sind Falsch",
                status = StatusCodes.Status401Unauthorized,
                detail = "Benutzerdaten sind Falsch"
            };

            var json = JsonSerializer.Serialize(succesDetails);
            await context.Response.WriteAsync(json);
        }
    }
   
});

app.MapPost("/sign", async (HttpContext httpcontext) =>
{
    var requestBody = await new StreamReader(httpcontext.Request.Body).ReadToEndAsync();
    var model = JsonSerializer.Deserialize<LoginViewModel>(requestBody);
    var password = model.password;
    var email = model.email;
    var hashpassword = GenerateHashseperated(password);
    var hashemail = GenerateHashseperated(email); 
    var passwordemailhash = GenerateHash(email, password); 
    using (var context = new MyDashCraftContext())
    {
        var userexists = context.Benutzers.Any(u => u.EmailPasswordHash == passwordemailhash); 

        if (!userexists)
        {
            var newuser = new Benutzer
            {
                Name = model.name, 
                Email = hashemail,
                Passwort = hashpassword,
                Aktiviert = true,
                EmailPasswordHash = passwordemailhash,
                Erstellungsdatum = DateTime.Now,
                LetztesAnmeldedatum = DateTime.Now,
            };

            context.Benutzers.Add(newuser);
            
            var succesDetails = new
            {
                title = "Nutzer gespeichert",
                status = StatusCodes.Status200OK,
                detail = "Email und Password wurden in der Datenbank gespeichert"
            };
            var json = JsonSerializer.Serialize(succesDetails);
            await httpcontext.Response.WriteAsync(json);

            context.SaveChanges();
        }
        else
        {
            httpcontext.Response.StatusCode = StatusCodes.Status409Conflict;
            httpcontext.Response.ContentType = "application/json";

            var problemDetails = new
            {
                title = "Email bereits vorhanden",
                status = StatusCodes.Status409Conflict,
                detail = "Die angegebene E-Mail-Adresse ist bereits registriert."
            };

            var json = JsonSerializer.Serialize(problemDetails);
            await httpcontext.Response.WriteAsync(json);
        }
    }

});

app.MapGet("/Name",  (HttpContext httpContext) =>
{
    // Den Authorization-Header aus dem HTTP-Kontext abrufen
    string authorizationHeader = httpContext.Request.Headers["Authorization"];
    Debug.WriteLine(authorizationHeader);

    if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
    {
    
        Debug.WriteLine("Authorization-Header nicht gefunden oder ungültig.");
    }

    // Den JWT-Token extrahieren, indem "Bearer " entfernt wird
    var jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();

    string username = GenerateHashseperated(GetUsernameFromToken(jwtToken));
     

    using(var context = new MyDashCraftContext())
    {
        var userinformations = context.Benutzers.Where(u => u.Email == username).ToList(); 
        return userinformations; 
    }


});

 string GetUsernameFromToken(string jwtToken)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    string secretKey = "p3gn'K,d\"[!IV'o@?[N>J'KSpw},bI_7";

    try
    {
        // Setze die Token-Validierungseinstellungen, einschließlich des geheimen Schlüssels
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        // Validiere und entschlüssele den Token
        var principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out _);

        // Extrahiere Benutzername aus den Ansprüchen (Claims) des Tokens
        var username = principal.Identity.Name;

        return username;
    }
    catch (Exception ex)
    {
        // Fehlerbehandlung, z.B. Loggen des Fehlers
        Console.WriteLine("Fehler beim Extrahieren des Benutzernamens aus dem JWT-Token: " + ex.Message);
        return null;
    }
}



string GenerateHash(string email, string password)
{
    // Kombinieren Sie die E-Mail-Adresse und das Passwort zu einer Zeichenfolge
    string combinedString = email + password;

    // Konvertieren Sie die kombinierte Zeichenfolge in ein Byte-Array
    byte[] bytes = Encoding.UTF8.GetBytes(combinedString);

    // Erstellen Sie einen SHA256-Hashwert aus dem Byte-Array
    using (SHA256 sha256 = SHA256.Create())
    {
        byte[] hashBytes = sha256.ComputeHash(bytes);

        // Konvertieren Sie den Hashwert zurück in eine Zeichenfolge
        string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        return hash;
    }
}


string GenerateHashseperated(string input)
{
    // Kombinieren Sie die E-Mail-Adresse und das Passwort zu einer Zeichenfolge
 

    // Konvertieren Sie die kombinierte Zeichenfolge in ein Byte-Array
    byte[] bytes = Encoding.UTF8.GetBytes(input);

    // Erstellen Sie einen SHA256-Hashwert aus dem Byte-Array
    using (SHA256 sha256 = SHA256.Create())
    {
        byte[] hashBytes = sha256.ComputeHash(bytes);

        // Konvertieren Sie den Hashwert zurück in eine Zeichenfolge
        string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        return hash;
    }
}


string GenerateJwtToken(string email)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes("p3gn'K,d\"[!IV'o@?[N>J'KSpw},bI_7"); // Hier sollte der gleiche Schlüssel wie oben verwendet werden
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, email)
        }),
        Expires = DateTime.UtcNow.AddHours(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}

app.Run();


class LoginViewModel
{
    public string email { get; set; }
    public string password { get; set; }

    public string name { get; set; }
}

class User
{
    public string name { get; set; }
    public string email { get; set; }
    public string erstelldatum { get; set; }
    

}


