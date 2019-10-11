namespace QuallyLib
{
    public class Message
    {
        public int Id { get; set; }
        public int ProgramId { get; set; }
        public Program Program { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Body { get; set; }
        public string Version { get; set; }
        public string LogError { get; set; }
        public string LogAcc { get; set; }
        public string Log { get; set; }
        public string LogMain { get; set; }
        public string Contacts { get; set; }
    }
}
