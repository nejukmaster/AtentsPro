using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeSystem : MonoBehaviour
{
    public static HomeSystem instance;
    public static HomeSystem GetInstance() {  return instance; }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
