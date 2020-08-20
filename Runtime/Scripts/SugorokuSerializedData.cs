using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FieldGenerator;

namespace SugorokuGenerator
{
	[System.Serializable]
	public class SugorokuSerializedData
	{

		public void SetSeed( int seed)
		{
			seedValue = seed;
		}
		public int GetSeed()
		{
			return seedValue;
		}

		public void SetPointDataList( List<SugorokuPointData> list)
		{
			pointDataList = list;
		}
		public List<SugorokuPointData> GetPointDataList()
		{
			return pointDataList;
		}

		[SerializeField]
		int seedValue;
		[SerializeField]
		List<SugorokuPointData> pointDataList;
	}
}
