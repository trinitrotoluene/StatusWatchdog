﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ServiceWatchdog.Api.Services;

namespace ServiceWatchdog.Api.Migrations
{
    [DbContext(typeof(WatchdogContext))]
    partial class WatchdogContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("ServiceWatchdog.Api.Services.Entities.IncidentModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("CausedStatus")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValueSql("timezone('utc', now())");

                    b.Property<int>("MostRecentUpdateId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("ResolvedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValue(null);

                    b.Property<int>("ServiceId")
                        .HasColumnType("integer");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("ServiceId");

                    b.ToTable("Incidents");
                });

            modelBuilder.Entity("ServiceWatchdog.Api.Services.Entities.IncidentUpdateModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValueSql("timezone('utc', now())");

                    b.Property<int>("IncidentId")
                        .HasColumnType("integer");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("character varying(2048)")
                        .HasMaxLength(2048);

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("IncidentId");

                    b.ToTable("IncidentUpdates");
                });

            modelBuilder.Entity("ServiceWatchdog.Api.Services.Entities.KeyValueModel", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("character varying(8192)")
                        .HasMaxLength(8192);

                    b.HasKey("Key");

                    b.ToTable("KeyValueStore");
                });

            modelBuilder.Entity("ServiceWatchdog.Api.Services.Entities.ServiceModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTimeOffset>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("timezone('utc', now())");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("character varying(32)")
                        .HasMaxLength(32);

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("character varying(32)")
                        .HasMaxLength(32);

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasColumnType("character varying(32)")
                        .HasMaxLength(32);

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.ToTable("Services");
                });

            modelBuilder.Entity("ServiceWatchdog.Api.Services.Entities.IncidentModel", b =>
                {
                    b.HasOne("ServiceWatchdog.Api.Services.Entities.ServiceModel", "Service")
                        .WithMany("Incidents")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ServiceWatchdog.Api.Services.Entities.IncidentUpdateModel", b =>
                {
                    b.HasOne("ServiceWatchdog.Api.Services.Entities.IncidentModel", "Incident")
                        .WithMany("Updates")
                        .HasForeignKey("IncidentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
