using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard{

	public delegate void GridsizeChangedHandler();
	public static event GridsizeChangedHandler OnGridsizeChanged;
	public static void DoGridsizeChanged() {
		if (OnGridsizeChanged != null) OnGridsizeChanged();
	}
}
