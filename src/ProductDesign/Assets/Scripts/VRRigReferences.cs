using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRRigReferences : MonoBehaviour
{
    public static VRRigReferences Instance { get; private set; }

    [SerializeField] private Transform root;
    [SerializeField] private Transform head;
    [SerializeField] private Transform left;
    [SerializeField] private Transform right;

    public Transform Root { 
        get { return root; }
        set { root = value; }
    }
    public Transform Head { 
        get { return head; }
        set { head = value; }
    }
    public Transform Left {
        get { return left; }
        set { left = value; }
    }
    public Transform Right {
        get { return right; }
        set { right = value; }
    }

    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
        
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
}
