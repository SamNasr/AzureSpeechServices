using Microsoft.CognitiveServices.Speech;
using System;
using System.Configuration;
using System.Threading.Tasks;

class Program
{
    async static Task Main(string[] args)
    {
        string subscriptionKey = ConfigurationManager.AppSettings["subscription"].ToString();
        string region = ConfigurationManager.AppSettings["region"].ToString();

        SpeechConfig speechConfig = SpeechConfig.FromSubscription(subscriptionKey, region);
        speechConfig.SpeechSynthesisVoiceName = "en-US-JennyNeural";
        SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer(speechConfig);

        while (true)
        {
            Console.WriteLine("Enter Text: ");
            string text = Console.ReadLine();

            SpeechSynthesisResult speechSynthesisResult = await speechSynthesizer.SpeakTextAsync(text);
            CheckResult(speechSynthesisResult, text);
            Console.WriteLine("\r\nPress ENTER to continue...");
            Console.ReadLine();
        }
    }

    static void CheckResult(SpeechSynthesisResult speechSynthesisResult, string text)
    {
        switch (speechSynthesisResult.Reason)
        {
            case ResultReason.SynthesizingAudioCompleted:
                Console.WriteLine($"Speech for text: [{text}]");
                break;

            case ResultReason.Canceled:
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechSynthesisResult);
                Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}  ErrorDetails=[{cancellation.ErrorDetails}]");
                }
                break;

            default:
                break;
        }
    }
}

