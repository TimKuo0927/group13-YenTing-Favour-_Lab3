using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace group_13_YenTing_Favour__Lab_3.Models;

public partial class Lab3Context : IdentityDbContext
{
    public Lab3Context()
    {
    }

    public Lab3Context(DbContextOptions<Lab3Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Episode> Episodes { get; set; }

    public virtual DbSet<Podcast> Podcasts { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    //public virtual DbSet<User> Users { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); 
        modelBuilder.Entity<Episode>(entity =>
        {
            entity.Property(e => e.EpisodeId).HasColumnName("EpisodeID");
            entity.Property(e => e.AudioFileUrl)
                .HasMaxLength(200)
                .HasColumnName("AudioFileURL");
            entity.Property(e => e.PodcastId).HasColumnName("PodcastID");
            entity.Property(e => e.ReleaseDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Podcast).WithMany(p => p.Episodes)
                .HasForeignKey(d => d.PodcastId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Episodes_Podcasts");
        });

        modelBuilder.Entity<Podcast>(entity =>
        {
            entity.Property(e => e.PodcastId).HasColumnName("PodcastID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatorId).HasColumnName("CreatorID");
            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(50);

            //entity.HasOne(d => d.Creator).WithMany(p => p.Podcasts)
            //    .HasForeignKey(d => d.CreatorId)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_Podcasts_User_ID");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.Property(e => e.SubscriptionId).HasColumnName("SubscriptionID");
            entity.Property(e => e.PodcastId).HasColumnName("PodcastID");
            entity.Property(e => e.SubscribedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Podcast).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.PodcastId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subscriptions_Podcasts");

            //entity.HasOne(d => d.User).WithMany(p => p.Subscriptions)
            //    .HasForeignKey(d => d.UserId)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_Subscriptions_Users");
        });

        //modelBuilder.Entity<User>(entity =>
        //{
        //    entity.Property(e => e.UserId)
        //        .HasDefaultValueSql("(newid())")
        //        .HasColumnName("UserID");
        //    entity.Property(e => e.Email).HasMaxLength(50);
        //    entity.Property(e => e.Password).HasMaxLength(512);
        //    entity.Property(e => e.Role).HasMaxLength(50);
        //    entity.Property(e => e.Username).HasMaxLength(50);
        //});

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
