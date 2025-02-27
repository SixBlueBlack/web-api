﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers
{
    public class CreateDto
    {
        [Required]
        public string Login { get; set; }

        [DefaultValue("John")]
        public string FirstName { get; set; }

        [DefaultValue("Doe")]
        public string LastName { get; set; }
    }
}