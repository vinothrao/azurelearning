using Mldemo_issue_ClassificationML.Model;
using System;

namespace mldemo_issue_Classification
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = new ModelInput();
            input.Description = "The WebSockets communication used under the covers by SignalR looks like is going slow in my development machine..";
            input.Title = "WebSockets communication is slow in my machine";

            // Load model and predict output of sample data
            ModelOutput result = ConsumeModel.Predict(input);
            Console.WriteLine($"Area: { result.Prediction }");
            Console.ReadLine();
        }
    }
}
