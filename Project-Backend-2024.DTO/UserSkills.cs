﻿namespace Project_Backend_2024.DTO;

public class UserSkills
{
    public User? User { get; set; }
    public int UserID { get; set; }

    public Skill? Skill { get; set; }
    public int SkillID { get; set; }
}