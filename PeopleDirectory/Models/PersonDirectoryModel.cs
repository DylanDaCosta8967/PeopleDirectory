using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace PeopleDirectory.Models
{
    public class PersonDirectoryModel
    {
        [Key]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public String Name { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public String SurName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("Mobile Number")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public String Gender { get; set; }

        [DisplayName("Email Address")]
        [Required(ErrorMessage = "This field is required.")]
        public String EmailAddress { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("Profile Picture")]
        public byte[] ProfilePic { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public int ClientCityId { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public int ClientCountryId { get; set; }
        

    }

    public class Country
    {
        [Key]
        public int CountryId { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("Country")]
        public String CountryName { get; set; }

    }

    public class City
    {
        [Key]
        public int CityId { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("City")]
        public String CityName { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }

    }

    public class Users
    {
        [Key]
        public int UserId { get; set; }
        public String UserName { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }

}