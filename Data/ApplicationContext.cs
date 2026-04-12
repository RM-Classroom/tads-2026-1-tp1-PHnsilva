using LocadoraVeiculosApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LocadoraVeiculosApi.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }

    public DbSet<Fabricante> Fabricantes => Set<Fabricante>();
    public DbSet<CategoriaVeiculo> CategoriasVeiculo => Set<CategoriaVeiculo>();
    public DbSet<Veiculo> Veiculos => Set<Veiculo>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Aluguel> Alugueis => Set<Aluguel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Fabricante>(entity =>
        {
            entity.ToTable("Fabricantes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nome).IsRequired().HasMaxLength(100);
            entity.Property(x => x.PaisOrigem).HasMaxLength(60);
        });

        modelBuilder.Entity<CategoriaVeiculo>(entity =>
        {
            entity.ToTable("CategoriasVeiculo");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nome).IsRequired().HasMaxLength(60);
            entity.Property(x => x.ValorDiariaBase).HasColumnType("decimal(10,2)");
        });

        modelBuilder.Entity<Veiculo>(entity =>
        {
            entity.ToTable("Veiculos");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Placa).IsUnique();
            entity.Property(x => x.Modelo).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Placa).IsRequired().HasMaxLength(10);

            entity.HasOne(x => x.Fabricante)
                .WithMany(x => x.Veiculos)
                .HasForeignKey(x => x.FabricanteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.CategoriaVeiculo)
                .WithMany(x => x.Veiculos)
                .HasForeignKey(x => x.CategoriaVeiculoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clientes");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Cpf).IsUnique();
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.Nome).IsRequired().HasMaxLength(120);
            entity.Property(x => x.Cpf).IsRequired().HasMaxLength(14);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(120);
            entity.Property(x => x.Telefone).HasMaxLength(20);
        });

        modelBuilder.Entity<Aluguel>(entity =>
        {
            entity.ToTable("Alugueis");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ValorDiaria).HasColumnType("decimal(10,2)");
            entity.Property(x => x.ValorTotal).HasColumnType("decimal(10,2)");

            entity.HasOne(x => x.Cliente)
                .WithMany(x => x.Alugueis)
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Veiculo)
                .WithMany(x => x.Alugueis)
                .HasForeignKey(x => x.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
