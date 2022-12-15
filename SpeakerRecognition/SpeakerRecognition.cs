using Microsoft.CognitiveServices.Speech;        //Requires "Microsoft.CognitiveServices.Speech" NuGet package.
using Microsoft.CognitiveServices.Speech.Audio;  //Requires "Microsoft.CognitiveServices.Speech" NuGet package.
using Microsoft.CognitiveServices.Speech.Speaker;//Requires "Microsoft.CognitiveServices.Speech" NuGet package.
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

//This is a gated service, make sure your Azure Subscription ID is approved: https://aka.ms/azure-speaker-recognition.
//For more information, please visit https://aka.ms/SpeakerRecoTransparencyNote

namespace SpeakerRecognition
{
    public class SpeakerRecognition
    {
        static async Task Main(string[] args)
        {
            string subscriptionKey = ConfigurationManager.AppSettings["subscription"].ToString();
            string region = ConfigurationManager.AppSettings["region"].ToString();
            var config = SpeechConfig.FromSubscription(subscriptionKey, region);

            // persist profileMapping if you want to store a record of who the profile is
            var profileMapping = new Dictionary<string, string>();
            await VerificationEnroll(config, profileMapping);

            Console.ReadLine();
        }

        public static async Task VerificationEnroll(SpeechConfig config, Dictionary<string, string> profileMapping)
        {
            using (var client = new VoiceProfileClient(config))
            using (var profile = await client.CreateProfileAsync(VoiceProfileType.TextDependentVerification, "en-us"))
            {
                var phraseResult = await client.GetActivationPhrasesAsync(VoiceProfileType.TextDependentVerification, "en-us");
                using (var audioInput = AudioConfig.FromDefaultMicrophoneInput())
                {
                    Console.WriteLine($"Enrolling profile id {profile.Id}.");
                    // give the profile a human-readable display name
                    profileMapping.Add(profile.Id, "Your Name");

                    VoiceProfileEnrollmentResult result = null;
                    while (result is null || result.RemainingEnrollmentsCount > 0)
                    {
                        Console.WriteLine($"Speak the passphrase, \"${phraseResult.Phrases[0]}\"");
                        result = await client.EnrollProfileAsync(profile, audioInput);
                        Console.WriteLine($"Remaining enrollments needed: {result.RemainingEnrollmentsCount}");
                        Console.WriteLine("");
                    }

                    if (result.Reason == ResultReason.EnrolledVoiceProfile)
                    {
                        await SpeakerVerify(config, profile, profileMapping);
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = VoiceProfileEnrollmentCancellationDetails.FromResult(result);
                        Console.WriteLine($"CANCELED {profile.Id}: ErrorCode={cancellation.ErrorCode} ErrorDetails={cancellation.ErrorDetails}");
                    }
                }
            }
        }

        public static async Task SpeakerVerify(SpeechConfig config, VoiceProfile profile, Dictionary<string, string> profileMapping)
        {
            var speakerRecognizer = new SpeakerRecognizer(config, AudioConfig.FromDefaultMicrophoneInput());
            var model = SpeakerVerificationModel.FromProfile(profile);

            Console.WriteLine("Speak the passphrase to verify: \"My voice is my passport, please verify me.\"");
            var result = await speakerRecognizer.RecognizeOnceAsync(model);
            Console.WriteLine($"Verified voice profile for speaker {profileMapping[result.ProfileId]}, score is {result.Score}");
        }

    }
}
