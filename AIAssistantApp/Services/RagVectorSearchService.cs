using AIAssistantApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssistantApp.Services
{
    public class RagVectorSearchService
    {
        public List<(DocumentChunk Chunk, double Score)> Search(
            double[] queryEmbedding,
            List<DocumentChunk> chunks,
            int topK = 3)
        {
            return chunks
                .Where(x => x.Embedding != null &&
                            x.Embedding.Length > 0)
                .Select(x => (
                    Chunk: x,
                    Score: CalculateCosineSimilarity(
                        queryEmbedding,
                        x.Embedding)))
                .OrderByDescending(x => x.Score)
                .Take(topK)
                .ToList();
        }

        private double CalculateCosineSimilarity(
            double[] vectorA,
            double[] vectorB)
        {
            if (vectorA.Length != vectorB.Length)
                throw new ArgumentException(
                    "Vectors must have the same length.");

            double dotProduct = 0;
            double magnitudeA = 0;
            double magnitudeB = 0;

            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];

                magnitudeA += vectorA[i] * vectorA[i];

                magnitudeB += vectorB[i] * vectorB[i];
            }

            if (magnitudeA == 0 || magnitudeB == 0)
                return 0;

            return dotProduct /
                   (Math.Sqrt(magnitudeA) *
                    Math.Sqrt(magnitudeB));
        }
    }
}
