using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lagoon.Model.EntityConfigurations;

/// <summary>
/// GridView profile configuration
/// </summary>
internal class GridViewProfileConfiguration : IEntityTypeConfiguration<GridViewProfile>
{
    ///<inheritdoc/>
    public void Configure(EntityTypeBuilder<GridViewProfile> builder)
    {
        JsonSerializerOptions options = new()
        { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull };
        // This Converter will perform the conversion to and from Json to the desired type
        builder.Property(e => e.Columns).HasConversion(
            v => JsonSerializer.Serialize(v, options),
            v => JsonSerializer.Deserialize<List<GridViewColumnProfile>>(v, options));
        builder.Property(e => e.Groups).HasConversion(
            v => JsonSerializer.Serialize(v, options),
            v => JsonSerializer.Deserialize<List<GridViewGroupProfile>>(v, options));
        // Remove value comparer warning 
        ValueComparer<List<GridViewGroupProfile>> groupValueComparer = new((c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());
        ValueComparer<List<GridViewColumnProfile>> columnValueComparer = new((c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());
        builder.Property(e => e.Columns).Metadata.SetValueComparer(columnValueComparer);
        builder.Property(e => e.Groups).Metadata.SetValueComparer(groupValueComparer);
    }
}
