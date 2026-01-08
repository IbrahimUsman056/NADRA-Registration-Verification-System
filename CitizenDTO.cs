namespace NADRASystem.DTOs
{
    public class CitizenDTO
    {
        public int CitizenId { get; set; }
        public string FullName { get; set; }
        public string CNIC { get; set; }
        public string FatherName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string MaritalStatus { get; set; }
        public string Nationality { get; set; }
        public bool IsAlive { get; set; }
    }
}