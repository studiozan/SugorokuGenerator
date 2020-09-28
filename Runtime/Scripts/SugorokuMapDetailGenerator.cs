using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FieldGenerator;

namespace SugorokuGenerator
{
	[System.Serializable]
	public class SugorokuMapDetailGenerator
	{
		public void Initialize(int seed = 0)
		{
			random = new System.Random(seed);
			allPoints = new List<FieldConnectPoint>();
			usedList = new List<bool>();
			sugorokuPoints = new List<FieldConnectPoint>();
			masuTypes = new List<int>();
			pointDataList = new List<SugorokuPointData>();
			extendablePoints = new List<FieldConnectPoint>();
			startIndex = 0;
			goalIndex = -1;
		}

		public IEnumerator SugorokuMapCreate(SugorokuMapParameter parameter)
		{
			lastInterruptionTime = System.DateTime.Now;

			this.parameter = parameter;

			sugorokuPoints.Clear();
			masuTypes.Clear();
			pointDataList.Clear();
			extendablePoints.Clear();

			var startPoint = new FieldConnectPoint();
			startPoint.Initialize(allPoints[startIndex].Position, 0);
			startPoint.Index = startIndex;

			extendablePoints.Add(startPoint);
			sugorokuPoints.Add(startPoint);
			masuTypes.Add(0);
			usedList[startIndex] = true;

			int sugorokuSize = parameter.sugorokuSize;
			int minAisleSize = parameter.minAisleSize;
			int maxAisleSize = parameter.maxAisleSize;
			while (sugorokuPoints.Count < sugorokuSize)
			{
				int aisleSize = random.Next(minAisleSize, maxAisleSize + 1);
				yield return CoroutineUtility.CoroutineCycle(GenerateAisles(aisleSize));
			}

			for (int i0 = 0; i0 < sugorokuPoints.Count; ++i0)
			{
				FieldConnectPoint sugorokuPoint = sugorokuPoints[i0];
				var data = new SugorokuPointData();
				data.SetPosition(sugorokuPoint.Position);
				data.SetRoomType(masuTypes[i0]);
				data.SetIndex(sugorokuPoint.Index);
				var indexList = new List<int>();
				for(int i1 = 0; i1 < sugorokuPoint.ConnectionList.Count; ++i1)
				{
					indexList.Add(sugorokuPoint.ConnectionList[i1].Index);
				}
				data.SetConnectionIndexList(indexList);
				data.SetMassType(0);
				data.SetMassParam(0f);
				pointDataList.Add(data);
			}
		}

		IEnumerator GenerateAisles(int aisleSize)
		{
			int index = random.Next(extendablePoints.Count);

			for (int i0 = 0; i0 < aisleSize && sugorokuPoints.Count < parameter.sugorokuSize; ++i0)
			{
				FieldConnectPoint extendablePoint = extendablePoints[index];
				FieldConnectPoint point = allPoints[extendablePoint.Index];
				List<FieldConnectPoint> connectedPoints = point.ConnectionList;
				if (connectedPoints.Count - extendablePoint.ConnectionList.Count > 0)
				{
					var candidates = new List<FieldConnectPoint>();
					for (int i1 = 0; i1 < connectedPoints.Count; ++i1)
					{
						FieldConnectPoint connectedPoint = connectedPoints[i1];
						if (usedList[connectedPoint.Index] == false)
						{
							candidates.Add(connectedPoint);
						}
					}

					if (candidates.Count > 0)
					{
						FieldConnectPoint nextPointSrc = candidates[random.Next(candidates.Count)];
						var sugorokuPoint = new FieldConnectPoint();
						sugorokuPoint.Initialize(nextPointSrc.Position, 0);
						sugorokuPoint.Index = nextPointSrc.Index;
						ConnectPoints(extendablePoint, sugorokuPoint);
						AddSugorokuPoint(sugorokuPoint, 0);

						index = extendablePoints.Count - 1;
					}
				}

				if (ShouldInterrupt() != false)
				{
					yield return null;
					lastInterruptionTime = System.DateTime.Now;
				}
			}

			int roomSize = random.Next(parameter.minRoomSize, parameter.maxRoomSize + 1);
			GenerateRoom(index, roomSize);
		}

