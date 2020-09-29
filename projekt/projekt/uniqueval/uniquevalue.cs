using projekt.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace projekt.uniqueval
{
    public class uniquevalue: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            projektEntities db = new projektEntities();
            var username = Convert.ToString(value);
            var nr = db.AspNetUsers.Where(tmp => tmp.UserName == username).Count();
            if (nr >= 1)
            {
                return new ValidationResult(ErrorMessage = "Usernami eshte i zene !");

            }
            else return ValidationResult.Success;
            
        }
    }
}