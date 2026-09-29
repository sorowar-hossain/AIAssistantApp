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

        public List<(DocumentChunk Chunk, double Score)> RagSearch(string question)
        {
            // 1. Generate question embedding
            // 2. Search similar chunks
            // 3. Return top results


            double[] queryEmbedding=ragEmbeddingService.GenerateEmbedding(question);
            //var result=ragVectorSearchService.Search(queryEmbedding,)

            return null;
        }
    }
}
