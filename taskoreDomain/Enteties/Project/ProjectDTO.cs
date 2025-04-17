using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace taskoreDomain.Enteties.Project
{
    public class ProjectDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Budget { get; set; }
        public string Deadline { get; set; }
        public string RequiredSkills { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 