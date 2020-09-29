using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace projekt.Identity
{
    public class ApplicationUser :IdentityUser
    {

        public string emri { get; set; }
        public string gjinia { get; set; }
        public string imgurl { get; set; }
    }
}