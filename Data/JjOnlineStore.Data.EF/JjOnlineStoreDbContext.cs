﻿using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreTemplate.Data.Models;
using JjOnlineStore.Data.Entities;
using JjOnlineStore.Data.Entities.Base;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace JjOnlineStore.Data.EF
{
	public class JjOnlineStoreDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
	{
		public JjOnlineStoreDbContext(DbContextOptions<JjOnlineStoreDbContext> options)
			: base(options)
		{
		}

		private IDbContextTransaction currentTransaction;


		public virtual void BeginTransaction()
		{
			if(this.currentTransaction != null)
			{
				return;
			}

			this.currentTransaction = Database.BeginTransaction(IsolationLevel.ReadCommitted);
		}

		public virtual async Task CommitTransactionAsync()
		{
			try
			{
				await SaveChangesAsync();

				this.currentTransaction?.Commit();
			}
			catch(Exception)
			{
				RollbackTransaction();
			}
			finally
			{
				if(this.currentTransaction != null)
				{
					this.currentTransaction.Dispose();
					this.currentTransaction = null;
				}
			}
		}

		public virtual void RollbackTransaction()
		{
			try
			{
				this.currentTransaction?.Rollback();
			}
			finally
			{
				if(this.currentTransaction != null)
				{
					this.currentTransaction.Dispose();
					this.currentTransaction = null;
				}
			}
		}

		public override int SaveChanges(bool acceptAllChangesOnSuccess)
		{
			this.ApplyAuditInfoRules();
			return base.SaveChanges(acceptAllChangesOnSuccess);
		}

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
			this.SaveChangesAsync(true , cancellationToken);

		public override Task<int> SaveChangesAsync(
			bool acceptAllChangesOnSuccess ,
			CancellationToken cancellationToken = default)
		{
			this.ApplyAuditInfoRules();
			return base.SaveChangesAsync(acceptAllChangesOnSuccess , cancellationToken);
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			ConfigureUserIdentityRelations(builder);
		}

		private static void ConfigureUserIdentityRelations(ModelBuilder builder)
		{
			builder.Entity<ApplicationUser>()
				.HasMany(e => e.Claims)
				.WithOne()
				.HasForeignKey(e => e.UserId)
				.IsRequired()
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<ApplicationUser>()
				.HasMany(e => e.Logins)
				.WithOne()
				.HasForeignKey(e => e.UserId)
				.IsRequired()
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<ApplicationUser>()
				.HasMany(e => e.Roles)
				.WithOne()
				.HasForeignKey(e => e.UserId)
				.IsRequired()
				.OnDelete(DeleteBehavior.Restrict);
		}

		private static void SetIsDeletedQueryFilter<T>(ModelBuilder builder)
			where T : class, IDeletableEntity
		{
			builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
		}

		private void ApplyAuditInfoRules()
		{
			var changedEntries = this.ChangeTracker
				.Entries()
				.Where(e =>
					e.Entity is IAuditInfo &&
					(e.State == EntityState.Added || e.State == EntityState.Modified));

			foreach(var entry in changedEntries)
			{
				var entity = (IAuditInfo)entry.Entity;
				if(entry.State == EntityState.Added && entity.CreatedOn == default)
				{
					entity.CreatedOn = DateTime.UtcNow;
				}
				else
				{
					entity.ModifiedOn = DateTime.UtcNow;
				}
			}
		}
	}
}