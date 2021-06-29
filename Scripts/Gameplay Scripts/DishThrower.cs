using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DishThrower : MonoBehaviour
{
    public GameObject DishPrefab;
    public List<GameObject> DishSpawners;
    float timeBetweenDishes = 1f;
    float timeSinceDishThrow = 0f;
    List<int> dishSelection = new List<int> { 5, 5, 3, 3, 2, 1 };
    List<int> dishSelectionThisGame;
    bool throwingDishes = false;
    int dishPattern = 0;
    //1 = left to right 3
    //2 - right to left 3
    //3 - left to right 5
    //4 - right to left 5
    //5 - middle, left and right, leftmost and rightmost

    int dishesThrownInPattern = 0;

    public void ResetDishes()
    {
        dishSelectionThisGame = new List<int>(dishSelection);
    }

    public void SpawnDish()
    {
        if (dishSelectionThisGame.Count > 0)
        {
            int index = Random.Range(0, dishSelectionThisGame.Count);
            int dishes = dishSelectionThisGame[index];
            dishSelectionThisGame.Remove(index);

            int pattern = 0;
            switch (dishes)
            {
                case 1:
                    //If we're throwing one dish, choose a random dish to throw
                    GameObject randomSpawner = DishSpawners[Random.Range(0, DishSpawners.Count)];
                    break;
                case 2:
                    int rand = Random.Range(0, DishSpawners.Count);
                    Instantiate(DishPrefab, DishSpawners[rand].transform.position, DishSpawners[rand].transform.rotation);
                    if (rand + 2 >= DishSpawners.Count)
                    {
                        Instantiate(DishPrefab, DishSpawners[rand - 2].transform.position, DishSpawners[rand - 2].transform.rotation);
                    }
                    else
                    {
                        Instantiate(DishPrefab, DishSpawners[rand + 2].transform.position, DishSpawners[rand + 2].transform.rotation);
                    }
                    break;
                case 3:
                    //If we're throwing three, randomly decide if we're throwing pattern 1, 2, or all three at once.
                    pattern = Random.Range(0, 3);
                    if (pattern > 0)
                    {
                        dishPattern = pattern;
                        throwingDishes = true;
                    }
                    else
                    {
                        Instantiate(DishPrefab, DishSpawners[1].transform.position, DishSpawners[1].transform.rotation);
                        Instantiate(DishPrefab, DishSpawners[2].transform.position, DishSpawners[2].transform.rotation);
                        Instantiate(DishPrefab, DishSpawners[3].transform.position, DishSpawners[3].transform.rotation);
                    }
                    break;
                case 5:
                    //If we're throwing five, are we throwing pattern 3, 4, or 5?
                    pattern = Random.Range(3, 6);
                    dishPattern = pattern;
                    throwingDishes = true;
                    break;
                default:
                    break;
            }
        }
    }

    void Update()
    {
        timeSinceDishThrow += Time.deltaTime;
        if (throwingDishes)
        {
            if (timeSinceDishThrow >= timeBetweenDishes)
            {
                switch (dishPattern)
                {
                    case 1:
                        //1 = left to right 3
                        switch (dishesThrownInPattern)
                        {
                            case 0:
                                Instantiate(DishPrefab, DishSpawners[1].transform.position, DishSpawners[1].transform.rotation);
                                dishesThrownInPattern++;
                                break;
                            case 1:
                                Instantiate(DishPrefab, DishSpawners[2].transform.position, DishSpawners[2].transform.rotation);
                                dishesThrownInPattern++;
                                break;
                            case 2:
                                Instantiate(DishPrefab, DishSpawners[3].transform.position, DishSpawners[3].transform.rotation);
                                dishesThrownInPattern = 0;
                                dishPattern = 0;
                                throwingDishes = false;
                                break;
                        }
                        break;

                    case 2:
                        //2 - right to left 3
                        switch (dishesThrownInPattern)
                        {
                            case 0:
                                Instantiate(DishPrefab, DishSpawners[3].transform.position, DishSpawners[3].transform.rotation);
                                dishesThrownInPattern++;
                                break;
                            case 1:
                                Instantiate(DishPrefab, DishSpawners[2].transform.position, DishSpawners[2].transform.rotation);
                                dishesThrownInPattern++;
                                break;
                            case 2:
                                Instantiate(DishPrefab, DishSpawners[1].transform.position, DishSpawners[1].transform.rotation);
                                dishesThrownInPattern = 0;
                                dishPattern = 0;
                                throwingDishes = false;
                                break;
                        }
                        break;
                    case 3:
                        //3 - left to right 5
                        switch (dishesThrownInPattern)
                        {
                            case 0:
                                Instantiate(DishPrefab, DishSpawners[0].transform.position, DishSpawners[0].transform.rotation);
                                dishesThrownInPattern++;
                                break;
                            case 1:
                                Instantiate(DishPrefab, DishSpawners[1].transform.position, DishSpawners[1].transform.rotation);
                                dishesThrownInPattern++;
                                break;
                            case 2:
                                Instantiate(DishPrefab, DishSpawners[2].transform.position, DishSpawners[2].transform.rotation);
                                dishesThrownInPattern++;
                                break;
                            case 3:
                                Instantiate(DishPrefab, DishSpawners[3].transform.position, DishSpawners[3].transform.rotation);
                                dishesThrownInPattern++;
                                break;
                            case 4:
                                Instantiate(DishPrefab, DishSpawners[4].transform.position, DishSpawners[4].transform.rotation);
                                dishesThrownInPattern = 0;
                                dishPattern = 0;
                                throwingDishes = false;
                                break;
                        }
                        break;
                    case 4:
                        //4 - right to left 5
                        switch (dishesThrownInPattern)
                        {
                            case 0:
                                Instantiate(DishPrefab, DishSpawners[4].transform.position, DishSpawners[4].transform.rotation);
                                dishesThrownInPattern++;
                                break;
                            case 1:
                                Instantiate(DishPrefab, DishSpawners[3].transform.position, DishSpawners[3].transform.rotation);
                                dishesThrownInPattern++;
                                break;
                            case 2:
                                Instantiate(DishPrefab, DishSpawners[2].transform.position, DishSpawners[2].transform.rotation);
                                dishesThrownInPattern++;
                                break;
                            case 3:
                                Instantiate(DishPrefab, DishSpawners[1].transform.position, DishSpawners[1].transform.rotation);
                                dishesThrownInPattern++;
                                break;
                            case 4:
                                Instantiate(DishPrefab, DishSpawners[0].transform.position, DishSpawners[0].transform.rotation);
                                dishesThrownInPattern = 0;
                                dishPattern = 0;
                                throwingDishes = false;
                                break;
                        }
                        break;
                    case 5:
                        //5 - middle, left and right, leftmost and rightmost
                        switch (dishesThrownInPattern)
                        {
                            case 0:
                                Instantiate(DishPrefab, DishSpawners[2].transform.position, DishSpawners[2].transform.rotation);
                                dishesThrownInPattern++;
                                break;
                            case 1:
                                Instantiate(DishPrefab, DishSpawners[3].transform.position, DishSpawners[3].transform.rotation);
                                Instantiate(DishPrefab, DishSpawners[1].transform.position, DishSpawners[1].transform.rotation);
                                dishesThrownInPattern++;
                                break;
                            case 2:
                                Instantiate(DishPrefab, DishSpawners[0].transform.position, DishSpawners[0].transform.rotation);
                                Instantiate(DishPrefab, DishSpawners[4].transform.position, DishSpawners[4].transform.rotation);
                                dishesThrownInPattern = 0;
                                dishPattern = 0;
                                throwingDishes = false;
                                break;
                        }
                        break;
                    default:
                        break;
                }
                timeSinceDishThrow = 0f;
            }
        }
    }
}

