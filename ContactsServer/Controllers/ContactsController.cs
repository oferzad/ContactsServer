using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
        
        //set the contact default photo image name
        public const string DEFAULT_PHOTO = "defaultphoto.jpg";

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

            User user = HttpContext.Session.GetObject<User>("theUser");
            //Check if user logged in and its ID is the same as the contact user ID
            if (user != null && user.Id == contact.UserId)
            {
                
                //update contact to the DB by marking all entities that should be modified or added
                if (contact.ContactId > 0)
                {
                    context.Entry(contact).State = EntityState.Modified;
                }
                else
                {
                    context.Entry(contact).State = EntityState.Added;
                }

                foreach (ContactPhone cp in contact.ContactPhones)
                {
                        if (cp.PhoneId > 0)
                        {
                            context.Entry(cp).State = EntityState.Modified;
                        }
                        else
                        {
                            context.Entry(cp).State = EntityState.Added;
                        }
                }
                //Save change into the db
                context.SaveChanges();


                //Now check if an image exist for the contact (photo). If not, set the default image!
                var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", DEFAULT_PHOTO);
                var targetPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", $"{contact.ContactId}.jpg");
                System.IO.File.Copy(sourcePath, targetPath);

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
                    context.Entry(c).State = EntityState.Deleted;
                }
                //now remove the contact it self
                context.Entry(contact).State = EntityState.Deleted;
                context.SaveChanges();

                //now delete the photo image of the contact 
                var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", $"{contact.ContactId}.jpg");
                System.IO.File.Delete(sourcePath);
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
            if (phone == null || phone.Contact == null)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                return;
            }
            
            User user = HttpContext.Session.GetObject<User>("theUser");
            //Check if user logged in and its ID is the same as the contact user ID
            if (user != null && user.Id == phone.Contact.UserId)
            {
                //remove the phone
                context.Entry(phone).State = EntityState.Deleted;
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
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                        
                    
                    return Ok(new { length = file.Length, name = file.FileName });
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return BadRequest();
                }
            }
            return Forbid();
        }
    }
}
