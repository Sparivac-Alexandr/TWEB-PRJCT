using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taskoreDomain.Enteties.User;

namespace taskoreBusinessLogic.DBModel.Seed
{
   public class UserContext : DbContext
    {
        public UserContext () :
            base ("name=taskore")
        {   
        }

        public virtual DbSet<UDbTable> Users { get; set; }
    }
}
