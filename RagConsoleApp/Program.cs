using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using SmartComponents.LocalEmbeddings;
using SmartComponents.Inference;

if (args.Length < 2)
{
    Console.WriteLine("Usage: RagConsoleApp <pdf-path> <question>");
    return;
}

string pdfPath = args[0];
string question = string.Join(" ", args.Skip(1));

var sb = new StringBuilder();
using (var document = PdfDocument.Open(pdfPath))
{
    foreach (var page in document.GetPages())
    {
        sb.AppendLine(ContentOrderTextExtractor.GetText(page));
    }
}
string pdfText = sb.ToString();

List<string> chunks = new();
for (int i = 0; i < pdfText.Length; i += 1000)
{
    int len = Math.Min(1000, pdfText.Length - i);
    chunks.Add(pdfText.Substring(i, len));
}

using var embedder = new LocalEmbedder();
var embeddedChunks = embedder.EmbedRange(chunks);
var results = embedder.FindClosestWithScore(
    new SimilarityQuery { SearchText = question, MaxResults = 3 },
    embeddedChunks);

Console.WriteLine("Top matching chunks:");
foreach (var r in results)
{
    Console.WriteLine($"Score: {r.Similarity:F3}\n{r.Item}\n");
}
