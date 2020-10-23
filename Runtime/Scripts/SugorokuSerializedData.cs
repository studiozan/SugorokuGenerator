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
		public void SetStartIndex( int index)
		{
			startIndex = index;
		}
		public int GetStartIndex()
		{
			return startIndex;
		}
		public void SetGoalIndex( int index)
		{
			goalIndex = index;
		}
		public int GetGoalIndex()
		{
			return goalIndex;
		}
		public void SetBiomeType( string type)
		{
			biomeType = type;
		}
		public string GetBiomeType()
		{
			return biomeType;
		}
		public void SetMapHide( bool flag)
		{
			mapHideFlag = flag;
		}
		public bool GetMapHide()
		{
			return mapHideFlag;
		}

		[SerializeField]
		int seedValue;
		[SerializeField]
		List<SugorokuPointData> pointDataList;
		[SerializeField]
		int startIndex;
		[SerializeField]
		int goalIndex;
		[SerializeField]
		string biomeType;
		[SerializeField]
		bool mapHideFlag;
		[SerializeField]
		public int lastTurn;
		[SerializeField]
		public string lastTurnEffect;
		[SerializeField]
		public int lastTurnParameter1;
		[SerializeField]
		public int lastTurnParameter2;
		[SerializeField]
		public int lastTurnParameter3;
	}
}
