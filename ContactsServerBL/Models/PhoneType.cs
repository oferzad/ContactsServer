using System;
using System.Collections.Generic;

#nullable disable

namespace ContactsServerBL.Models
{
    public partial class PhoneType
    {
        public PhoneType()
        {
            ContactPhones = new HashSet<ContactPhone>();
        }

        public int TypeId { get; set; }
        public string TypeName { get; set; }

        public virtual ICollection<ContactPhone> ContactPhones { get; set; }
    }
}
