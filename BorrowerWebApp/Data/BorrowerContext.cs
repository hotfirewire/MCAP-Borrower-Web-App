using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BorrowerWebApp.Models
{
    public class BorrowerContext : DbContext
    {
        public BorrowerContext (DbContextOptions<BorrowerContext> options)
            : base(options)
        {
        }

        public DbSet<BorrowerWebApp.Models.Borrower> Borrower { get; set; }
    }
}
