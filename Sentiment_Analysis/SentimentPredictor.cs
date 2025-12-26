using Microsoft.ML;

namespace Sentiment_Analysis;

public sealed class SentimentPredictor
{
    private readonly PredictionEngine<ModelInput, ModelOutput> _predictionEngine;

    private SentimentPredictor(PredictionEngine<ModelInput, ModelOutput> predictionEngine)
    {
        _predictionEngine = predictionEngine;
    }

    public static SentimentPredictor Load(string modelPath)
    {
        var mlContext = new MLContext();

        DataViewSchema _;
        ITransformer model = mlContext.Model.Load(modelPath, out _);

        PredictionEngine<ModelInput, ModelOutput> engine =
            mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(model);

        return new SentimentPredictor(engine);
    }

    public ModelOutput Predict(string text)
        => _predictionEngine.Predict(new ModelInput { Text = text });
}