namespace PrzykladKolokwium.Models
{
    public class Information
    {
        bool done { get; set; }
        string info { get; set; }

        public Information(bool done, string info)
        {
            this.done = done;
            this.info = info;
        }
    }
}
