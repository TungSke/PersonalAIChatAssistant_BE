using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PersonalAIAssistant.Application.DTOs.Request
{
    public class RegisterWithPhoneNumberRequest
    {
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [Required(ErrorMessage = "Phone number is required.")]
        public string PhoneNumber { get; set; }
    }
}
