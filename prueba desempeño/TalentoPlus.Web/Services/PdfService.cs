using System;
using System.IO;
using TalentoPlus.Web.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace TalentoPlus.Web.Services
{
    public class PdfService
    {
        public byte[] GenerarHojaVidaPDF(Employee employee)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    // Create the PDF
                    var pdfWriter = new PdfWriter(memoryStream);
                    var pdfDocument = new PdfDocument(pdfWriter);
                    var document = new Document(pdfDocument);

                    // Title
                    document.Add(new Paragraph("CURRICULUM VITAE")
                        .SetFontSize(20)
                        .SetBold()
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                    document.Add(new Paragraph("TalentoPlus S.A.S")
                        .SetFontSize(10)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                    document.Add(new Paragraph(" "));

                    // Personal Data
                    document.Add(new Paragraph("PERSONAL INFORMATION")
                        .SetFontSize(14)
                        .SetBold());

                    document.Add(new Paragraph($"Name: {employee.FirstName} {employee.LastName}"));
                    document.Add(new Paragraph($"Document: {employee.Document}"));
                    document.Add(new Paragraph($"Birth Date: {employee.BirthDate:yyyy-MM-dd}"));
                    document.Add(new Paragraph($"Email: {employee.Email}"));
                    document.Add(new Paragraph($"Phone: {employee.Phone}"));
                    document.Add(new Paragraph($"Address: {employee.Address}"));

                    document.Add(new Paragraph(" "));

                    // Employment Information
                    document.Add(new Paragraph("EMPLOYMENT INFORMATION")
                        .SetFontSize(14)
                        .SetBold());

                    document.Add(new Paragraph($"Position: {employee.Position}"));
                    document.Add(new Paragraph($"Status: {employee.Status}"));
                    document.Add(new Paragraph($"Department: {employee.Department?.Name}"));
                    document.Add(new Paragraph($"Hire Date: {employee.HireDate:dd/MM/yyyy}"));
                    document.Add(new Paragraph($"Salary: ${employee.Salary:N2}"));

                    document.Add(new Paragraph(" "));

                    // Education
                    document.Add(new Paragraph("EDUCATION LEVEL")
                        .SetFontSize(14)
                        .SetBold());

                    document.Add(new Paragraph($"Level: {employee.EducationLevel}"));

                    document.Add(new Paragraph(" "));

                    // Professional Profile
                    document.Add(new Paragraph("PROFESSIONAL PROFILE")
                        .SetFontSize(14)
                        .SetBold());

                    document.Add(new Paragraph(employee.ProfessionalProfile ?? "Not specified"));

                    document.Add(new Paragraph(" "));
                    document.Add(new Paragraph(" "));

                    // Footer
                    document.Add(new Paragraph($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
                        .SetFontSize(9)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                    document.Close();
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating PDF: {ex.Message}");
                throw;
            }
        }
    }
}

