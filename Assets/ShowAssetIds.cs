//Get instance, guid, and file ID of file during play mode
//Possibly useful for getting the type or instance of a specific file in the project folder and possibly loading
//This script prints these details for a selected asset in project window

using System.Text;
using UnityEngine;
using UnityEditor;

class ShowAssetIds
{
	[MenuItem("Assets/Show Asset Ids")]
	static void MenuShowIds()
	{
		var stringBuilder = new StringBuilder();

	foreach (var obj in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(Selection.activeObject)))
	{ 
		string guid;
		long file;

	if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out guid, out file))
	{
		stringBuilder.AppendFormat("Asset: " + obj.name +
			"\n  Instance ID: " + obj.GetInstanceID() +
			"\n  GUID: " + guid +
			"\n  File ID: " + file);
	}
	}

	Debug.Log(stringBuilder.ToString());
	}
}