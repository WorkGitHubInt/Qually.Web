using System;

namespace QuallyLib
{
    public class Counter
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string IP { get; set; }
        public int ProgramId { get; set; }
        public Program Program { get; set; }
        public DateTime Date { get; set; }
    }
}
