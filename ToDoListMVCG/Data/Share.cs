using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoListMVCG.Data
{
    public class Share : BaseEntity
    {
        public int ToDoListId { get; set; }
        public virtual ToDoList ToDoList { get; set; }
        public string UserId { get; set; }
        public virtual AppUser User { get; set; }
    }
}
