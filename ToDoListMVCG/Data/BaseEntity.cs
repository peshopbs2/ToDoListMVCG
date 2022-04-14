using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoListMVCG.Data
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [ScaffoldColumn(false)]
        public DateTime CreatedAt { get; set; }
        [ScaffoldColumn(false)]
        public DateTime ModifiedAt { get; set; }
        [ScaffoldColumn(false)]
        public string CreatedById { get; set; }
        [ScaffoldColumn(false)]
        public virtual AppUser CreatedBy { get; set; }
        [ScaffoldColumn(false)]
        public string ModifiedById { get; set; }
        public virtual AppUser ModifiedBy { get; set; }
    }
}
