using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Brushs", menuName = "TerrainBrushs", order = 1)]
public class BrushesNames : ScriptableObject
{
    [SerializeField] Brushes[] clips;

    public Brushes[] Clips { get => clips; }
}

[Serializable]
public struct Brushes
{
    public string Name;
    public AudioClip[] terrainAudios;
}
