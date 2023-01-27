using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniveralToolsScript : MonoBehaviour
{
    private static UniveralToolsScript _instance;
    public static UniveralToolsScript Instance {
        get {
            return _instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if(_instance == null) {
            _instance = this;
            DontDestroyOnLoad(this);
        } else if(_instance != this) {
            Destroy(this);
        }
    }
    
}
