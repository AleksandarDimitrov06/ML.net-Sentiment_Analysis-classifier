# ML.NET Sentiment Classifier

A small ML.NET console project that trains a binary sentiment classifier (positive / negative) from tab-separated training data and uses the saved model for interactive prediction. The project targets .NET 8 and uses Microsoft.ML 5.0.0.

## Table of Contents
- [Overview](#overview)
- [Repository Structure](#repository-structure)
- [Prerequisites](#prerequisites)
- [Build](#build)
- [Train the model](#train-the-model)
- [Run predictions (interactive)](#run-predictions-interactive)
- [Data format](#data-format)
- [Model file location](#model-file-location)
- [Development notes](#development-notes)
- [Recommended improvements](#recommended-improvements)
- [Contributing](#contributing)
- [License](#license)

## Overview

This repository contains a simple ML.NET console application that demonstrates how to:

- Load labeled training data from a TSV file.
- Featurize text using TF-IDF and n-grams (bigrams + unigrams).
- Train a binary classifier (SDCA logistic regression).
- Save the trained model to `SentimentModel.zip`.
- Load the saved model and run interactive predictions from the console.

The goal is a minimal, clear example you can extend for production scenarios.

## Repository Structure

- `Program.cs` — App entry point. Supports two modes: `train` (trains and saves the model) and `predict` (loads the model and runs an interactive prompt).
- `SentimentModelTrainer.cs` — Encapsulates training and model saving.
- `SentimentPredictor.cs` — Encapsulates loading the model and making predictions.
- `ModelInput.cs` — Input schema for training / prediction.
- `ModelOutput.cs` — Prediction output schema.
- `sentiment_data.tsv` — (Not checked in) Example training data. Place it next to the built binary when training.
- `SentimentModel.zip` — The saved model produced by training.
- `Sentiment_Analysis.csproj` — Project file (targets .NET 8, references Microsoft.ML 5.0.0).

## Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or later, or any editor that supports .NET 8
- (Optional) `dotnet` CLI

## Build

From Visual Studio: open the solution and press **Build**.

From the command line:

dotnet build

## Train the model

Training requires a TSV file named `sentiment_data.tsv` to be available in the application working directory (the app expects it at runtime relative to `AppContext.BaseDirectory`). The file should have a header and two columns: `Text` and `Label`.

Example training command (CLI):

# Train and save the model to SentimentModel.zip
dotnet run -- train

When run from Visual Studio, the working directory is usually the build output (e.g. `bin/Debug/net8.0`). Put `sentiment_data.tsv` there or update `dataPath` in `Program.cs`.

After training, the saved model will be written to `SentimentModel.zip` in the same directory.

## Run predictions (interactive)

Make sure `SentimentModel.zip` exists in the working directory (either because you trained it or you copied it there). Run:

# Runs interactive prediction (default)
dotnet run -- predict
# or simply
dotnet run

Type a review at the prompt and press Enter. Type `Close` (or press Enter on an empty line) to exit.

## Data format

The project expects a tab-separated file (`.tsv`) with a header and two columns. Columns must be in this order:

1. `Text` — string containing the review text.
2. `Label` — boolean indicating positive (true) or negative (false).

Example `sentiment_data.tsv`:

Text	Label
"I loved the product, it works perfectly"	true
"Terrible experience, will not buy again"	false

Be careful with quoting and tabs when producing the file.

## Model file location

The app uses `AppContext.BaseDirectory` to compute the working directory at runtime. When running in Debug from Visual Studio, that will generally be `bin/Debug/net8.0`. Ensure both `sentiment_data.tsv` (for training) and `SentimentModel.zip` (for prediction) are present in that folder, or edit `Program.cs` to point to a different path.

## Development notes

- `ModelInput` and `ModelOutput` must be in the same namespace (`Sentiment_Analysis`) to avoid type resolution issues.
- The project references `Microsoft.ML` 5.0.0. API signatures can change across ML.NET versions (e.g., `FeaturizeText` overloads). If you upgrade the package, check the pipeline builder calls.
- The trainer currently uses TF-IDF with n-gram length 2 and `UseAllLengths = true` so both unigrams and bigrams are used.

