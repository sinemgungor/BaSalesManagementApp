﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Dtos.AdminDTOs
{
    public class AdminUpdateDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public byte[]? PhotoData { get; set; }
        public bool IsPhotoRemoved { get; set; }  // Fotoğrafı kaldırma isteği
    }
}