		void GenerateRoom(int basePointIndex, int roomSize = 3)
		{
			var roomPoints = new List<FieldConnectPoint>();
			bool extendable = false;

			if (sugorokuPoints.Count <= parameter.sugorokuSize - 2)
			{
				FieldConnectPoint basePoint = extendablePoints[basePointIndex];
				roomPoints.Add(basePoint);
				FieldConnectPoint basePointSrc = allPoints[basePoint.Index];
				List<FieldConnectPoint> connectedPoints = basePointSrc.ConnectionList;
				var candidates = new List<FieldConnectPoint>();
				for (int i0 = 0; i0 < connectedPoints.Count; ++i0)
				{
					FieldConnectPoint connectedPoint = connectedPoints[i0];
					if (usedList[connectedPoint.Index] == false)
					{
						candidates.Add(connectedPoint);
					}
				}

				while (candidates.Count > 0)
				{
					int randomIndex = random.Next(candidates.Count);
					FieldConnectPoint nextPointSrc = candidates[randomIndex];

					if (TryGetCommonPoint(basePointSrc, nextPointSrc, out FieldConnectPoint commonPoint) != false)
					{
						var sugorokuPoint = new FieldConnectPoint();
						sugorokuPoint.Initialize(nextPointSrc.Position, 0);
						sugorokuPoint.Index = nextPointSrc.Index;
						ConnectPoints(basePoint, sugorokuPoint);
						AddSugorokuPoint(sugorokuPoint, 1);
						roomPoints.Add(sugorokuPoint);

						var sugorokuPoint2 = new FieldConnectPoint();
						sugorokuPoint2.Initialize(commonPoint.Position, 0);
						sugorokuPoint2.Index = commonPoint.Index;
						ConnectPoints(basePoint, sugorokuPoint2);
						AddSugorokuPoint(sugorokuPoint2, 1);
						roomPoints.Add(sugorokuPoint2);

						ConnectPoints(sugorokuPoint, sugorokuPoint2);

						int baseSugorokPointIndex = sugorokuPoints.FindIndex(point => point.Index == basePoint.Index);
						masuTypes[baseSugorokPointIndex] = 1;

						extendable = true;
						break;
					}
					else
					{
						candidates.RemoveAt(randomIndex);
					}
				}
			}

			while (roomPoints.Count < roomSize && extendable != false && sugorokuPoints.Count < parameter.sugorokuSize)
			{
				bool foundNewRoomPoint = false;
				for (int i0 = 0; i0 < roomPoints.Count; ++i0)
				{
					FieldConnectPoint roomPoint1 = roomPoints[i0];
					FieldConnectPoint point1 = allPoints[roomPoint1.Index];
					for (int i1 = 0; i1 < roomPoints.Count; ++i1)
					{
						if (i1 != i0)
						{
							FieldConnectPoint roomPoint2 = roomPoints[i1];
							FieldConnectPoint point2 = allPoints[roomPoint2.Index];
							if (TryGetCommonPoint(point1, point2, out FieldConnectPoint commonPoint) != false)
							{
								var sugorokuPoint = new FieldConnectPoint();
								sugorokuPoint.Initialize(commonPoint.Position, 0);
								sugorokuPoint.Index = commonPoint.Index;
								ConnectPoints(roomPoint1, sugorokuPoint);
								ConnectPoints(roomPoint2, sugorokuPoint);
								AddSugorokuPoint(sugorokuPoint, 1);
								roomPoints.Add(sugorokuPoint);

								foundNewRoomPoint = true;
								break;
							}
						}
					}

					if (foundNewRoomPoint != false)
					{
						break;
					}
				}

				extendable = foundNewRoomPoint;
			}
		}

		bool TryGetCommonPoint(FieldConnectPoint point1, FieldConnectPoint point2, out FieldConnectPoint commonPoint)
		{
			bool foundCommonPoint = false;
			commonPoint = null;

			List<FieldConnectPoint> connectedPoints = point1.ConnectionList;
			List<FieldConnectPoint> connectedPoints2 = point2.ConnectionList;
			FieldConnectPoint basePoint = sugorokuPoints[sugorokuPoints.FindIndex(p => p.Index == point1.Index)];

			for (int i0 = 0; i0 < connectedPoints.Count; ++i0)
			{
				FieldConnectPoint connectedPoint = connectedPoints[i0];
				if (connectedPoint.Index != point2.Index)
				{
					for (int i1 = 0; i1 < connectedPoints2.Count; ++i1)
					{
						FieldConnectPoint connectedPoint2 = connectedPoints2[i1];
						if (connectedPoint.Index == connectedPoint2.Index && usedList[connectedPoint.Index] == false)
						{
							foundCommonPoint = true;
							commonPoint = connectedPoint;
							break;
						}
					}
				}

				if (foundCommonPoint != false)
				{
					break;
				}
			}

			return foundCommonPoint;
		}

		void ConnectPoints(FieldConnectPoint point1, FieldConnectPoint point2)
		{
			point1.SetConnection(point2);
			point2.SetConnection(point1);
		}

