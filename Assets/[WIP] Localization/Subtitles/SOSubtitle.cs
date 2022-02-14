using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Subtitle Default", menuName = "Subtitle SO", order = 1)]
public class SOSubtitle : ScriptableObject
{
#if UNITY_EDITOR
	public AudioClip clip;
#endif
#if FMOD
	[FMODUnity.EventRef] public string soundEvent;
#else
	public string soundEvent;
#endif
	public string localizationTerm;
	public List<string> lines;
	public List<float> timestamps;
	public float length;
	public bool isDictaphone;
}