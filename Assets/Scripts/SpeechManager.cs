using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crosstales.RTVoice;
//using Crosstales.RTVoice.Model;
using System.Linq;

public class SpeechManager : MonoBehaviour
{
    public static SpeechManager Instance { get; private set; } = null;
    [Range(0f, 3f)]
    public float rate = 1.15f;
    [Range(0, 2f)]
    public float pitch = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    public void StartTalking(string speech, bool isChild = false)
    {
        float ageRate = isChild ? rate + 0.05f : rate - 0.05f;
        float agePitch = isChild ? pitch + 0.65f : pitch - 0.25f;
        Speaker.SpeakNative(speech, Speaker.VoiceForGender(Crosstales.RTVoice.Model.Enum.Gender.FEMALE, "de"), ageRate, agePitch);
    }
    
    public IEnumerator WaitForTalking(string speech, bool isChild = false)
    {
        float ageRate = isChild ? rate + 0.05f : rate - 0.05f;
        float agePitch = isChild ? pitch + 0.65f : pitch - 0.25f;
        Speaker.SpeakNative(speech, Speaker.VoiceForGender(Crosstales.RTVoice.Model.Enum.Gender.FEMALE, "de"), ageRate, agePitch);
        while (!Speaker.isSpeaking)
            yield return null;
        while (Speaker.isSpeaking)
            yield return null;        
    }


#if false
    public void Start()
    {
        StartCoroutine(TestSpeak());
    }
    private IEnumerator TestSpeak()
    {
        int ctr = 3;
        while( ctr > 0 ) {
            Debug.LogFormat("(SpeechManager) | countdown: {0}", ctr--);
            yield return new WaitForSeconds(1f);
        }

        // async speech call
        StartTalking("jetzt ist die Zeit");

        yield return new WaitForSeconds(3f);
        if (Speaker.Cultures.Count > 0)
            foreach (string foo in Speaker.Cultures)
                Debug.LogFormat("voices: {0}", foo);

        // wait for speech to finish call
        yield return StartCoroutine(WaitForTalking("jetzt ist die Zeit"));
        
        SpeakingFinished();
    }
    public void SpeakingFinished()
    {
        Debug.Log("Finished Speaking");
    }
#endif
}
