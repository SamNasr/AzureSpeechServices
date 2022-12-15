using Microsoft.CognitiveServices.Speech;        //Requires "Microsoft.CognitiveServices.Speech" NuGet package.
using Microsoft.CognitiveServices.Speech.Audio;  //Requires "Microsoft.CognitiveServices.Speech" NuGet package.
using System;
using System.Configuration;
using System.Threading.Tasks;


// Gotchas: 
// 1. Had to set "Error Condition="'$(Platform.ToLower())' == 'x64'" in D:\Users\sam\OneDrive\Presentations\SpeechToText\MicDemo\packages\Microsoft.CognitiveServices.Speech.1.18.0\build\Microsoft.CognitiveServices.Speech.props
// 2. In Project Properties > Build, change "Platform" to x64

class Program
{
    async static Task Main(string[] args)
    {
        string subscriptionKey = ConfigurationManager.AppSettings["subscription"].ToString();
        string region = ConfigurationManager.AppSettings["region"].ToString(); 
        SpeechConfig speechConfig = SpeechConfig.FromSubscription(subscriptionKey, region);

        AudioConfig audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        SpeechRecognizer recognizer = new SpeechRecognizer(speechConfig, audioConfig);

        while (true)
        {
            Console.WriteLine("How can I help you?");
            SpeechRecognitionResult result = await recognizer.RecognizeOnceAsync();
            Console.WriteLine($"You said '{result.Text}'. \r\nPress ENTER to continue...");
            Console.ReadLine();
        }
    }
}
