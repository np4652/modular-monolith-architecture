using App.BuildingBlocks.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace App.BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Common conventions every module's DbContext must follow: explicit snake_case
/// naming and an automatic "not deleted" filter for soft-deletable entities.
/// Auditing, soft-delete conversion and domain event dispatch are applied uniformly
/// via the interceptors registered when the context is added to the container.
/// </summary>
public abstract class ModuleDbContextBase : DbContext
{
    protected ModuleDbContextBase(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            ApplySnakeCaseNaming(modelBuilder, entityType);
            ApplySoftDeleteFilter(modelBuilder, entityType);
        }
    }

    private static void ApplySnakeCaseNaming(ModelBuilder modelBuilder, Microsoft.EntityFrameworkCore.Metadata.IMutableEntityType entityType)
    {
        entityType.SetTableName(ToSnakeCase(entityType.GetTableName()!));

        foreach (var property in entityType.GetProperties())
            property.SetColumnName(ToSnakeCase(property.Name));

        foreach (var key in entityType.GetKeys())
            key.SetName(ToSnakeCase(key.GetName()!));

        foreach (var foreignKey in entityType.GetForeignKeys())
            foreignKey.SetConstraintName(ToSnakeCase(foreignKey.GetConstraintName()!));

        foreach (var index in entityType.GetIndexes())
        {
            var name = index.GetDatabaseName();
            if (name is not null)
                index.SetDatabaseName(ToSnakeCase(name));
        }
    }

    private static void ApplySoftDeleteFilter(ModelBuilder modelBuilder, Microsoft.EntityFrameworkCore.Metadata.IMutableEntityType entityType)
    {
        if (!typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType) || entityType.ClrType.IsAbstract)
            return;

        var method = typeof(ModuleDbContextBase)
            .GetMethod(nameof(BuildSoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .MakeGenericMethod(entityType.ClrType);

        var filter = method.Invoke(null, null);
        modelBuilder.Entity(entityType.ClrType).HasQueryFilter((System.Linq.Expressions.LambdaExpression)filter!);
    }

    private static System.Linq.Expressions.Expression<Func<TEntity, bool>> BuildSoftDeleteFilter<TEntity>()
        where TEntity : class, ISoftDeletable =>
        entity => !entity.IsDeleted;

    private static string ToSnakeCase(string value)
    {
        if (string.IsNullOrEmpty(value)) return value;

        var builder = new System.Text.StringBuilder();
        for (var i = 0; i < value.Length; i++)
        {
            var current = value[i];
            if (char.IsUpper(current))
            {
                if (i > 0) builder.Append('_');
                builder.Append(char.ToLowerInvariant(current));
            }
            else
            {
                builder.Append(current);
            }
        }

        return builder.ToString();
    }
}
