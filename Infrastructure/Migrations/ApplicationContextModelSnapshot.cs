using System;
using LocadoraVeiculosApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace LocadoraVeiculosApi.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    partial class ApplicationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            modelBuilder.Entity("LocadoraVeiculosApi.Domain.Entities.Aluguel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");
                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
                    b.Property<int>("ClienteId").HasColumnType("int");
                    b.Property<DateTime?>("DataDevolucao").HasColumnType("datetime2");
                    b.Property<DateTime>("DataFimPrevista").HasColumnType("datetime2");
                    b.Property<DateTime>("DataInicio").HasColumnType("datetime2");
                    b.Property<int?>("QuilometragemFinal").HasColumnType("int");
                    b.Property<int>("QuilometragemInicial").HasColumnType("int");
                    b.Property<decimal>("ValorDiaria").HasColumnType("decimal(10,2)");
                    b.Property<decimal>("ValorTotal").HasColumnType("decimal(10,2)");
                    b.Property<int>("VeiculoId").HasColumnType("int");
                    b.HasKey("Id");
                    b.HasIndex("ClienteId");
                    b.HasIndex("VeiculoId");
                    b.ToTable("Alugueis", (string)null);
                });

            modelBuilder.Entity("LocadoraVeiculosApi.Domain.Entities.CategoriaVeiculo", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
                    b.Property<string>("Nome").IsRequired().HasMaxLength(60).HasColumnType("nvarchar(60)");
                    b.Property<decimal>("ValorDiariaBase").HasColumnType("decimal(10,2)");
                    b.HasKey("Id");
                    b.ToTable("CategoriasVeiculo", (string)null);
                });

            modelBuilder.Entity("LocadoraVeiculosApi.Domain.Entities.Cliente", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
                    b.Property<string>("Cpf").IsRequired().HasMaxLength(14).HasColumnType("nvarchar(14)");
                    b.Property<string>("Email").IsRequired().HasMaxLength(120).HasColumnType("nvarchar(120)");
                    b.Property<string>("Nome").IsRequired().HasMaxLength(120).HasColumnType("nvarchar(120)");
                    b.Property<string>("Telefone").HasMaxLength(20).HasColumnType("nvarchar(20)");
                    b.HasKey("Id");
                    b.HasIndex("Cpf").IsUnique();
                    b.HasIndex("Email").IsUnique();
                    b.ToTable("Clientes", (string)null);
                });

            modelBuilder.Entity("LocadoraVeiculosApi.Domain.Entities.Fabricante", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
                    b.Property<string>("Nome").IsRequired().HasMaxLength(100).HasColumnType("nvarchar(100)");
                    b.Property<string>("PaisOrigem").HasMaxLength(60).HasColumnType("nvarchar(60)");
                    b.HasKey("Id");
                    b.ToTable("Fabricantes", (string)null);
                });

            modelBuilder.Entity("LocadoraVeiculosApi.Domain.Entities.Veiculo", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
                    b.Property<int>("AnoFabricacao").HasColumnType("int");
                    b.Property<int>("CategoriaVeiculoId").HasColumnType("int");
                    b.Property<int>("FabricanteId").HasColumnType("int");
                    b.Property<string>("Modelo").IsRequired().HasMaxLength(100).HasColumnType("nvarchar(100)");
                    b.Property<string>("Placa").IsRequired().HasMaxLength(10).HasColumnType("nvarchar(10)");
                    b.Property<int>("QuilometragemAtual").HasColumnType("int");
                    b.HasKey("Id");
                    b.HasIndex("CategoriaVeiculoId");
                    b.HasIndex("FabricanteId");
                    b.HasIndex("Placa").IsUnique();
                    b.ToTable("Veiculos", (string)null);
                });

            modelBuilder.Entity("LocadoraVeiculosApi.Domain.Entities.Aluguel", b =>
                {
                    b.HasOne("LocadoraVeiculosApi.Domain.Entities.Cliente", "Cliente")
                        .WithMany("Alugueis")
                        .HasForeignKey("ClienteId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.HasOne("LocadoraVeiculosApi.Domain.Entities.Veiculo", "Veiculo")
                        .WithMany("Alugueis")
                        .HasForeignKey("VeiculoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("Cliente");
                    b.Navigation("Veiculo");
                });

            modelBuilder.Entity("LocadoraVeiculosApi.Domain.Entities.Veiculo", b =>
                {
                    b.HasOne("LocadoraVeiculosApi.Domain.Entities.CategoriaVeiculo", "CategoriaVeiculo")
                        .WithMany("Veiculos")
                        .HasForeignKey("CategoriaVeiculoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.HasOne("LocadoraVeiculosApi.Domain.Entities.Fabricante", "Fabricante")
                        .WithMany("Veiculos")
                        .HasForeignKey("FabricanteId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("CategoriaVeiculo");
                    b.Navigation("Fabricante");
                });

            modelBuilder.Entity("LocadoraVeiculosApi.Domain.Entities.CategoriaVeiculo", b => { b.Navigation("Veiculos"); });
            modelBuilder.Entity("LocadoraVeiculosApi.Domain.Entities.Cliente", b => { b.Navigation("Alugueis"); });
            modelBuilder.Entity("LocadoraVeiculosApi.Domain.Entities.Fabricante", b => { b.Navigation("Veiculos"); });
#pragma warning restore 612, 618
        }
    }
}
