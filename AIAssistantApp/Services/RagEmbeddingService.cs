using AIAssistantApp.Models;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tokenizers.HuggingFace.Tokenizer;
using UglyToad.PdfPig.Tokenization;

namespace AIAssistantApp.Services
{
    public class RagEmbeddingService :  IDisposable
    {
        // Attension mask, Tensor, Mean pooling
        /*
         1. Attention Mask

        Purpose: Tells the model which tokens are real and which tokens should be ignored.

        Example:

        Tokens:          [Employees, can, apply, <PAD>, <PAD>]
        Attention Mask:  [1,         1,   1,     0,     0]
        1 → real token, pay attention
        0 → padding token, ignore

        In your embedding code, the attention mask is also used during mean pooling so padding tokens don't affect the final embedding.

        2. Tensor

        A tensor is a container for numerical data arranged in dimensions.

        For your MiniLM model:

        Input IDs
        Shape: [1, sequenceLength]

        The model output:

        last_hidden_state
        Shape: [1, sequenceLength, 384]

        Meaning:

        1            → one text/input
        sequenceLength → number of tokens
        384          → embedding values for each token

        So you can think of a tensor as a multi-dimensional array of numbers.

        3. Mean Pooling

        MiniLM produces an embedding for each token.

        For example:

        Token 1 → [0.2, 0.4, 0.1]
        Token 2 → [0.3, 0.5, 0.2]
        Token 3 → [0.1, 0.3, 0.4]

        Mean pooling calculates the average across the valid tokens:

        Sentence Embedding
        = (Token1 + Token2 + Token3) / 3

        In your code, the attention mask ensures that only real tokens participate in this average.

        So the complete concept is:

        Text
         ↓
        Tokens
         ↓
        MiniLM
         ↓
        Token embeddings
         ↓
        Attention mask
         ↓
        Mean Pooling
         ↓
        384-dimensional sentence/chunk embedding

        One-line memory tip:

        Attention Mask = what to consider, Tensor = how numbers are organized, 
        Mean Pooling = how token vectors become one text vector.
         */

        // Mean Pooling example

        /*
         Suppose the model produces 3-dimensional vectors for 4 tokens:

            Token 1 → [0.2, 0.4, 0.6]
            Token 2 → [0.4, 0.2, 0.8]
            Token 3 → [0.6, 0.8, 0.2]
            Token 4 → [0.1, 0.3, 0.5]

            Assume all 4 tokens are real:

            Attention Mask → [1, 1, 1, 1]
            Step 1: Add each dimension

            First dimension:

            0.2 + 0.4 + 0.6 + 0.1 = 1.3

            Second dimension:

            0.4 + 0.2 + 0.8 + 0.3 = 1.7

            Third dimension:

            0.6 + 0.8 + 0.2 + 0.5 = 2.1
            Step 2: Divide by number of valid tokens

            There are 4 valid tokens:

            1.3 / 4 = 0.325
            1.7 / 4 = 0.425
            2.1 / 4 = 0.525

            So the final sentence embedding is:

            [0.325, 0.425, 0.525]
            What if there is padding?

            Suppose:

            Token 1 → [0.2, 0.4, 0.6]
            Token 2 → [0.4, 0.2, 0.8]
            Token 3 → [0.6, 0.8, 0.2]
            Token 4 → [0.1, 0.3, 0.5]  ← padding

            Attention mask:

            [1, 1, 1, 0]

            Token 4 is ignored.

            Therefore:

            First  = (0.2 + 0.4 + 0.6) / 3 = 0.400
            Second = (0.4 + 0.2 + 0.8) / 3 = 0.467
            Third  = (0.6 + 0.8 + 0.2) / 3 = 0.533

            Final embedding:

            [0.400, 0.467, 0.533]

            That's exactly why your code checks:

            if (attentionMask[token] == 0)
                continue;

            It means:

            "Don't include this token when calculating the average."
         */

        private readonly InferenceSession session;
        private readonly Tokenizer tokenizer;
        string modelPath = @"Models\all-MiniLM-L6-v2\model.onnx";
        string tokenizerPath = @"Models\all-MiniLM-L6-v2\tokenizer.json";

