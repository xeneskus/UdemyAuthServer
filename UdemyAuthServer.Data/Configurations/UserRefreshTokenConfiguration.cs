﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UdemyAuthServer.Core.Models;

namespace UdemyAuthServer.Data.Configurations
{
    public class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
    {
        public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
        {
            builder.HasKey(x => x.UserId);
            builder.Property(x=>x.Code).IsRequired().HasMaxLength(200);

        }
    }
}
