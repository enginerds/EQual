using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeController : MonoBehaviour {
    
    public IEnumerator GradualVolumeChange(int numSeconds, AudioSource audioSource, float endVolume) {

        float increment = ((endVolume - audioSource.volume) / numSeconds);

        for (int i = 0; i <= numSeconds; i++) {
            yield return new WaitForSeconds(1.0f);
            audioSource.volume += increment;
        }
    }

    public IEnumerator GradualVolumeRiseAndFall(int numSeconds, AudioSource audioSource, float midVolume, float endVolume) {
        //This will increment the volume up to the midVolume level by the mid-point of the duration, then decrease the volume to the endVolume from the mid-point to the end of the duration.
        float increment1 = ((midVolume - audioSource.volume) / (numSeconds / 2));
        float increment2 = ((endVolume - midVolume) / (numSeconds / 2));

        for (int i = 0; i <= numSeconds; i++) {
            yield return new WaitForSeconds(1.0f);
            if (i < (numSeconds / 2)) {
                audioSource.volume += increment1;
            } else {
                audioSource.volume += increment2;
            }
        }
    }
}
