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

		/* 部屋かどうかの情報 */
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

		/* マスの種類 */
		public void SetMassType( int type)
		{
			massType = type;
		}
		public int GetMassType()
		{
			return massType;
		}
		/* マスの固有パラメータ */
		public void SetMassParam( float param)
		{
			massParam = param;
		}
		public float GetMassParam()
		{
			return massParam;
		}
		/* マスの使用回数 */
		public void SetMassUseNumber( int number)
		{
			massUseNumber = number;
		}
		public int GetMassUseNumber()
		{
			return massUseNumber;
		}
		/* マスの効果 */
		public void SetMassEffect( string effect)
		{
			massEffect = effect;
		}
		public string GetMassEffect()
		{
			return massEffect;
		}
		/* 敵のキーテーブル */
		public void SetEnemyKeys( string[] keys)
		{
			enemyKeys = keys;
		}
		public string[] GetEnemyKeys()
		{
			return enemyKeys;
		}
		/* アイテムのキーテーブル */
		public void SetItemKeys( string[] keys)
		{
			itemKeys = keys;
		}
		public string[] GetItemKeys()
		{
			return itemKeys;
		}
		/* アイテムの個数テーブル */
		public void SetItemCounts( long[] counts)
		{
			itemCounts = counts;
		}
		public long[] GetItemCounts()
		{
			return itemCounts;
		}
		/* アイテムの入手確率 */
		public void SetItemProbabilitys( float[] probabilitys)
		{
			itemProbabilitys = probabilitys;
		}
		public float[] GetItemProbabilitys()
		{
			return itemProbabilitys;
		}

		[SerializeField]
		Vector3 position;
		[SerializeField]
		int roomType;
		[SerializeField]
		int index;
		[SerializeField]
		List<int> connectionIndexList;
		[SerializeField]
		int massType;
		[SerializeField]
		float massParam;
		[SerializeField]
		int massUseNumber;
		[SerializeField]
		string massEffect;
		[SerializeField]
		string[] enemyKeys;
		[SerializeField]
		string[] itemKeys;
		[SerializeField]
		long[] itemCounts;
		[SerializeField]
		float[] itemProbabilitys;
	}
}
