﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShelfQuest.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        [MaxLength(30)]
        public string Author { get; set; }
        [Required]
        [Display(Name = "List Price")]
        [Range(1,1000)]
        public double ListPrice { get; set; }

        //Bulk Price
        [Required]
        [Display(Name = "Price for 1-50")]
        [Range(1,1000)]
        public double Price { get; set; }
        [Required]
        [Display(Name = "Price for 50-100")]
        [Range(1,1000)]
        public double Price50 { get; set; }
        [Required]
        [Display(Name = "Price for 100+")]
        [Range(1,1000)]
        public double Price100 { get; set; }
        
        [Display(Name ="Category Id")]
        public int CategoryId { get; set; }

        [ValidateNever] 
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [ValidateNever]
        public string ImageURL { get; set; }


    }
}