		void AddSugorokuPoint(FieldConnectPoint sugorokuPoint, int masuType)
		{
			usedList[sugorokuPoint.Index] = true;
			sugorokuPoints.Add(sugorokuPoint);
			masuTypes.Add(masuType);

			if (IsExtendablePoint(sugorokuPoint) != false)
			{
				extendablePoints.Add(sugorokuPoint);
			}

			List<FieldConnectPoint> connectedPoints = allPoints[sugorokuPoint.Index].ConnectionList;
			for (int i0 = 0; i0 < connectedPoints.Count; ++i0)
			{
				FieldConnectPoint connectedPoint = connectedPoints[i0];
				int index = extendablePoints.FindIndex(point => point.Index == connectedPoint.Index);
				if (index > 0)
				{
					if (IsExtendablePoint(extendablePoints[index]) == false)
					{
						extendablePoints.RemoveAt(index);
					}
				}
			}
		}

		bool IsExtendablePoint(FieldConnectPoint point)
		{
			bool extendable = false;

			List<FieldConnectPoint> connectedPoints = allPoints[point.Index].ConnectionList;
			for (int i0 = 0; i0 < connectedPoints.Count; ++i0)
			{
				if (usedList[connectedPoints[i0].Index] == false)
				{
					extendable = true;
					break;
				}
			}

			return extendable;
		}

		public void SetPointList(List<FieldConnectPoint> points)
		{
			allPoints = points;
			for (int i0 = 0; i0 < allPoints.Count; ++i0)
			{
				allPoints[i0].Index = i0;
				usedList.Add(false);
			}
		}

		public List<FieldConnectPoint> GetPointList()
		{
			return sugorokuPoints;
		}

		public List<FieldConnectPoint> GetRoadPointList()
		{
			return allPoints;
		}

		public List<int> GetSugorokuDataList()
		{
			return masuTypes;
		}

		public List<SugorokuPointData> GetSugorokuPointDataList()
		{
			return pointDataList;
		}

		public SugorokuSerializedData GetSerializedData()
		{
			var dataList = new List<SugorokuPointData>();
			var serializedData = new SugorokuSerializedData();

			for(int i0 = 0; i0 < sugorokuPoints.Count; ++i0)
			{
				var data = new SugorokuPointData();
				data.SetPosition(sugorokuPoints[i0].Position);
				data.SetRoomType(masuTypes[i0]);
				data.SetIndex(sugorokuPoints[i0].Index);
				var indexList = new List<int>();
				for(int i1 = 0; i1 < sugorokuPoints[i0].ConnectionList.Count; ++i1)
				{
					indexList.Add(sugorokuPoints[i0].ConnectionList[i1].Index);
				}
				data.SetConnectionIndexList(indexList);
				dataList.Add(data);
			}
			serializedData.SetPointDataList(dataList);

			serializedData.SetStartIndex(startIndex);
			serializedData.SetGoalIndex(goalIndex);
			pointDataList.AddRange(dataList);

			return serializedData;
		}

		public void SetSerializedData(SugorokuSerializedData data)
		{
			pointDataList = data.GetPointDataList();

			sugorokuPoints.Clear();
			masuTypes.Clear();

			for(int i0 = 0; i0 < pointDataList.Count; ++i0)
			{
				var point = new FieldConnectPoint();
				SugorokuPointData pointData = pointDataList[i0];
				point.Initialize(pointData.GetPosition(), PointType.kGridRoad);
				point.Index = pointData.GetIndex();
				masuTypes.Add(pointData.GetRoomType());
				sugorokuPoints.Add(point);
			}

			for(int i0 = 0; i0 < pointDataList.Count; ++i0)
			{
				var indexList = pointDataList[i0].GetConnectionIndexList();
				for(int i1 = 0; i1 < indexList.Count; ++i1)
				{
					sugorokuPoints[i0].SetConnection(sugorokuPoints[indexList[i1]]);
				}
			}

			startIndex = data.GetStartIndex();
			goalIndex = data.GetGoalIndex();

			if(allPoints == null)
			{
				allPoints = new List<FieldConnectPoint>();
			}
			allPoints.Clear();
			allPoints.AddRange(sugorokuPoints);
		}

		bool ShouldInterrupt()
		{
			return System.DateTime.Now.Subtract(lastInterruptionTime).TotalMilliseconds >= kElapsedTimeToInterrupt;
		}

		public static readonly float kElapsedTimeToInterrupt = 16.7f;

		System.DateTime lastInterruptionTime;
		System.Random random;
		SugorokuMapParameter parameter;
		List<FieldConnectPoint> allPoints;
		List<bool> usedList;
		List<FieldConnectPoint> sugorokuPoints;
		List<int> masuTypes;
		List<SugorokuPointData> pointDataList;
		List<FieldConnectPoint> extendablePoints;
		int startIndex;
		int goalIndex;
	}
}
