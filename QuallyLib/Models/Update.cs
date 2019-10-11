namespace QuallyLib
{
    public class Update
    {
        public int Id { get; set; }
        public int ProgramId { get; set; }
        public Program Program { get; set; }
        public string Version { get; set; }
        public byte[] Zip { get; set; }
    }
}
