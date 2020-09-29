using projekt.uniqueval;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace projekt.ViewModels
{
    public class RegjistrimViewModel
    {
        [Required(ErrorMessage = "Emri sduhet te jete bosh  !")]
        [MaxLength(10,ErrorMessage ="Emri nuk duhet te jete me i gjate se 10 karaktere!")]
       
        public string emri { get; set; }
        [Required(ErrorMessage = "Fusha usernami sduhet te jete bosh !")]
        [uniquevalue]
        public string username { get; set; }
        [Required(ErrorMessage = "Fusha password sduhet te jete bosh !")]
        [DataType(DataType.Password)]
        public string password { get; set; }
        [Required(ErrorMessage = "Fusha konfirmim pasuord sduhet te jete bosh!")]
        [Compare("password", ErrorMessage = "Fusha pasuord nuk perputhet me fushen confirmpassword! ")]
        [DataType(DataType.Password)]
        public string repasuord { get; set; }
        [Required(ErrorMessage = "Fusha e emailit sduhet te jete bosh !")]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }
        [Required(ErrorMessage = "Fusha e gjinise sduhet te jete bosh !")]
        public string gjinia { get; set; }

        public string imgurl { get; set; }
    }
}