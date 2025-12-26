using Microsoft.ML;
using Sentiment_Analysis;

string basePath = AppContext.BaseDirectory;
string modelPath = Path.Combine(basePath, "SentimentModel.zip");
string dataPath = Path.Combine(basePath, "sentiment_data.tsv");

string mode = args.Length > 0 ? args[0] : "predict";

if (mode.Equals("train", StringComparison.OrdinalIgnoreCase))
{
    if (!File.Exists(dataPath))
    {
        Console.WriteLine($"Error: Training data file not found: {dataPath}");
        return;
    }

    SentimentModelTrainer.TrainAndSave(dataPath, modelPath);
    return;
}

if (!File.Exists(modelPath))
{
    Console.WriteLine($"Error: Model file not found: {modelPath}");
    Console.WriteLine("Run: dotnet run -- train");
    return;
}

var predictor = SentimentPredictor.Load(modelPath);

Console.WriteLine("Model loaded from file!");
Console.WriteLine("Type a review to test (or 'Close' to exit).");

while (true)
{
    Console.Write("Enter review: ");
    string? input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input) || input.Equals("Close", StringComparison.OrdinalIgnoreCase))
        break;

    var result = predictor.Predict(input);

    string sentiment = result.PredictedLabel ? "Positive" : "Negative";
    Console.WriteLine($"Prediction: {sentiment} (Score: {result.Score:F2})");
    Console.WriteLine();
}