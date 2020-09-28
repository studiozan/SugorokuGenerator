using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SugorokuGenerator
{
	[System.Serializable]
	public class SugorokuMapParameter
	{
		[SerializeField]
		public int sugorokuSize = default;
		[SerializeField]
		public int minAisleSize = default;
		[SerializeField]
		public int maxAisleSize = default;
		[SerializeField]
		public int minRoomSize = default;
		public int maxRoomSize = default;
	}
}
