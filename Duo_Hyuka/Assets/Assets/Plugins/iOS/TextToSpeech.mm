#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>

static AVSpeechSynthesizer *speechSynthesizer = nil;

extern "C"
{
    void SpeakText(const char* text)
    {
        if (text == NULL)
            return;

        NSString *string =
            [NSString stringWithUTF8String:text];

        if (string == nil || string.length == 0)
            return;

        // Configure iOS audio output
        AVAudioSession *audioSession =
            [AVAudioSession sharedInstance];

        NSError *error = nil;

        [audioSession setCategory:AVAudioSessionCategoryPlayback
                            mode:AVAudioSessionModeSpokenAudio
                         options:AVAudioSessionCategoryOptionDuckOthers
                           error:&error];

        if (error != nil)
        {
            NSLog(@"TTS Audio Session Error: %@", error);
        }

        error = nil;

        [audioSession setActive:YES error:&error];

        if (error != nil)
        {
            NSLog(@"TTS Audio Activation Error: %@", error);
        }

        // Keep synthesizer alive
        if (speechSynthesizer == nil)
        {
            speechSynthesizer =
                [[AVSpeechSynthesizer alloc] init];
        }

        AVSpeechUtterance *utterance =
            [AVSpeechUtterance speechUtteranceWithString:string];

        utterance.voice =
            [AVSpeechSynthesisVoice voiceWithLanguage:@"en-US"];

        utterance.rate = 0.45f;
        utterance.volume = 1.0f;

        [speechSynthesizer speakUtterance:utterance];

        NSLog(@"TTS speaking: %@", string);
    }
}