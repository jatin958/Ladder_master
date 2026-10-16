using System;
using UnityEngine;

public enum colorType
{
    None,
    red,
    yellow,
    blue,
    green
}
public class materialManager : MonoBehaviour
{
    public static materialManager instance;
    [SerializeField] private Material ladderMaterial;
    [SerializeField] private Material playerMaterial;


    private void Awake()
    {
        instance = this;
    }
    
    public void setColor(Color cl)
    {
        playerMaterial.color = cl;
        ladderMaterial.color = cl;
    }
}
