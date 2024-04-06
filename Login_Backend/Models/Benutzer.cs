using System;
using System.Collections.Generic;

namespace Login_Backend.Models;

public partial class Benutzer
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Passwort { get; set; }

    public DateTime? Erstellungsdatum { get; set; }

    public DateTime? LetztesAnmeldedatum { get; set; }

    public string? Rolle { get; set; }

    public bool? Aktiviert { get; set; }

    public string? EmailPasswordHash { get; set; }
}
