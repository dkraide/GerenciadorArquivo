using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication.Models;

namespace Communication.Mapping
{
    public class FileMap : IEntityTypeConfiguration<MFile>
    {
        public void Configure(EntityTypeBuilder<MFile> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.User).WithMany(x => x.Files);
            builder.ToTable("File");
        }
    }
}