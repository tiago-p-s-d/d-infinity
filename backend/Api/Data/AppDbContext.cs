using Api.Models;
using Api.Models.Gameplay; // Essencial para encontrar as classes na subpasta
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // --- Identidade e Acesso ---
    public DbSet<User> Users { get; set; } = null!;
    
    // --- Campanhas e Social ---
    public DbSet<Campaign> Campaigns { get; set; } = null!;
    public DbSet<CampaignUser> CampaignMembers { get; set; } = null!;

    // --- Core do Gameplay (Fichas e Sistemas) ---
    public DbSet<SystemModel> Systems { get; set; } = null!;        
    public DbSet<CharacterSheet> CharacterSheets { get; set; } = null!;
    public DbSet<CharacterSheetModel> CharacterSheetModels { get; set; } = null!;
    public DbSet<Race> Races { get; set; } = null!;
    public DbSet<MapModel> Maps { get; set; } = null!;

    // --- Itens e Inventário ---
    public DbSet<Item> Items { get; set; } = null!;
    public DbSet<Inventory> Inventories { get; set; } = null!;
    public DbSet<InventoryItem> InventoryItems { get; set; } = null!;

    // --- Magias e Perícias (Conhecimento) ---
    public DbSet<Skill> Skills { get; set; } = null!;
    public DbSet<KnownSkill> KnownSkills { get; set; } = null!;
    public DbSet<Spell> Spells { get; set; } = null!;
    public DbSet<KnownSpell> KnownSpells { get; set; } = null!;

    // --- Economia ---
    public DbSet<Currency> Currencies { get; set; } = null!;
    public DbSet<CurrencyValue> CurrencyValues { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Isso garante que qualquer configuração adicional (como restrições de JSON) 
        // seja lida de arquivos de configuração separados se você os criar.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Opcional: Configurar Delete Cascade para mapas quando a campanha for deletada
        modelBuilder.Entity<MapModel>()
            .HasOne(m => m.Campaign)
            .WithMany(c => c.Maps)
            .HasForeignKey(m => m.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}