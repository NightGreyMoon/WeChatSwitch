using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WeChatSwitch.Web.Models
{
    public class ItemViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Expectation")]
        public string Expectation { get; set; }

        [Display(Name = "Available")]
        public bool Available { get; set; }

        [Display(Name = "Photos")]
        public List<string> ItemPhotos { get; set; }

        [Display(Name = "PublishUser")]
        public string PublishUser { get; set; }

        [Display(Name = "PublishUserOpenId")]
        public string PublishUserOpenId { get; set; }
    }
}