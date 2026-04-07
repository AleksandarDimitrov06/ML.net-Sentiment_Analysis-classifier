using Microsoft.ML;
using Sentiment_Analysis;

string basePath = AppContext.BaseDirectory;
string modelPath = Path.Combine(basePath, "SentimentModel.zip");
string dataPath = Path.Combine(basePath, "sentiment_data.tsv");
string testDataPath = Path.Combine(basePath, "sentiment_test_data.tsv");


Console.WriteLine("test/train/predict: ");
string? mode = Console.ReadLine();


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

if (mode.Equals("test", StringComparison.OrdinalIgnoreCase))
{
    if (!File.Exists(testDataPath))
    {
        Console.WriteLine($"Error: Test data file not found: {testDataPath}");
        Console.WriteLine("Place 'sentiment_test_data.tsv' in the output folder.");
        return;
    }

    if (!File.Exists(modelPath))
    {
        Console.WriteLine($"Model not found: {modelPath}");
        Console.WriteLine("Training a new model...");

        if (!File.Exists(dataPath))
        {
            Console.WriteLine($"Error: Training data file not found: {dataPath}");
            return;
        }

        SentimentModelTrainer.TrainAndSave(dataPath, modelPath);

        if (!File.Exists(modelPath))
        {
            Console.WriteLine("Error: Training completed but the model file was not created.");
            return;
        }
    }

    RunAccuracyTest(modelPath, testDataPath);
    return;
}

if (!File.Exists(modelPath))
{
    Console.WriteLine($"Model not found: {modelPath}");
    Console.WriteLine("Training a new model...");

    if (!File.Exists(dataPath))
    {
        Console.WriteLine($"Error: Training data file not found: {dataPath}");
        Console.WriteLine("Place 'sentiment_data.tsv' in the output folder or update the path in Program.cs.");
        return;
    }

    SentimentModelTrainer.TrainAndSave(dataPath, modelPath);

    if (!File.Exists(modelPath))
    {
        Console.WriteLine("Error: Training completed but the model file was not created.");
        return;
    }
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

// ── TEST HELPER ───────────────────────────────────────────────────────────────

static void RunAccuracyTest(string modelPath, string testDataPath)
{
    var predictor = SentimentPredictor.Load(modelPath);

    var lines = File.ReadAllLines(testDataPath).Skip(1);

    int total = 0, correct = 0, truePos = 0, trueNeg = 0, falsePos = 0, falseNeg = 0;

    foreach (var line in lines)
    {
        var parts = line.Split('\t');
        if (parts.Length < 2) continue;

        string text = parts[0].Trim();

        bool actualLabel;
        if (!bool.TryParse(parts[1].Trim(), out actualLabel))
        {
            if (int.TryParse(parts[1].Trim(), out int numLabel))
                actualLabel = numLabel == 1;
            else
                continue;
        }

        var result = predictor.Predict(text);
        bool predicted = result.PredictedLabel;

        total++;
        if (predicted == actualLabel)
        {
            correct++;
            if (predicted) truePos++; else trueNeg++;
        }
        else
        {
            if (predicted) falsePos++; else falseNeg++;
        }
    }

    if (total == 0)
    {
        Console.WriteLine("No valid samples found in the test file.");
        return;
    }

    double accuracy = (double)correct / total;
    double precision = (truePos + falsePos) > 0 ? (double)truePos / (truePos + falsePos) : 0;
    double recall = (truePos + falseNeg) > 0 ? (double)truePos / (truePos + falseNeg) : 0;
    double f1 = (precision + recall) > 0 ? 2 * precision * recall / (precision + recall) : 0;

    Console.WriteLine("==============================================");
    Console.WriteLine("         SENTIMENT MODEL ACCURACY TEST        ");
    Console.WriteLine("==============================================");
    Console.WriteLine();
    Console.WriteLine($"  Samples tested : {total}");
    Console.WriteLine($"  Correct        : {correct}");
    Console.WriteLine($"  Wrong          : {total - correct}");
    Console.WriteLine();
    Console.WriteLine($"  Accuracy       : {accuracy * 100:F2}%");
    Console.WriteLine($"  Precision      : {precision * 100:F2}%");
    Console.WriteLine($"  Recall         : {recall * 100:F2}%");
    Console.WriteLine($"  F1 Score       : {f1 * 100:F2}%");
    Console.WriteLine();
    Console.WriteLine("  Confusion Matrix:");
    Console.WriteLine($"    True  Positive : {truePos,4}   False Positive : {falsePos,4}");
    Console.WriteLine($"    False Negative : {falseNeg,4}   True  Negative : {trueNeg,4}");
    Console.WriteLine("==============================================");
}