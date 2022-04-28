using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoListMVCG.Models.ViewModels.ToDoLists
{
    public class ToDoListShareViewModel
    {
        public ToDoListShareViewModel()
        {
            SharedWith = new List<SelectListItem>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string CreatedById { get; set; }
        public string ModifiedById { get; set; }
        public List<SelectListItem> SharedWith { get; set; }
        public string[] SharedWithIds { get; set; }
    }
}
