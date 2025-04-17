using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace taskore.Models
{
    public class ProjectModel
    {
        [Required(ErrorMessage = "Project title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Project description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Budget is required")]
        public string Budget { get; set; }

        [Required(ErrorMessage = "Deadline is required")]
        public string Deadline { get; set; }

        [Required(ErrorMessage = "Required skills are required")]
        public string RequiredSkills { get; set; }
    }
} 