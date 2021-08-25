using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//Add the below
using ContactsServerBL.Models;
using System.IO;

namespace ContactsServer.Controllers
{
    [Route("contactsAPI")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        #region Add connection to the db context using dependency injection
        ContactsDBContext context;
        public ContactsController(ContactsDBContext context)
        {
            this.context = context;
        }
        #endregion

        [Route("Login")]
        [HttpGet]
        public User Login([FromQuery] string email, [FromQuery] string pass)
        {
            User user = context.Login(email, pass);

            //Check user name and password
            if (user != null)
            {
                HttpContext.Session.SetObject("theUser", user);

                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                //Important! Due to the Lazy Loading, the user will be returned with all of its contects!!
                return user;
            }
            else
            {

                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return null;
            }
        }

        [Route("UpdateContact")]
        [HttpPost]
        public UserContact UpdateContact([FromBody] UserContact contact)
        {
            //If contact is null the request is bad
            if (contact == null)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                return null;
            }

            //foreach (ContactPhone cp in contact.ContactPhones)
            //{
            //    cp.PhoneType = null;
            //}

            User user = HttpContext.Session.GetObject<User>("theUser");
            //Check if user logged in and its ID is the same as the contact user ID
            if (user != null && user.Id == contact.UserId)
            {
                //update or add contact to the DB
                context.UserContacts.Update(contact);
                context.SaveChanges();
                //return the contact with its new ID if that was a new contact
                return contact;
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return null;
            }
        }

        [Route("GetPhoneTypes")]
        [HttpGet]
        public List<PhoneType> GetPhoneTypes()
        {
            //Disable lazy loading so not all of the DB will be read!
            this.context.ChangeTracker.LazyLoadingEnabled = false;
            return context.PhoneTypes.ToList();
        }


        [Route("RemoveContact")]
        [HttpPost]
        public void RemoveContact([FromBody] UserContact contact)
        {
            //If contact is null the request is bad
            if (contact == null)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                return;
            }
            User user = HttpContext.Session.GetObject<User>("theUser");
            //Check if user logged in and its ID is the same as the contact user ID
            if (user != null && user.Id == contact.UserId)
            {
                //First remove all contact phones
                foreach (ContactPhone c in contact.ContactPhones)
                {
                    context.ContactPhones.Remove(c);
                }
                //now remove the contact it self
                context.UserContacts.Remove(contact);
                context.SaveChanges();
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return;
            }
        }

        [Route("RemoveContactPhone")]
        [HttpPost]
        public void RemoveContactPhone([FromBody] ContactPhone phone)
        {
            //If phone is null the request is bad
            if (phone == null)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                return;
            }

            //Check if phone exist in DB and who is the user who owns it
            UserContact contact = context.UserContacts.Where(c => c.ContactId == phone.ContactId).FirstOrDefault();

            if (contact == null)
                return;

            User user = HttpContext.Session.GetObject<User>("theUser");
            //Check if user logged in and its ID is the same as the contact user ID
            if (user != null && user.Id == contact.UserId)
            {
                //Remove the tracking over the entities so the remove will work
                context.ChangeTracker.Clear();
                //remove the phone
                context.ContactPhones.Remove(phone);
                context.SaveChanges();
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return;
            }
        }

        [Route("UploadImage")]
        [HttpPost]
        
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            User user = HttpContext.Session.GetObject<User>("theUser");
            //Check if user logged in and its ID is the same as the contact user ID
            if (user != null)
            {
                if (file == null)
                {
                    return BadRequest();
                }

                try
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", file.FileName);
                    var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                    return Ok(new { length = file.Length, name = file.FileName });
                }
                catch
                {
                    return BadRequest();
                }
            }
            return Forbid();
        }
    }
}
