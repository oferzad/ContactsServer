using System;
using System.Collections.Generic;

#nullable disable

namespace ContactsServerBL.Models
{
    public partial class User
    {
        public User()
        {
            UserContacts = new HashSet<UserContact>();
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserPswd { get; set; }

        public virtual ICollection<UserContact> UserContacts { get; set; }
    }
}
