using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RanmaizeGameobjectChildren : MonoBehaviour
{


    public GameObject[] childObjects;

    public int[] shuffledPos;
    int tempInt;

    // Start is called before the first frame update
    void Start()
    {
        childObjects = new GameObject[transform.childCount];
        shuffledPos = new int[transform.childCount];

        for (int i = 0; i < transform.childCount; i++) {
            shuffledPos[i] = i;
            childObjects[i] = transform.GetChild(i).gameObject;
        }

        Shuffle();
    }

    // Update is called once per frame
    void Update()
    {


    }


    public void Shuffle()
    {
        for (int i = 0; i < shuffledPos.Length; i++)
        {
            int rnd = Random.Range(0, shuffledPos.Length);
            tempInt = shuffledPos[rnd];
            shuffledPos[rnd] = shuffledPos[i];
            shuffledPos[i] = tempInt;

        }

        for (int i = 0; i < childObjects.Length; i++)
        {
            childObjects[i].transform.SetSiblingIndex(shuffledPos[i]);
        }


    }



}
