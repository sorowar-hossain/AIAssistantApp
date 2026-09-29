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
            pages = ExtractText();

            //foreach (PdfPage page in pages)
            //{
            //    Console.WriteLine("------------");
            //    Console.WriteLine($"Page No: {page.PageNumber}");
            //    Console.WriteLine(page.Title);
            //    Console.WriteLine(page.Text);
            //}

            List<DocumentChunk> chunks = chunkService.CreateChunks(
                                    pages,
                                    maxWords: 50,
                                    overlapWords: 10);

            foreach (var chunk in chunks)
            {
                //Console.WriteLine("----------------------------");
                //Console.WriteLine($"Chunk ID: {chunk.Id}");
                //Console.WriteLine($"Page: {chunk.PageNumber}");
                //Console.WriteLine($"Title: {chunk.SectionTitle}");
                //Console.WriteLine($"Text: {chunk.Text}");

                chunk.Embedding = embeddingService.GenerateEmbedding(chunk.Text);
            }

            int a = 0;

        }

        public List<PdfPage> ExtractText()
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
        }
    }
}
