using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HouseMatcher.Models
{
    public partial class HouseMatcherContext : DbContext
    {
        public HouseMatcherContext()
        {
        }

        public HouseMatcherContext(DbContextOptions<HouseMatcherContext> options)
            : base(options)
        {
        }

        public virtual DbSet<HouseData> HouseData { get; set; } = null!;
        public virtual DbSet<UserData> UserData { get; set; } = null!;
        public virtual DbSet<MessageData> MessageData { get; set; } = null!;
        public virtual DbSet<FeatureLabelList> FeatureLabelList { get; set; } = null!;
        public virtual DbSet<FeatureLabelDictionary> FeatureLabelDictionary { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HouseData>(entity =>
            {
                entity.HasKey(e => e.HouseId);

                entity.Property(e => e.UserId);

                entity.Property(e => e.HouseName).HasMaxLength(50);

                entity.Property(e => e.Location).HasMaxLength(50);

                entity.Property(e => e.Latitude);

                entity.Property(e => e.Longitude);

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.District).HasMaxLength(50);

                entity.Property(e => e.Road).HasMaxLength(50);

                entity.Property(e => e.UpdateTime);

                entity.Property(e => e.HouseImg);
            });

            modelBuilder.Entity<UserData>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserEmail).HasMaxLength(50);

                entity.Property(e => e.UserLocation).HasMaxLength(50);

                entity.Property(e => e.UserName).HasMaxLength(50);

                entity.Property(e => e.UserDescription);

                entity.Property(e => e.UserPhoneNumber)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.UserPassword).HasMaxLength(50);

                entity.Property(e => e.UserImgId).HasMaxLength(50);
            });

            modelBuilder.Entity<MessageData>(entity =>
            {
                entity.HasKey(e => e.MessageId);

                entity.Property(e => e.MessageDescription).HasMaxLength(50);

                entity.Property(e => e.SenderId);

                entity.Property(e => e.ReceiverId);

                entity.Property(e => e.CreatedTime).HasColumnType("datetime");

                entity.Property(e => e.HouseDataId);
            });

            modelBuilder.Entity<FeatureLabelList>(entity =>
            {
                entity.HasKey(e => e.LabelId);

                entity.Property(e => e.LabelName).HasMaxLength(50);
            });

            modelBuilder.Entity<FeatureLabelDictionary>(entity =>
            {
                entity.HasKey(e => e.DictionaryId);

                entity.Property(e => e.HouseDataId);

                entity.Property(e => e.FeatureLabelId);

                entity.Property(e => e.FeatureLabelName);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
