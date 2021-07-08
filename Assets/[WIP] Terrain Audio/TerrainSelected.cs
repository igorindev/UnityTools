using UnityEngine;

[RequireComponent(typeof(TerrainBrushs))]
public class TerrainSelected : MonoBehaviour
{
    public Terrain terrain;
    public TerrainBrushs terrainBrushs;

    private void Reset()
    {
        terrainBrushs = GetComponent<TerrainBrushs>();
        terrain = GetComponent<Terrain>();
    }

    public void Set()
    {
        //Set Terrain as Active
        //PlayerManager.instance.PlayerMove.SetTerrain(terrain, terrainBrushs.brushs);
    }
}
