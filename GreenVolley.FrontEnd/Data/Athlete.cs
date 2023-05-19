namespace GreenVolley.FrontEnd.Data
{
    public class Athlete
    {
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public string DataDiNascita { get; set; }
        public string TagliaMaglia { get; set; }

        public Athlete()
        {
            Cognome = string.Empty;
            Nome = string.Empty;
            DataDiNascita = string.Empty;
            TagliaMaglia = string.Empty;
        }
    }
}
