#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>

extern "C"
{
    void SpeakText(const char* text)
    {
        if (text == NULL)
            return;

        NSString *string =
            [NSString stringWithUTF8String:text];

        AVSpeechUtterance *utterance =
            [AVSpeechUtterance speechUtteranceWithString:string];

        utterance.voice =
            [AVSpeechSynthesisVoice voiceWithLanguage:@"en-US"];

        utterance.rate = 0.45f;

        AVSpeechSynthesizer *synthesizer =
            [[AVSpeechSynthesizer alloc] init];

        [synthesizer speakUtterance:utterance];
    }
}