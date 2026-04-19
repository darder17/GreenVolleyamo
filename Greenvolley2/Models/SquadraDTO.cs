namespace Greenvolley2.Models
{
    public class SquadraDTO
    {
        public string NomeSquadra { get; set; } = "";
        public string NomeResponsabile { get; set; } = "";
        public string TelefonoReponsabile { get; set; } = "";
        public List<AtletaDTO> Atleti { get; set; } = [];

        public List<IFormFile> Allegati { get; set; } = [];
    }
}
