using AIAssistantApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;

namespace AIAssistantApp.Services
{
    public class PdfTextExtractorService
    {
        List<PdfPage> pages = new List<PdfPage>();
        DocumentChunkService chunkService = new DocumentChunkService();
        RagEmbeddingService embeddingService = new RagEmbeddingService();   

        public PdfTextExtractorService()
        {
          
        }

        public async Task< List<PdfPage>> ExtractText()
        {
            string pdfPath = Path.Combine(
                AppContext.BaseDirectory,
                "Data",
                "RAG_Data.pdf"
            );

            if (!File.Exists(pdfPath))
            {
                Console.WriteLine($"PDF not found: {pdfPath}");
                return pages;
            }

            using var document = PdfDocument.Open(pdfPath);

            foreach (var page in document.GetPages())
            {
                string text = page.Text?.Trim() ?? "";

                string title = GetTitle(page.Number);

                if (!string.IsNullOrEmpty(title))
                {
                    // Remove only the first occurrence
                    if (text.StartsWith(title, StringComparison.OrdinalIgnoreCase))
                    {
                        text = text.Substring(title.Length).Trim();
                    }
                }

                pages.Add(new PdfPage
                {
                    PageNumber = page.Number,
                    Title = title,
                    Text = text
                });
            }

            return pages;
        }

        private string GetTitle(int pageNumber)
        {
            return pageNumber switch
            {
                1 => "Employee Leave Policy",
                2 => "Salary Information",
                3 => "Employee Attendance Policy",
                _ => ""
            };

            //return pageNumber switch
            //{
            //    1 => "OVERVIEW & SCOPE",
            //    2 => "DATA CLASSIFICATION & EMBEDDING RULES",
            //    3 => "VECTOR DATABASE HYGIENE",
            //    4 => "RETRIEVAL & ACCURACY THRESHOLDS",
            //    5 => "COMPLIANCE & AUDITING",
            //    _ => ""
            //};
        }
    }
}
