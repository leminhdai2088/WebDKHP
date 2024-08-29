namespace dev_DKHP.CoreModule.Model
{
    using dev_DKHP.CoreModule.Dto;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    public class DKHPDbContext:     IdentityDbContext<TL_USER_ENTITY, IdentityRole<string>, string>
    {
        public DbSet<CLASS_ENTITY> ClassEntities { get; set; }
        public DbSet<DEPARTMENT_ENTITY> DeploymentEntities { get; set; }
        public DbSet<ENROLLED_STUDENT_ENTITY> eNROLLED_STUDENT_ENTITies { get; set; }
        public DbSet<REQUIRED_SUBJECT_ENTITY> rEQUIRED_SUBJECT_ENTITies{ get; set; }
        public DbSet<SUBJECT_ENTITY> SubjectEntities { get; set; }
        public DbSet<TL_USER_ENTITY> TLUserEntities { get; set; }
        public DbSet<EMAIL_SENDER_ENTITY> EmailSenderEntities { get; set; }
        public DKHPDbContext(DbContextOptions<DKHPDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<CLASS_ENTITY>().HasKey(e => e.CLASS_ID);
            builder.Entity<DEPARTMENT_ENTITY>().HasKey(e => e.DEP_ID);
            builder.Entity<ENROLLED_STUDENT_ENTITY>().HasKey(e => e.ENROLLED_ID);
            builder.Entity<REQUIRED_SUBJECT_ENTITY>().HasKey(e => new { e.SUBJECT_ID, e.REQUIRED_SUBJECT_ID });
            builder.Entity<SUBJECT_ENTITY>().HasKey(e => e.SUBJECT_ID);
            builder.Entity<TL_USER_ENTITY>().HasKey(e => e.Id);
            builder.Entity<EMAIL_SENDER_ENTITY>().HasKey(e => e.EMAIL_ID);
        }
    }
}
