using AIAssistantApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssistantApp.Services
{
    public class RagHybridSearchService
    {
        private readonly RagVectorSearchService vectorSearchService;

        public RagHybridSearchService(RagVectorSearchService vectorSearchService)
        {
            this.vectorSearchService = vectorSearchService;
        }

        public async Task<List<(DocumentChunk Chunk, double Score)>>SearchHybrid( 
                string question,
                double[] queryEmbedding,
                List<DocumentChunk> chunks,
                int topK = 3)
        {
            // Semantic search
            var semanticResults = await vectorSearchService.Search(
                    queryEmbedding,
                    chunks,
                    chunks.Count);

            var results = new List<(DocumentChunk Chunk, double Score)>();

            foreach (var semanticResult in semanticResults)
            {
                double keywordScore = CalculateKeywordScore(question,semanticResult.Chunk.Text);
                double semanticScore = semanticResult.Score;

                // 30% keyword + 70% semantic
                double finalScore =
                    (keywordScore * 0.30) +
                    (semanticScore * 0.70);

                results.Add((
                    semanticResult.Chunk,
                    finalScore));
            }

            return results
                .OrderByDescending(x => x.Score)
                .Take(topK)
                .ToList();
        }

        private double CalculateKeywordScore(string question, string chunkText)
        {
            var questionWords =
                Tokenize(question);

            var chunkWords =
                Tokenize(chunkText);

            if (questionWords.Count == 0)
                return 0;

            int matchedWords =
                questionWords
                .Count(word => chunkWords.Contains(word));

            return (double)matchedWords /
                   questionWords.Count;
        }

        private List<string> Tokenize(string text)
        {
            return text
                .ToLowerInvariant()
                .Split(
                    new[] { ' ', '.', ',', '?', '!', ':', ';', '(', ')' },
                    StringSplitOptions.RemoveEmptyEntries)
                .ToList();
        }
    }
}
