using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebChatSwitch.Web.Models
{
    public class ItemViewModel
    {
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Expectation")]
        public string Expectation { get; set; }

        [Display(Name = "Available")]
        public bool Available { get; set; }
    }
}