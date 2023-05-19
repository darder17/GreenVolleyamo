using Microsoft.AspNetCore.Components.Forms;

namespace GreenVolley.FrontEnd.Data
{
    public class ToBeExported
    {
        public ToBeExported(string teamName, List<Athlete> athletes, string nameResp, string telephoneResp, List<IBrowserFile> browserFiles)
        {
            TeamName = teamName;
            this.athletes = athletes;
            NameResp = nameResp;
            TelephoneResp = telephoneResp;
            this.browserFiles = browserFiles;
        }

        public string TeamName { get; set; }
        public List<Athlete> athletes { get; set; }
        public string NameResp { get; set; }
        public string TelephoneResp { get; set; }
        public List<IBrowserFile> browserFiles { get; set; }
    }
}
