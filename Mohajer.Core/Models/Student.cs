using System.Collections.Generic;
using System.Net;

namespace Mohajer.Core.Models
{
    public class Student
    {
        public int UserName { get; set; }
        public string FullName { get; set; }
        public string StudentCode { get; set; }
        public string Password { get; set; }
        public List<Food> Foods { get; set; } = new List<Food>();
    }
}