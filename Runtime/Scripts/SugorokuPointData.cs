using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FieldGenerator;

namespace SugorokuGenerator
{
	[System.Serializable]
	public class SugorokuPointData
	{
		public void SetPosition( Vector3 pos)
		{
			position = pos;
		}
		public Vector3 GetPosition()
		{
			return position;
		}

		public void SetRoomType( int type)
		{
			roomType = type;
		}
		public int GetRoomType()
		{
			return roomType;
		}

		public void SetIndex( int idx)
		{
			index = idx;
		}
		public int GetIndex()
		{
			return index;
		}

		public void SetConnectionIndexList( List<int> list)
		{
			connectionIndexList = list;
		}
		public List<int> GetConnectionIndexList()
		{
			return connectionIndexList;
		}

		[SerializeField]
		Vector3 position;
		[SerializeField]
		int roomType;
		[SerializeField]
		int index;
		[SerializeField]
		List<int> connectionIndexList;
	}
}
