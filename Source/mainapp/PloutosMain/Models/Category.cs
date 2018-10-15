using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PloutosMain.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CategoryType CategoryType { get; set; }
        //figure out 
    }
    public enum CategoryType { Income, Expense, Asset }
}