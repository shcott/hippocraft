using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A simple class for useful math functions.
 */
public class MathHC {

	private MathHC() {}

	/**
	 * C# defines modulus for negative numbers differently than what is convenient. This is
	 * a more useful definition.
	 */
	public static int mod(int x, int m) {
		int r = x%m;
		return r<0 ? r+m : r;
	}
}
