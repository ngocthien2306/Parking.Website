using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.WellKnownTypes;
using InfrastructureCore.Extensions;
using InfrastructureCore.Models.Identity;
using InfrastructureCore.Models.Menu;
using InfrastructureCore.Models.Site;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureCore.Web.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SYSite> Sites { get; set; }
        public DbSet<SYUser> Users { get; set; }
        public DbSet<SYUserGroups> UserGroups{ get; set; }
        public DbSet<SYUsersInGroup> UsersInGroup { get; set; }

        public DbSet<SYMenu> Menus { get; set; }
        public DbSet<SYGroupAccessMenus> GroupAccessMenus { get; set; }
        public DbSet<SYUserAccessMenus> UserAccessMenus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SYSite>(etb =>
            {
                etb.HasKey(e => e.SiteID);
                etb.Property(e => e.SiteID).HasColumnName("SITE_ID").ValueGeneratedOnAdd().HasDefaultValueSql("NEWID()");
                etb.Property(e => e.SiteCode).HasColumnName("SITE_CD");
                etb.Property(e => e.SiteName).HasColumnName("SITE_NAME");
                etb.Property(e => e.SiteDescription).HasColumnName("SITE_DESCRIPTION");
                etb.Property(e => e.ChangePassPeriod).HasColumnName("PASS_CHANGE_PERIOD");
                etb.Property(e => e.FailedWaitTime).HasColumnName("FAILED_WAIT_TIME");
                etb.Property(e => e.MaxLogFail).HasColumnName("MAX_LOG_FAIL");
                etb.Property(e => e.SessionTimeOut).HasColumnName("SESSION_TIMEOUT");
                etb.ToTable("SYSite");
            });

            modelBuilder.Entity<SYUser>(etb =>
            {
                etb.HasKey(e => e.UserID);
                etb.Property(e => e.UserID).HasColumnName("USER_ID").ValueGeneratedOnAdd().HasDefaultValueSql("NEWID()");
                etb.Property(e => e.UserCode).HasColumnName("USER_CODE");
                etb.Property(e => e.Email).HasColumnName("EMAIL");
                etb.Property(e => e.FirstName).HasColumnName("FIRST_NAME");
                etb.Property(e => e.LastName).HasColumnName("LAST_NAME");
                etb.Property(e => e.UserName).HasColumnName("USER_NAME");
                etb.Property(e => e.Password).HasColumnName("PASSWORD");
                etb.Property(e => e.UserType).HasColumnName("USER_TYPE");
                etb.Property(e => e.SiteID).HasColumnName("SITE_ID");
                etb.Property(e => e.UseYN).HasColumnName("USE_YN");
                etb.Property(e => e.IsBlock).HasColumnName("IS_BLOCK");
                etb.Property(e => e.IsCount).HasColumnName("IS_COUNT");
                etb.Property(e => e.LastLoggedIn).HasColumnName("LAST_LOGIN");
                etb.Property(e => e.LastBlock).HasColumnName("LAST_BLOCK");
                etb.Property(e => e.LastPassChange).HasColumnName("LAST_PASS_CHANGE");
                etb.Property(e => e.SystemUserType).HasColumnName("SYSTEM_USER_TYPE");
                etb.Property(e => e.PartnerName).HasColumnName("PartnerName");
                etb.Property(e => e.PoMessage).HasColumnName("PoMessage");



                etb.ToTable("SYUser");
            });

            modelBuilder.Entity<SYUserGroups>(etb =>
            {
                etb.HasKey(e => e.GROUP_ID);
                etb.Property(e => e.GROUP_ID).HasColumnName("GROUP_ID");
                etb.Property(e => e.GROUP_NAME).HasColumnName("GROUP_NAME");
                etb.Property(e => e.DESCRIPTION).HasColumnName("DESCRIPTION");
                etb.Property(e => e.SITE_ID).HasColumnName("SITE_ID");
                etb.ToTable("SYUserGroups");
            });

            modelBuilder.Entity<SYUsersInGroup>(etb =>
            {
                etb.HasKey(e => new { e.USER_CODE, e.GROUP_ID});
                etb.Property(e => e.GROUP_ID);
                etb.Property(e => e.USER_CODE);
                etb.Property(e => e.SITE_ID);
                etb.ToTable("SYUsersInGroup");
            }
            );

            modelBuilder.Entity<SYMenu>(etb =>
            {
                etb.HasKey(e => e.MenuID);
                etb.Property(e => e.MenuID).ValueGeneratedOnAdd().UseSqlServerIdentityColumn<int>();
                etb.Property(e => e.SiteID).HasColumnName("SITE_ID");
                etb.Property(e => e.MenuName).HasColumnName("MENU_NM");
                etb.Property(e => e.MenuPath).HasColumnName("MENU_PATH");
                etb.Property(e => e.MenuLevel).HasColumnName("MENU_LEVEL");
                etb.Property(e => e.MenuParentID).HasColumnName("MENU_PARENT_ID");
                etb.Property(e => e.MenuSeq).HasColumnName("MENU_SEQ");
                etb.Property(e => e.AdminLevel).HasColumnName("ADMIN_LV");
                etb.Property(e => e.MenuType).HasColumnName("MENU_TYPE");
                etb.Property(e => e.ProgramID).HasColumnName("PROG_ID");
                etb.Property(e => e.MobileUse).HasColumnName("MOBILE_USE_YN");
                etb.Property(e => e.IntraUse).HasColumnName("INTRA_USE_YN");
                etb.Property(e => e.StartupPageUse).HasColumnName("STARTUP_PAGE_USE");
                etb.Property(e => e.IsCanClose).HasColumnName("IS_CAN_CLOSE");
                etb.Property(e => e.MenuDesc).HasColumnName("MENU_DESC");
                etb.Property(e => e.UseYN).HasColumnName("USE_YN"); 
                etb.Property(e => e.HiddenYN).HasColumnName("HIDDEN_YN");
                etb.ToTable("SYMenu");
            }
            );

            modelBuilder.Entity<SYGroupAccessMenus>(etb =>
            {
                etb.HasKey(e => new { e.GROUP_ID, e.MENU_ID, e.SITE_ID });
                etb.ToTable("SYGroupAccessMenus");
            }
            );

            modelBuilder.Entity<SYUserAccessMenus>(etb =>
            {
                etb.HasKey(e => new { e.USER_ID, e.MENU_ID, e.SITE_ID });
                etb.ToTable("SYUserAccessMenus");
            }
            );
        }
    }
}
