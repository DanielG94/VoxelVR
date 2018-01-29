using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Made with help of: https://docs.unity3d.com/ScriptReference/Mesh.CombineMeshes.html
public class MeshCombiner : MonoBehaviour {

	private static List<Color> usedColors = new List<Color>();	// Contains all used colors to determine if a container exists
	private static List<GameObject> containerList = new List<GameObject>(); // List of all containers
	private static List<CombineInstance> combine = new List<CombineInstance>(); // List of single combine instances that will be merged

	public void CombineMeshes() {
		Transform[] childObjects = GetComponentsInChildren<Transform>();
		Mesh finalmesh = new Mesh();


		Debug.Log("Trying to combine " + childObjects.Length + " meshes");
		int counter = 0;
		foreach (Transform child in childObjects) {

			if (child.transform == transform || child.tag != "Cube"  || containerList.Contains(child.gameObject)) {
				//Debug.Log("Skipping " + (child.transform == transform) + " " + (child.tag != "Cube") + " " + containerList.Contains(child.gameObject));
				continue; // Skip the object that is collecting all meshes or cubes that shouldnt be combined
			} 
				

			Color cubeColor = child.GetComponent<Renderer>().material.color;
			string cubeColorString = ColorUtility.ToHtmlStringRGBA(cubeColor);

			MeshFilter meshFilter = child.GetComponent<MeshFilter>();
			CombineInstance singleCombine = new CombineInstance();
			


			GameObject container;
			//Check if color got combined before
			if (!usedColors.Contains(cubeColor)) {
				Debug.Log("New color detected: " + cubeColorString);
				usedColors.Add(cubeColor);
				// Create new container because color wasnt used
				container = CreateNewContainer(cubeColorString, cubeColor);
			} else {
				// Try to find container that should exist
				container = containerList.FindLast(x => x.name == cubeColorString);
				if (container != null) {
					Debug.Log(counter + " Trying to find: " + cubeColorString + " and found: " + container.name);
					if (counter == 0) {
						Debug.Log("Exisiting container found, trying to combine old mesh " + container.GetComponent<MeshFilter>().sharedMesh.vertexCount);
						singleCombine.subMeshIndex = 0;
						singleCombine.mesh = container.GetComponent<MeshFilter>().sharedMesh;
						singleCombine.transform = container.transform.localToWorldMatrix;
						combine.Add(singleCombine);
						//finalmesh.CombineMeshes(combine.ToArray());
						//combine.Clear();
					}
				} else {
					//Fallback - create new container because it wasnt found for some reason
					Debug.LogError(counter + " Trying to find: " + cubeColorString + " and container didnt exist! Trying to create new container");
					container = CreateNewContainer(cubeColorString, cubeColor);
				}
				
				
			}

			


			singleCombine.subMeshIndex = 0;
			singleCombine.mesh = meshFilter.sharedMesh; // Take mesh of current block
			singleCombine.transform = child.transform.localToWorldMatrix; // and position
			combine.Add(singleCombine); // Add to combine array to merge it

			//Remove child because it isnt needed anymore
			child.gameObject.SetActive(false);
			Destroy(child.gameObject);


			//Debug.Log("Finalmesh vertex count: " + finalmesh.vertexCount);
			//Prevent Overflow of verticies
			if (finalmesh.vertexCount<=65000)
				finalmesh.CombineMeshes(combine.ToArray()); // If its within the limit just continue normally
			else {
				Debug.LogWarning("Vertexlimit reached: " + finalmesh.vertexCount);
				finalmesh.CombineMeshes(combine.ToArray());
				//When the limit is hit combine what is currently selected and create a new mesh and container for a new group
				container.GetComponent<MeshFilter>().sharedMesh = null;
				container.GetComponent<MeshFilter>().sharedMesh = finalmesh;
				finalmesh = new Mesh();
				container = CreateNewContainer(cubeColorString, cubeColor);
			}



			container.GetComponent<MeshFilter>().sharedMesh = null;
			container.GetComponent<MeshFilter>().sharedMesh = finalmesh;
			finalmesh = new Mesh();

			counter++;
		}

		combine.Clear();

		Debug.Log("Combined " + counter + " meshes" + " with " + containerList[0].GetComponent<MeshFilter>().sharedMesh.vertexCount + " verticies");
		counter = 0;
		
		//Add colliders to the new container
		foreach(GameObject container in containerList) {
			Debug.Log("Container: " + container.name);
			if(container.GetComponent<MeshCollider>() == null) {
				container.AddComponent<MeshCollider>();
			} else {
				Destroy(container.GetComponent<MeshCollider>());
				container.AddComponent<MeshCollider>();
			}
		}




	}

	private GameObject CreateNewContainer(string name, Color color) {

		GameObject container = new GameObject();
		container.name = name;
		container.tag = "Cube";

		container.transform.SetParent(transform);
		container.AddComponent<MeshFilter>();
		container.AddComponent<MeshRenderer>();
		container.GetComponent<Renderer>().material = Resources.Load("CubeMat") as Material;
		container.GetComponent<Renderer>().material.color = color;
		containerList.Add(container);
		Debug.Log("Created new container: " + container.name);
		combine.Clear();
		return container;
	}


	public static void AdjustContainerName() {
		foreach(GameObject container in containerList) {
			if(container.name != ColorUtility.ToHtmlStringRGBA(container.GetComponent<Renderer>().material.color)) {
				Color oldColor;
				ColorUtility.TryParseHtmlString(container.name, out oldColor);
				usedColors.Remove(oldColor);
				usedColors.Add(container.GetComponent<Renderer>().material.color);
				container.name = ColorUtility.ToHtmlStringRGBA(container.GetComponent<Renderer>().material.color);
			}
			
			
		}
	}
}
