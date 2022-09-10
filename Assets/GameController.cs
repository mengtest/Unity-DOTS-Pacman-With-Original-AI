using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public UnityEngine.Sprite[] rightAnimSprites;

    private void Start()
    {
        Instance = this;
    }
}
