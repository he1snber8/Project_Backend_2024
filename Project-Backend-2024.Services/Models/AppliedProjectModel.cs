﻿namespace Project_Backend_2024.Services.Models;

public class AppliedProjectModel : IBasicModel
{
    public string? CoverLetter { get; set; }
    public string ApplicationStatus { get; set; } = null!;
    public int ProjectID { get; set; }
    public int UserID { get; set; }
}
