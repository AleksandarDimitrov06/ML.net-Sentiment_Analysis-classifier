using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;

namespace Sentiment_Analysis;

public static class SentimentModelTrainer
{
    public static void TrainAndSave(string dataPath, string modelPath)
    {
        var mlContext = new MLContext();

        IDataView trainingData = mlContext.Data.LoadFromTextFile<ModelInput>(
            path: dataPath,
            hasHeader: true,
            separatorChar: '\t');

        var textOptions = new TextFeaturizingEstimator.Options
        {
            WordFeatureExtractor = new WordBagEstimator.Options
            {
                NgramLength = 2,
                UseAllLengths = true,
                Weighting = NgramExtractingEstimator.WeightingCriteria.TfIdf,
            },
        };

        var pipeline = mlContext.Transforms.Text
            .FeaturizeText("Features", textOptions, nameof(ModelInput.Text))
            .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());

        Console.WriteLine("Training the model...");
        ITransformer model = pipeline.Fit(trainingData);

        mlContext.Model.Save(model, trainingData.Schema, modelPath);
        Console.WriteLine($"Model saved to: {modelPath}");
    }
}