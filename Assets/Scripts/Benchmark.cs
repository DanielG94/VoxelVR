using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class Benchmark : MonoBehaviour {
	public static int AddedCubeAmount = 1;
	public static int SpaceBetweenCubes = 1;
	private static int _xAmount, _yAmount, _zAmount = 0;
	private static List<GameObject> _childBlocks = new List<GameObject>();
	private static bool _isRunning = false;

	// Debug stuff for csv export
	private static VRTK_FramesPerSecondViewer _fpsCounter;
	private static List<string[]> rowData = new List<string[]>();


	private void Start() {
		_fpsCounter = GameObject.Find("FramesPerSecondText").GetComponent<VRTK_FramesPerSecondViewer>();
		string[] _column = new string[2];
		_column[1] = "FPS";
		_column[0] = "Cubes";
		rowData.Add(_column);
	}

	public void SpawnCubes() {
		if (_isRunning) return;

		// Add "AddCubeAmount" to each direction to spawn (AddedCubeAMount * n)^3 cubes on each call
		_xAmount += AddedCubeAmount;
		_yAmount += AddedCubeAmount;
		_zAmount += AddedCubeAmount;

		StartCoroutine("SpawnCube");
	}

	IEnumerator SpawnCube() {
		_isRunning = true;
		Debug.Log("Starting spawning | amount: " + (_zAmount));
		for (int z = 0; z < _zAmount; z++) {
			for (int y = 0; y < _yAmount; y++) {	
				for (int x = 0; x < _xAmount; x++) {
					//Skip exisiting blocks and continue spawning next to them
					if(z > _zAmount - AddedCubeAmount - 1 || y > _yAmount - AddedCubeAmount - 1 || x > _xAmount - AddedCubeAmount -1 ) {
						_childBlocks.Add(Instantiate(Resources.Load("cube"), new Vector3(x + (SpaceBetweenCubes * x), y + 2 + (SpaceBetweenCubes * y), z + (SpaceBetweenCubes * z)), Quaternion.identity) as GameObject);
						_childBlocks[_childBlocks.Count - 1].transform.SetParent(transform);
					}
				}
			}
		}
		yield return null;

		//Combine all cube meshes after they spawned to improve performance
		GetComponent<MeshCombiner>().CombineMeshes();
		Debug.LogWarning("Currently " + _childBlocks.Count + " spawned blocks ");
		GetComponentInChildren<Text>().text = "Cubes: " + _childBlocks.Count;

		yield return new WaitForSeconds(1);

		// After 1 second save the fps and block count and write it to a csv file
		string[] _column = new string[2];
		_column[1] = _fpsCounter.GetFPS();
		_column[0] = _childBlocks.Count.ToString();
		//Debug.Log(_column[0] + " : " + _column[1]);
		rowData.Add(_column);
		WriteCSV();
		_isRunning = false;
	}


	// Code by https://sushanta1991.blogspot.de/2015/02/how-to-write-data-to-csv-file-in-unity.html
	void WriteCSV() {


		string[][] output = new string[rowData.Count][];

		for (int i = 0; i < output.Length; i++) {
			output[i] = rowData[i];
		}

		int length = output.GetLength(0);
		string delimiter = ";";

		StringBuilder sb = new StringBuilder();

		for (int index = 0; index < length; index++)
			sb.AppendLine(string.Join(delimiter, output[index]));

		string path = Application.dataPath + "/CSV/";

		if (!Directory.Exists(path))
			Directory.CreateDirectory(path);

		string filePath = path + "benchmark.csv";

		StreamWriter outStream = System.IO.File.CreateText(filePath);
		outStream.WriteLine(sb);
		outStream.Close();

		Debug.Log("Wrote file to " + filePath);
	}
}
