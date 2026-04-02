using Api.Models;
using Api.Models.Gameplay;
using Api.Models.Gameplay.Groups;
using Microsoft.EntityFrameworkCore;
using Api.Models.User;

namespace Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserVerification> UserVerifications { get; set; } = null!;

    public DbSet<Campaign> Campaigns { get; set; } = null!;
    public DbSet<CampaignUser> CampaignMembers { get; set; } = null!;

    public DbSet<SystemModel> Systems { get; set; } = null!;        
    public DbSet<CharacterSheet> CharacterSheets { get; set; } = null!;
    public DbSet<CharacterSheetModel> CharacterSheetModels { get; set; } = null!;
    public DbSet<Race> Races { get; set; } = null!;
    public DbSet<MapModel> Maps { get; set; } = null!;
    public DbSet<MapGroup> MapGroups { get; set; } = null!;
    public DbSet<SpellGroup> SpellGroups { get; set; } = null!;
    public DbSet<RaceGroup> RaceGroups { get; set; } = null!;
    public DbSet<ItemGroup> ItemGroups { get; set; } = null!;
    public DbSet<SkillGroup> SkillGroups { get; set; } = null!;
    public DbSet<Item> Items { get; set; } = null!;
    public DbSet<Inventory> Inventories { get; set; } = null!;
    public DbSet<InventoryItem> InventoryItems { get; set; } = null!;
    public DbSet<ClassModel> Classes { get; set; } = null!;
    public DbSet<ClassGroup> ClassGroups { get; set; } = null!;
    public DbSet<Skill> Skills { get; set; } = null!;
    public DbSet<KnownSkill> KnownSkills { get; set; } = null!;
    public DbSet<Spell> Spells { get; set; } = null!;
    public DbSet<KnownSpell> KnownSpells { get; set; } = null!;

    public DbSet<Currency> Currencies { get; set; } = null!;
    public DbSet<CurrencyValue> CurrencyValues { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

    }
}