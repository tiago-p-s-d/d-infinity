using Api.Models.Gameplay;
using Api.Models.Gameplay.Groups; 


public class SystemRaceGroup {
    public int Id { get; set; } 
    public int SystemId { get; set; }
    public SystemModel System { get; set; } = null!;
    public int RaceGroupId { get; set; }
    public RaceGroup RaceGroup { get; set; } = null!;
}


public class SystemItemGroup {
    public int Id { get; set; }
    public int SystemId { get; set; }
    public SystemModel System { get; set; } = null!;
    public int ItemGroupId { get; set; }
    public ItemGroup ItemGroup { get; set; } = null!;
}

public class SystemSpellGroup {
    public int Id { get; set; }
    public int SystemId { get; set; }
    public SystemModel System { get; set; } = null!;
    public int SpellGroupId { get; set; }
    public SpellGroup SpellGroup { get; set; } = null!;
}

public class SystemSkillGroup {
    public int Id { get; set; }
    public int SystemId { get; set; }
    public SystemModel System { get; set; } = null!;
    public int SkillGroupId { get; set; }
    public SkillGroup SkillGroup { get; set; } = null!;
}

public class SystemMapGroup {
    public int Id { get; set; }
    public int SystemId { get; set; }
    public SystemModel System { get; set; } = null!;
    public int MapGroupId { get; set; }
    public MapGroup MapGroup { get; set; } = null!;
}