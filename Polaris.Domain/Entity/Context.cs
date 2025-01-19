using Polaris.Domain.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Polaris.Domain.Entity
{
    public partial class PolarisContext : DbContext
    {
        public PolarisContext()
        {
        }

        public PolarisContext(DbContextOptions<PolarisContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Application> Application { get; set; }

        public virtual DbSet<Authentication> Authentication { get; set; }

        public virtual DbSet<Member> Member { get; set; }

        public virtual DbSet<User> User { get; set; }
        
        public virtual DbSet<RefreshToken> RefreshToken { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DatabaseConfig.ConnectionString);
            }
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Application>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_APPLICATION");

                entity.ToTable("application");

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Authentication>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_AUTHENTICATION");

                entity.ToTable("authentication");

                entity.HasIndex(e => e.MemberId, "UQ_AUTHENTICATION_MEMBER").IsUnique();

                entity.Property(e => e.Code)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("code");

                entity.Property(e => e.CodeAttempt)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("codeAttempt");

                entity.Property(e => e.CodeExpiration)
                    .HasColumnType("datetime")
                    .HasColumnName("codeExpiration");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.MemberId).HasColumnName("memberId");

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.HasOne(d => d.MemberNavigation).WithOne(x => x.AuthenticationNavigation)
                    .HasForeignKey<Authentication>(d => d.MemberId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_AUTHENTICATION_MEMBER");
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_MEMBER");

                entity.ToTable("member");

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("id");

                entity.Property(e => e.ApplicationId).HasColumnName("applicationId");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.ApplicationNavigation).WithMany(p => p.MemberNavigation)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MEMBER_APPLICATION");

                entity.HasOne(d => d.UserNavigation).WithMany(p => p.MemberNavigation)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_MEMBER_USER");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_USER");

                entity.ToTable("user");

                entity.HasIndex(e => e.Email, "UQ_USER_EMAIL").IsUnique();

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("id");

                entity.Property(e => e.Email)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Language)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("language");

                entity.Property(e => e.Name)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });
            
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_REFRESH_TOKEN");

                entity.ToTable("refreshToken");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.AuthenticationNavigation)
                      .WithMany(p => p.RefreshTokenNavigation)
                      .HasForeignKey(d => d.AuthenticationId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_REFRESH_TOKEN_AUTHENTICATION");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entities = ChangeTracker.Entries().Where(e => e.State == EntityState.Added ||
                                                              e.State == EntityState.Modified ||
                                                              e.State == EntityState.Deleted).ToList();

            var result = await base.SaveChangesAsync(cancellationToken);
            foreach (var entity in entities)
            {
                Entry(entity.Entity).State = EntityState.Detached;
            }
            return result;
        }

        public override int SaveChanges()
        {
            var entities = ChangeTracker.Entries().Where(e => e.State == EntityState.Added ||
                                                              e.State == EntityState.Modified ||
                                                              e.State == EntityState.Deleted).ToList();

            var result = base.SaveChanges();
            foreach (var entity in entities)
            {
                Entry(entity.Entity).State = EntityState.Detached;
            }
            return result;
        }
    }
}
