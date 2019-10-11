using System;

namespace QuallyLib
{
    public class Review
    {
        public int Id { get; set; }
        public string UserID { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
        public bool Approved { get; set; }
    }
}
