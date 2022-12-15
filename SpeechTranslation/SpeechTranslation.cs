using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;
using System;
using System.Configuration;
using System.Threading.Tasks;

class Program
{
    async static Task Main(string[] args)
    {
        string subscriptionKey = ConfigurationManager.AppSettings["subscription"].ToString();
        string region = ConfigurationManager.AppSettings["region"].ToString();
        var speechTranslationConfig = SpeechTranslationConfig.FromSubscription(subscriptionKey, region);

        speechTranslationConfig.SpeechRecognitionLanguage = "en-US";
        speechTranslationConfig.AddTargetLanguage("it");
        speechTranslationConfig.AddTargetLanguage("fr");

        var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        var translationRecognizer = new TranslationRecognizer(speechTranslationConfig, audioConfig);

        while (true)
        {
            Console.WriteLine("Speak into your microphone.");
            var translationRecognitionResult = await translationRecognizer.RecognizeOnceAsync();
            OutputSpeechRecognitionResult(translationRecognitionResult);
            Console.WriteLine("\r\nPress ENTER to continue...");
            Console.ReadLine();
        }
    }

    static void OutputSpeechRecognitionResult(TranslationRecognitionResult translationRecognitionResult)
    {
        switch (translationRecognitionResult.Reason)
        {
            case ResultReason.TranslatedSpeech:
                Console.WriteLine($"RECOGNIZED Text = {translationRecognitionResult.Text}");
                foreach (var element in translationRecognitionResult.Translations)
                {
                    Console.WriteLine($"TRANSLATED Text ('{element.Key}') = {element.Value}");
                }
                break;

            case ResultReason.NoMatch:
                Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                break;

            case ResultReason.Canceled:
                var cancellation = CancellationDetails.FromResult(translationRecognitionResult);
                Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
                }
                break;
        }
    }
}