﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GlamlyData
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class GlamlyEntities : DbContext
    {
        public GlamlyEntities()
            : base("name=GlamlyEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<wp_commentmeta> wp_commentmeta { get; set; }
        public virtual DbSet<wp_comments> wp_comments { get; set; }
        public virtual DbSet<wp_duplicator_packages> wp_duplicator_packages { get; set; }
        public virtual DbSet<wp_glamly_services> wp_glamly_services { get; set; }
        public virtual DbSet<wp_links> wp_links { get; set; }
        public virtual DbSet<wp_mondula_form_wizards> wp_mondula_form_wizards { get; set; }
        public virtual DbSet<wp_options> wp_options { get; set; }
        public virtual DbSet<wp_postmeta> wp_postmeta { get; set; }
        public virtual DbSet<wp_posts> wp_posts { get; set; }
        public virtual DbSet<wp_term_relationships> wp_term_relationships { get; set; }
        public virtual DbSet<wp_term_taxonomy> wp_term_taxonomy { get; set; }
        public virtual DbSet<wp_termmeta> wp_termmeta { get; set; }
        public virtual DbSet<wp_terms> wp_terms { get; set; }
        public virtual DbSet<wp_usermeta> wp_usermeta { get; set; }
        public virtual DbSet<wp_users> wp_users { get; set; }
        public virtual DbSet<wp_cf7_data> wp_cf7_data { get; set; }
        public virtual DbSet<wp_cf7_data_entry> wp_cf7_data_entry { get; set; }
        public virtual DbSet<wp_glamly_servicestypes> wp_glamly_servicestypes { get; set; }
        public virtual DbSet<wp_glamly_payment> wp_glamly_payment { get; set; }
        public virtual DbSet<wp_glamly_userresetpassword> wp_glamly_userresetpassword { get; set; }
        public virtual DbSet<wp_glamly_servicesbookings> wp_glamly_servicesbookings { get; set; }
        public virtual DbSet<wp_glamly_stylistschedules> wp_glamly_stylistschedules { get; set; }
    }
}
