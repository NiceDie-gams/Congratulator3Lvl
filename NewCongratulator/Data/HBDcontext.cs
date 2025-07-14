using Microsoft.EntityFrameworkCore;
using NewCongratulator.Models;

namespace NewCongratulator.Data
{
    public class HBDcontext : DbContext
    {
        public HBDcontext(DbContextOptions<HBDcontext> options) : base(options) { }
        public DbSet<HumanBirthdayData> HBDdatas { get; set; }

        
    }
}
