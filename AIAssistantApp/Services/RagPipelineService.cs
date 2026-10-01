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
        /*
         For your RAG project, neither is universally better. They solve different problems.

        | Approach                  | How it works                | Good at                                | Weakness                                    |
        | ------------------------- | --------------------------- | -------------------------------------- | ------------------------------------------- |
        | **Word/keyword matching** | Looks for matching words    | Exact terms, names, IDs, numbers       | Misses similar meaning with different words |
        | **Semantic matching**     | Compares embeddings/meaning | Different wording with similar meaning | Can return surprising matches               |
        | **Hybrid search**         | Combines both               | Exact terms + meaning                  | More complex                                |

        So how do we solve your problem?
        For your RAG project, I recommend we stop here and improve retrieval before moving to the LLM.

        We'll build:
        User Question
              
                ↓
     ┌───────────────────────┐
     │                       │
     ↓                       ↓
    Keyword Search      Semantic Search
     │                       │
     ↓                       ↓
    Keyword Score       Semantic Score
     │                       │
     └───────────┬───────────┘
                 ↓
           Combined Score
                 ↓
              Top-K

         */

        List<PdfPage> pages = new List<PdfPage>();
       private readonly RagHybridSearchService ragHybridSearchService;
        private readonly PdfTextExtractorService pdfTextExtractorService;
        private readonly DocumentChunkService documentChunkService;
        private readonly RagEmbeddingService ragEmbeddingService;
        private readonly RagVectorSearchService ragVectorSearchService;

        public RagPipelineService(
           PdfTextExtractorService pdfTextExtractorService,
           DocumentChunkService documentChunkService,
           RagEmbeddingService ragEmbeddingService,
           RagVectorSearchService ragVectorSearchService,
           RagHybridSearchService ragHybridSearchService
        )
        {
            this.pdfTextExtractorService = pdfTextExtractorService;
            this.documentChunkService = documentChunkService;
            this.ragEmbeddingService = ragEmbeddingService;
            this.ragVectorSearchService = ragVectorSearchService;
            this.ragHybridSearchService = ragHybridSearchService;
        }

        public async Task<List<(DocumentChunk Chunk, double Score)>> RagSearch(string question)
        {
            pages = await pdfTextExtractorService.ExtractText();
            //foreach (PdfPage page in pages)
            //{
            //    Console.WriteLine("------------");
            //    Console.WriteLine($"Page No: {page.PageNumber}");
            //    Console.WriteLine(page.Title);
            //    Console.WriteLine(page.Text);
            //    Console.WriteLine("------------");
            //}

            var chunks = await documentChunkService.CreateChunks(pages);

            foreach (var chunk in chunks)
            {
                Console.WriteLine("----------------------------");
                Console.WriteLine($"Chunk ID: {chunk.Id}");
                Console.WriteLine($"Page: {chunk.PageNumber}");
                Console.WriteLine($"Title: {chunk.SectionTitle}");
                Console.WriteLine($"Text: {chunk.Text}");
                Console.WriteLine("----------------------------");

                chunk.Embedding = await ragEmbeddingService.GenerateEmbedding(chunk.Text);
            }

            Console.WriteLine($"Question: {question}");
            double[] queryEmbedding = await ragEmbeddingService.GenerateEmbedding(question);

            var responsechunks = await ragHybridSearchService.SearchHybrid(question, queryEmbedding, chunks,3);
            //var responsechunks =await ragVectorSearchService.Search(queryEmbedding, chunks, 3);

            //foreach (var result in responsechunks)
            //{
            //    Console.WriteLine("--------------------------------------");
            //    Console.WriteLine($"Chunk ID: {result.Chunk.Id}");
            //    Console.WriteLine($"Page: {result.Chunk.PageNumber}");
            //    Console.WriteLine($"Title: {result.Chunk.SectionTitle}");
            //    Console.WriteLine($"Score: {result.Score:F4}");
            //    Console.WriteLine($"Text: {result.Chunk.Text}");
            //    Console.WriteLine("--------------------------------------");
            //}

            var combineResponse=BuildContext(responsechunks);
            Console.WriteLine(combineResponse);

            return responsechunks;
        }

        public string BuildContext(List<(DocumentChunk Chunk, double Score)> results)
        {
            if (results == null || results.Count == 0)
                return string.Empty;

            var context = new StringBuilder();

            foreach (var result in results)
            {
                context.AppendLine("--------------------------------------");
                context.AppendLine($"Source: Page {result.Chunk.PageNumber}");
                context.AppendLine($"Section: {result.Chunk.SectionTitle}");
                context.AppendLine(result.Chunk.Text);
                context.AppendLine();
                context.AppendLine("--------------------------------------");
                context.AppendLine();
            }

            return context.ToString();
        }
    }
}
