using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.AspNetCore.Components.Forms;
using iText.IO.Image;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Geom;

namespace GreenVolley.FrontEnd.Data
{
    public class ExportToPdf
    {

        public static async Task CreatePdfAsync(ToBeExported data)
        {
            List <byte[]> pdfs = new List<byte[]>();

            var memoryStream = new MemoryStream();

            using (var writer = new PdfWriter(memoryStream))
            using (var pdfDocument = new PdfDocument(writer))
            using (var document = new Document(pdfDocument))
            {
                // Aggiunta del contenuto al documento
                document.Add(new Paragraph("Nome Squadra: " + data.TeamName));
                document.Add(new Paragraph("Nome Responsabile: " + data.NameResp));
                document.Add(new Paragraph("Telefono Responsabile: " + data.TelephoneResp));

                // Creazione della tabella degli atleti
                if (data.athletes != null)
                {
                    var table = new Table(4);

                    // Aggiunta riga di intestazione
                    table.AddHeaderCell("Cognome");
                    table.AddHeaderCell("Nome");
                    table.AddHeaderCell("Data di Nascita");
                    table.AddHeaderCell("Taglia Maglia");

                    // Aggiunta righe degli atleti
                    foreach (var athlete in data.athletes)
                    {
                        table.AddCell(athlete.Cognome);
                        table.AddCell(athlete.Nome);
                        table.AddCell(athlete.DataDiNascita);
                        table.AddCell(athlete.TagliaMaglia);
                    }

                    document.Add(table);
                }

                // Aggiunta dei file PDF come pagine aggiuntive
                if (data.browserFiles != null && data.browserFiles.Any())
                {
                    foreach (var file in data.browserFiles)
                    {
                        var fileExtension = System.IO.Path.GetExtension(file.Name);

                        if (fileExtension == ".pdf")
                        {
                            var attachmentData = await GetFileDataAsync(file);
                            AddPdfPages(pdfDocument, attachmentData);
                        }
                        else if (IsImageFile(fileExtension))
                        {
                            var imageData = await GetFileDataAsync(file);
                            AddImagePage(pdfDocument, imageData);
                        }
                    }
                }
            }

            pdfs.Add(memoryStream.ToArray());

            SendEmailWithAttachment("volleyamonfl@gmail.com", "Registrazione Squadra", "Vedi allegati", pdfs, data.TeamName);
        }

        public static void SendEmailWithAttachment(string recipientEmail, string subject, string body, List<byte[]> pdfData, string filename)
        {
            // Creazione del messaggio email
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Leonardo Pecol", "leonardo.pecol@gmail.com"));
            message.To.Add(new MailboxAddress("leonardo", recipientEmail));
            message.Subject = subject;

            // Creazione del corpo del messaggio
            var bodyBuilder = new BodyBuilder
            {
                TextBody = body
            };

            // Aggiunta dell'allegato PDF
            foreach(var pdf in pdfData)
            {
                bodyBuilder.Attachments.Add(filename, pdf, ContentType.Parse("application/pdf"));
            }

            // Impostazione del corpo del messaggio
            message.Body = bodyBuilder.ToMessageBody();

            // Invio del messaggio email utilizzando SMTP
            using (var client = new SmtpClient())
            {
                // Configurazione delle impostazioni del client SMTP
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate("leonardo.pecol@gmail.com", "pvgoqomsyzwqewtk");

                // Invio del messaggio
                client.Send(message);
                client.Disconnect(true);
            }
        }

        private static bool IsImageFile(string fileExtension)
        {
            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            return imageExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }

        private async static Task<byte[]> GetFileDataAsync(IBrowserFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.OpenReadStream().CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
        private static void AddImagePage(PdfDocument pdfDocument, byte[] imageData)
        {
            var newPage = pdfDocument.AddNewPage();

            using (var memoryStream = new MemoryStream(imageData))
            {
                var image = new Image(ImageDataFactory.Create(imageData));
                image.ScaleToFit(newPage.GetPageSize().GetWidth(), newPage.GetPageSize().GetHeight());

                var canvas = new PdfCanvas(newPage);
                var rect = new Rectangle(0, 0, newPage.GetPageSize().GetWidth(), newPage.GetPageSize().GetHeight());

                var xObj = new Canvas(canvas, rect, false);
                xObj.Add(image);
                xObj.Flush();
            }
        }

        private static void AddPdfPages(PdfDocument document, byte[] pdfData)
        {
            using (var sourceDocument = new PdfDocument(new PdfReader(new MemoryStream(pdfData))))
            {
                for (int i = 1; i <= sourceDocument.GetNumberOfPages(); i++)
                {
                    document.AddPage(sourceDocument.GetPage(i).CopyTo(document));
                }
            }
        }
    }
}
