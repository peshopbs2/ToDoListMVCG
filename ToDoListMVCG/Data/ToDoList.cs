using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoListMVCG.Data
{
    public class ToDoList : BaseEntity
    {
        public ToDoList()
        {
            SharedWith = new List<Share>();
        }
        public string Title { get; set; }
        public virtual List<Share> SharedWith { get; set; }
    }
}
