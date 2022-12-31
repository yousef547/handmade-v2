using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandmadeStore.Models.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string ReviewText { get; set; }
        public DateTime? ReviewDate { get; set; }
        public int ProductId { get; set; }
        [ValidateNever]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
