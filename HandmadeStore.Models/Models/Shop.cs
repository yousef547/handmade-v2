using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandmadeStore.Models.Models
{
    public class Shop
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string City { get; set; }
        public string StreetAdress { get; set; }
        public string PostalCode { get; set; }
        public string Number { get; set; }
    }
}
