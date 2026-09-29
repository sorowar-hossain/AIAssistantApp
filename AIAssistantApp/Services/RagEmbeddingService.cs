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

        public double[] GenerateEmbedding(string text)
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

            return MeanPooling(
                output,
                attentionMask);
        }

        private double[] MeanPooling(Tensor<float> tokenEmbeddings, long[] attentionMask)
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

            return Normalize(sentenceEmbedding);
        }


        private double[] Normalize(double[] vector)
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
