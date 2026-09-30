using AIAssistantApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssistantApp.Services
{
    public class RagPipelineService
    {
        List<PdfPage> pages = new List<PdfPage>();
        List<DocumentChunk> responsechunks= new List<DocumentChunk>();  

        private readonly PdfTextExtractorService pdfTextExtractorService;
        private readonly DocumentChunkService documentChunkService;
        private readonly RagEmbeddingService ragEmbeddingService;
        private readonly RagVectorSearchService ragVectorSearchService;

        public RagPipelineService(
           PdfTextExtractorService pdfTextExtractorService,
           DocumentChunkService documentChunkService,
           RagEmbeddingService ragEmbeddingService,
           RagVectorSearchService ragVectorSearchService
        )
        {
            this.pdfTextExtractorService = pdfTextExtractorService;
            this.documentChunkService = documentChunkService;
            this.ragEmbeddingService = ragEmbeddingService;
            this.ragVectorSearchService = ragVectorSearchService;
        }

        public async Task< List<(DocumentChunk Chunk, double Score)>> RagSearch(string question)
        {
            pages = await pdfTextExtractorService.ExtractText();
            foreach (PdfPage page in pages)
            {
                Console.WriteLine("------------");
                Console.WriteLine($"Page No: {page.PageNumber}");
                Console.WriteLine(page.Title);
                Console.WriteLine(page.Text);
            }

            var chunks = await documentChunkService.CreateChunks(pages);

            foreach (var chunk in chunks)
            {
                //Console.WriteLine("----------------------------");
                //Console.WriteLine($"Chunk ID: {chunk.Id}");
                //Console.WriteLine($"Page: {chunk.PageNumber}");
                //Console.WriteLine($"Title: {chunk.SectionTitle}");
                //Console.WriteLine($"Text: {chunk.Text}");

                chunk.Embedding = await ragEmbeddingService.GenerateEmbedding(chunk.Text);
            }


            double[] queryEmbedding = await ragEmbeddingService.GenerateEmbedding(question);

            //responsechunks = ragVectorSearchService.Search(queryEmbedding, chunks, 3);

            return null;
        }
    }
}
