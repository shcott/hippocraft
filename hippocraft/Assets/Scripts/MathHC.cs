using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * MathHC.
 * 
 * A simple class for useful math functions.
 */
public class MathHC {

	private MathHC() {}

	public static int FloorDivision(int num, int den) {
		return Mathf.FloorToInt((float)num / den);
		//return num / den;
	}

	/*
	 * C# defines modulus for negative numbers differently than what is convenient. This is
	 * a more useful definition.
	 */
	public static int Mod(int x, int m) {
		int r = x%m;
		return r<0 ? r+m : r;
	}

	/*
	 * A linear interpolation helper function.
	 */
	public static float Lerp(float val1, float val2, float t) {
		return val1 * (1.0f-t) + val2 * t;
	}
}
