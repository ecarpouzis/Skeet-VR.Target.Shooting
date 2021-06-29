using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalGalleryManager : MonoBehaviour {

    public List<GameObject> animalModelPrefabs;
    public List<Transform> animalSpawnpoints;
    List<GameObject> spawnedAnimals = new List<GameObject>();
    public Transform playerSelectionSpawnpoint;
    public float timePlayed = 0f;
    public float timeToPlay = 60f;
    bool modeStarted;
    bool selectionInProgress = false;
    GameObject selectedAnimalSpawn;
    List<GameObject> spawningAnimals = new List<GameObject>();
    public static AnimalGalleryManager Instance;
    public GameObject galleryModels;

    void Awake()
    {
        Instance = this;
    }
        
    public void StartAnimalMode()
    {
        galleryModels.SetActive(true);
        timePlayed = 0f;
        modeStarted = true;
    }

    public void EndAnimalMode()
    {
        if (GameTracker.Instance.Points >= 2000)
            VRApiManager.Achievements.GrantAchievement(Achievements.SOME_ANIMALS_WERE_HARMED);

        galleryModels.SetActive(false);
        modeStarted = false;
        EndSelection();
        GameTracker.Instance.GameOver();
    }

    public void EndSelection() {
        Destroy(selectedAnimalSpawn);
        for(int i = 0; i< spawnedAnimals.Count; i++)
        {
            Destroy(spawnedAnimals[i].gameObject);
        }
        selectionInProgress = false;
    }

    public void StartSelection()
    {
        spawningAnimals.Clear();
        GameObject selectedAnimal = animalModelPrefabs.GetRandomValue();
        spawningAnimals.Add(selectedAnimal);
        int animalsSelected = 1;
        //While there's still animals to select
        while (animalsSelected < 9)
        {
            //Choose a random 
            GameObject animalToSpawn = animalModelPrefabs.GetRandomValue();
            if (animalToSpawn != selectedAnimal)
            {
                spawningAnimals.Add(animalToSpawn);
                animalsSelected++;
            }
        }
        spawningAnimals.Shuffle();
        
        for(int i = 0; i<animalSpawnpoints.Count;i++)
        {
            Transform spawnPoint = animalSpawnpoints[i];
            GameObject animal = Instantiate(spawningAnimals[i], spawnPoint);
            spawnedAnimals.Add(animal);
            ResetInstance(animal.transform);
            if (spawningAnimals[i] == selectedAnimal)
            {
                animal.GetComponent<AnimalGalleryTarget>().correctAnimal = true;
                selectedAnimalSpawn = Instantiate(spawningAnimals[i], playerSelectionSpawnpoint);
                selectedAnimalSpawn.GetComponent<AnimalGalleryTarget>().exampleAnimal = true;
                Destroy(selectedAnimalSpawn.GetComponent<BoxCollider>());
                Destroy(selectedAnimalSpawn.GetComponent<Rigidbody>());
                ResetInstance(selectedAnimalSpawn.transform);
            }
        }
        selectionInProgress = true;
    }


    void ResetInstance(Transform obj)
    {
        obj.localPosition = Vector3.zero;
        obj.localScale = Vector3.one;
        obj.localRotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update () {
        if (modeStarted)
        {
            timePlayed += Time.deltaTime;
            if (!selectionInProgress)
            {
                StartSelection();
            }
            if (timePlayed > timeToPlay)
            {
                timePlayed = 0f;
                EndAnimalMode();
            }
        }
	}
}
