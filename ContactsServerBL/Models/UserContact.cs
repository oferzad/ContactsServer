using System;
using System.Collections.Generic;

#nullable disable

namespace ContactsServerBL.Models
{
    public partial class UserContact
    {
        public UserContact()
        {
            ContactPhones = new HashSet<ContactPhone>();
        }

        public int UserId { get; set; }
        public int ContactId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<ContactPhone> ContactPhones { get; set; }
    }
}
