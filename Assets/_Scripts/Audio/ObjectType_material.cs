using UnityEngine;
using System.Collections;

public class ObjectType_material : MonoBehaviour {
    
    // material types
    public enum MaterialType
    {
        wood,
        grass
    }
    // public instant of enums to select current attached gameObject's material
    public MaterialType material;
}
