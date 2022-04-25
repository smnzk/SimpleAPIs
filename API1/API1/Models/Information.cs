namespace APBD5.Models
{
    public class Information
    {
        public string info { get; set; }
        public bool done { get; set; }

        public Information(string info, bool done)
        {
            this.info = info;
            this.done = done;   
        }
    }
}
