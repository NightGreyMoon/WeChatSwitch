using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebChatSwitch.Web.Models;

namespace WebChatSwitch.Web.Controllers
{
    public class ItemController : Controller
    {
        // GET: Item
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            ItemViewModel vm = new ItemViewModel();
            vm.Available = true;
            vm.ItemPhotos = new List<string>();
            vm.ItemPhotos.Add(string.Empty);
            vm.ItemPhotos.Add(string.Empty);
            vm.ItemPhotos.Add(string.Empty);
            return View(vm);
        }
    }
}