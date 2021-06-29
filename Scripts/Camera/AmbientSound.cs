using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AmbientSound : MonoBehaviour {
    public List<AudioSource> wind = new List<AudioSource>();
    public List<AudioSource> animalNoises = new List<AudioSource>();
    int animalToPlay = 0;
    float timeBetweenAnimalNoises;
    float minTimeBetweenNoises = 10f;
    float maxTimeBetweenNoises = 20f;
    float timeSinceAnimalNoise = 0f;

    void Start()
    {
        int wind1 = Random.Range(0, wind.Count);
        StartCoroutine(playWind(wind1, 0));
        animalNoises = Shuffle<AudioSource>(animalNoises);
        timeBetweenAnimalNoises = Random.Range(minTimeBetweenNoises, maxTimeBetweenNoises);
    }

    void Update()
    {
        timeSinceAnimalNoise += Time.deltaTime;
        if (timeSinceAnimalNoise > timeBetweenAnimalNoises)
        {
            animalNoises[animalToPlay].Play();
            animalToPlay++;
            if (animalToPlay == animalNoises.Count)
            {
                animalToPlay = 0;
                animalNoises = Shuffle<AudioSource>(animalNoises);
            }
            timeBetweenAnimalNoises = Random.Range(minTimeBetweenNoises, maxTimeBetweenNoises);
            timeSinceAnimalNoise = 0f;
        }
    }

    public static List<T> Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            var value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    IEnumerator playWind(int windIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioSource windToPlay = wind[windIndex];
        windToPlay.Play();
        StartCoroutine(playWind(Random.Range(0, wind.Count), windToPlay.clip.length / 2f));
    }
    

}
