using System.Collections.Generic;
using UnityEngine;

public class CheckTerrainTexture : MonoBehaviour
{
    [SerializeField] BrushesNames brushesObj;
    Transform playerTransform;
    AudioSource source;
    Terrain activeTerrain;

    int posX, posZ;
    float[] textureValues = new float[4];

    string[] brushsNames = null;
    Brushes[] brush = null;

    AudioClip previousClip;

    public float[] TextureValues { get => textureValues; set => textureValues = value; }
    public Terrain ActiveTerrain { get => activeTerrain; set => activeTerrain = value; }
    public string[] BrushsNames
    {
        set
        {
            brushsNames = value;
            GetClips();
        }
    }

    void GetClips()
    {
        List<Brushes> temp = new List<Brushes>();
        for (int i = 0; i < brushsNames.Length; i++)
        {
            for (int j = 0; j < brushesObj.Clips.Length; j++)
            {
                if (brushsNames[i] == brushesObj.Clips[j].Name)
                {
                    int clip = j;
                    temp.Add(brushesObj.Clips[clip]);
                    break;
                }
            }
        }

        brush = temp.ToArray();
    }

    private void Start()
    {
        source = GetComponent<AudioSource>();
        playerTransform = GetComponent<Transform>();
    }

    public void GetTerrainTexture()
    {
        if (activeTerrain != null)
        {
            ConvertPosition(playerTransform.position);
            CheckTexture();
        }
    }

    void ConvertPosition(Vector3 playerPosition)
    {
        Vector3 terrainPosition = playerPosition - activeTerrain.transform.position;

        Vector3 mapPosition = new Vector3
        (terrainPosition.x / activeTerrain.terrainData.size.x, 0,
        terrainPosition.z / activeTerrain.terrainData.size.z);

        float xCoord = mapPosition.x * activeTerrain.terrainData.alphamapWidth;
        float zCoord = mapPosition.z * activeTerrain.terrainData.alphamapHeight;

        posX = (int)xCoord;
        posZ = (int)zCoord;
    }

    void CheckTexture()
    {
        float[,,] aMap = activeTerrain.terrainData.GetAlphamaps(posX, posZ, 1, 1);

        for (int i = 0; i < brush.Length; i++)
        {
            TextureValues[i] = aMap[0, 0, i];
        }
    }

    [ContextMenu("Play")]
    public void PlayFootstep()
    {
        if (activeTerrain != null)
        {
            GetTerrainTexture();
            AudioClip step = GetClip(brush[0].terrainAudios);
            float value = 0;

            for (int i = 0; i < TextureValues.Length; i++)
            {
                if (TextureValues[i] > 0)
                {
                    step = GetClip(brush[i].terrainAudios);
                    value = TextureValues[i];
                }
            }

            source.PlayOneShot(step, value);
        }
    }

    AudioClip GetClip(AudioClip[] clipArray)
    {
        int attempts = 3;
        AudioClip selectedClip = clipArray[Random.Range(0, clipArray.Length - 1)];

        while (selectedClip == previousClip && attempts > 0)
        {
            selectedClip = clipArray[Random.Range(0, clipArray.Length - 1)];

            attempts--;
        }

        previousClip = selectedClip;
        return selectedClip;
    }
}