        public RagEmbeddingService()
        {
            session = new InferenceSession(modelPath);
            tokenizer = Tokenizer.FromFile(tokenizerPath);
        }
        public void Dispose()
        {
            session.Dispose();
            tokenizer.Dispose();
        }

        public async Task< double[]> GenerateEmbedding(string text)
        {
            var encoding = tokenizer
                .Encode(text, true)
                .First();

            var inputIds = encoding.Ids
                .Select(x => (long)x)
                .ToArray();

            var attentionMask = encoding.AttentionMask.Count > 0
                            ? encoding.AttentionMask
                                .Select(x => (long)x)
                                .ToArray()
                            : Enumerable
                                .Repeat(1L, inputIds.Length)
                                .ToArray();

            var tokenTypeIds =
                        Enumerable
                            .Repeat(0L, inputIds.Length)
                            .ToArray();

            int sequenceLength = inputIds.Length;

            var inputIdsTensor = new DenseTensor<long>(
                inputIds,
                new[] { 1, sequenceLength });

            var attentionMaskTensor = new DenseTensor<long>(
                attentionMask,
                new[] { 1, sequenceLength });

            var tokenTypeIdsTensor = new DenseTensor<long>(
                tokenTypeIds,
                new[] { 1, sequenceLength });

            var inputs = new List<NamedOnnxValue>
                            {
                                NamedOnnxValue.CreateFromTensor(
                                    "input_ids",
                                    inputIdsTensor),

                                NamedOnnxValue.CreateFromTensor(
                                    "attention_mask",
                                    attentionMaskTensor),

                                NamedOnnxValue.CreateFromTensor(
                                    "token_type_ids",
                                    tokenTypeIdsTensor)
                            };

            using var outputs = session.Run(inputs);

            var output = outputs
                        .First(x => x.Name == "last_hidden_state")
                        .AsTensor<float>();

            return await MeanPooling(
                output,
                attentionMask);
        }

        private async Task< double[]> MeanPooling(Tensor<float> tokenEmbeddings, long[] attentionMask)
        {
            int sequenceLength = tokenEmbeddings.Dimensions[1];
            int embeddingSize = tokenEmbeddings.Dimensions[2];

            var sentenceEmbedding =
                new double[embeddingSize];

            float totalTokens = 0;

            for (int token = 0; token < sequenceLength; token++)
            {
                if (attentionMask[token] == 0)
                    continue;

                totalTokens++;

                for (int dimension = 0;
                     dimension < embeddingSize;
                     dimension++)
                {
                    sentenceEmbedding[dimension] +=
                        tokenEmbeddings[0, token, dimension];
                }
            }

            if (totalTokens == 0)
                return sentenceEmbedding;

            for (int dimension = 0;
                 dimension < embeddingSize;
                 dimension++)
            {
                sentenceEmbedding[dimension] /=
                    totalTokens;
            }

            return await Normalize(sentenceEmbedding);
        }


        private async Task< double[]> Normalize(double[] vector)
        {
            double magnitude = 0;

            foreach (float value in vector)
            {
                magnitude += value * value;
            }

            magnitude = Math.Sqrt(magnitude);

            if (magnitude == 0)
                return vector;

            for (int i = 0; i < vector.Length; i++)
            {
                vector[i] =
                    (float)(vector[i] / magnitude);
            }

            return vector;
        }

        // Test model
        public void PrintModelInfo()
        {
            Console.WriteLine("========== INPUTS ==========");

            foreach (var input in session.InputMetadata)
            {
                Console.WriteLine(
                    $"Name: {input.Key}");

                Console.WriteLine(
                    $"Type: {input.Value.ElementType}");

                Console.WriteLine(
                    $"Dimensions: {string.Join(", ", input.Value.Dimensions)}");

                Console.WriteLine();
            }

            Console.WriteLine("========== OUTPUTS ==========");

            foreach (var output in session.OutputMetadata)
            {
                Console.WriteLine(
                    $"Name: {output.Key}");

                Console.WriteLine(
                    $"Type: {output.Value.ElementType}");

                Console.WriteLine(
                    $"Dimensions: {string.Join(", ", output.Value.Dimensions)}");

                Console.WriteLine();
            }
        }
    }
}
