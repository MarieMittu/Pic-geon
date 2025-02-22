using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMaterialVariator : MonoBehaviour
{
    public int variation = -1;
    public bool hasHeatSignature = true;
    static string[] variationNames =
    {
        "Pigeon basic texture set",
        "Pigeon brown spots",
        "Pigeon brown Texture set",
        "pigeon purple spot set",
        "pigeon purple texture set",
    };
    // materialCache[variation number][material slot] = material for this slot and this variation
    public static Material[][] materialCache;
    public static Material[][] materialCacheNoHeatSignature;
    public GameObject[] bodyParts;
    public GameObject[] beakParts;
    public GameObject[] wingParts;
    public GameObject[] feetParts;
    public GameObject[] eyesParts;
    public GameObject[] lidsParts;
    public GameObject[] thighParts;
    public GameObject[] antenna;
    public GameObject[] gears;

    public GameObject[][] slots;
    static string[] slotTextureNames =
    {
        "body_BaseColor-min",
        "peak_BaseColor-min",
        "wigs_BaseColor-min",
        "feet_BaseColor-min",
        "eyes_BaseColor-min",
        "eyelids1_BaseColor-min",
        "tigh_BaseColor-min"
    };
    // Start is called before the first frame update
    void Start()
    {
        if (variation == -1)
        {
            variation = Random.Range(0, variationNames.Length);
        }
        slots = new GameObject[][]
        {
                bodyParts, beakParts, wingParts, feetParts, eyesParts, lidsParts, thighParts
        };
        if (materialCache == null)
        {
            // create materials
            materialCache = new Material[variationNames.Length][];
            materialCacheNoHeatSignature = new Material[variationNames.Length][];
            for (int varNumber = 0; varNumber < variationNames.Length; varNumber++)
            {
                materialCache[varNumber] = new Material[slots.Length];
                materialCacheNoHeatSignature[varNumber] = new Material[slots.Length];
                for (int slotNumber = 0; slotNumber < slots.Length; slotNumber++)
                {
                    string path = "Textures/" + variationNames[varNumber] + "/" + slotTextureNames[slotNumber];
                    Texture tex = Resources.Load(path) as Texture;
                    // copy material from the built-in material of the right body part
                    Material mat = new Material(slots[slotNumber][0].GetComponent<SkinnedMeshRenderer>().material);
                    mat.SetTexture("_MainTex", tex);
                    materialCache[varNumber][slotNumber] = mat;
                    materialCacheNoHeatSignature[varNumber][slotNumber] = new Material(mat);
                }
            }
        }
        // apply the correct material based on variation
        for (int slotNumber = 0; slotNumber < slots.Length; slotNumber++)
        {
            for (int partNumber = 0; partNumber < slots[slotNumber].Length; partNumber++)
            {
                Material mat;
                if (hasHeatSignature) mat = materialCache[variation][slotNumber];
                else   mat = materialCacheNoHeatSignature[variation][slotNumber];
                slots[slotNumber][partNumber].GetComponent<SkinnedMeshRenderer>().material = mat;
            }
        }
    }
}
