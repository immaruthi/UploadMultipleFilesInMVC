using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace UploadMultipleFilesInMVC.Models
{
    public class Context: DbContext
    {
        public Context() : base("name=Context")
        {
        }

        public DbSet<DocUserDetails> docUserDetails { get; set; }
        public DbSet<ExtractHandwrittenText> extractHandwrittenText { get; set; }
        public DbSet<Text> text { get; set; }
        public DbSet<TextDocumentType> textDocumentType { get; set; }
        public DbSet<TextLocation> textLocation { get; set; }
    }
}