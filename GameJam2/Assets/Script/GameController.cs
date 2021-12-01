using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController instance;                     //Instance of this controller
    public Vector2 lastCheckPoint;                              //Holds last checkpoint position

    void Awake()
    {
        if (instance == null)                                   //If this object does not exist...
        {
            instance = this;                                    //Create the object
            DontDestroyOnLoad(instance);                        //Don't delete it on reset
        }
        else                                                    //else
        {
            Destroy(gameObject);                                //Destory this object
        }
    }
}
