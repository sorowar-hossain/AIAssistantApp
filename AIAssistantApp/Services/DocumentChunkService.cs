using AIAssistantApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AIAssistantApp.Services
{
    public class DocumentChunkService
    {
        /*
        ✅ Maximum chunk size is around 50 words
        ✅ Chunks contain complete sentences
        ✅ Chunks have overlap
        ✅ the overlap is a complete sentence, not exactly 10 words.
        ✅ PageNumber is preserved
        ✅ Title / SectionTitle is preserved
        ✅ Each chunk has a unique Id
        ✅ The chunks are small enough for semantic retrieval
         
         */
        public async Task<List<DocumentChunk>> CreateChunks(
        List<PdfPage> pages,
        int maxWords = 50,
        int overlapWords = 10)
        {
            var chunks = new List<DocumentChunk>();
            int chunkId = 1;

            foreach (var page in pages)
            {
                // Split page text into sentences
                var sentences = Regex
                    .Split(page.Text, @"(?<=[.!?])(?=\s|[A-Z])")
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList();

                int start = 0;

                while (start < sentences.Count)
                {
                    var chunkSentences = new List<string>();
                    int wordCount = 0;
                    int end = start;

                    // Add complete sentences until we reach maxWords
                    while (end < sentences.Count)
                    {
                        var sentenceWords = sentences[end]
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        if (chunkSentences.Count > 0 &&
                            wordCount + sentenceWords.Length > maxWords)
                        {
                            break;
                        }

                        chunkSentences.Add(sentences[end]);
                        wordCount += sentenceWords.Length;
                        end++;
                    }

                    string chunkText = string.Join(" ", chunkSentences);

                    chunks.Add(new DocumentChunk
                    {
                        Id = chunkId++,
                        PageNumber = page.PageNumber,
                        SectionTitle = page.Title,
                        Text = chunkText
                    });

                    // Find starting point for next chunk
                    if (end >= sentences.Count)
                        break;

                    // Calculate overlap based on words
                    var overlapText = chunkText
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .TakeLast(overlapWords)
                        .ToList();

                    // Find the sentence containing the overlap words
                    int overlapStart = end - 1;

                    while (overlapStart > start)
                    {
                        var words = sentences[overlapStart]
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        if (words.Length >= overlapWords)
                            break;

                        overlapStart--;
                    }

                    start = overlapStart;
                }
            }

            return chunks;
        }
    }
}
