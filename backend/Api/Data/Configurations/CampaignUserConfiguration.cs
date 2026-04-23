using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations;

public class CampaignUserConfiguration : IEntityTypeConfiguration<CampaignUser>
{
    public void Configure(EntityTypeBuilder<CampaignUser> builder)
    {
        builder.HasKey(campusr => campusr.Id);

        // Relacionamento com Usuário
        builder.HasOne(campusr => campusr.User)
               .WithMany() 
               .HasForeignKey(campusr => campusr.UserId);

        builder.HasOne(campusr => campusr.Campaign)
               .WithMany(camp => camp.CampaignMembers) 
               .HasForeignKey(campusr => campusr.CampaignId);
    }
}