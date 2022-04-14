using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoListMVCG.Data
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedtAt { get; set; }
        public string CreatedById { get; set; }
        public virtual AppUser CreatedBy { get; set; }
        public string ModifiedById { get; set; }
        public virtual AppUser ModifiedBy { get; set; }
    }
}
