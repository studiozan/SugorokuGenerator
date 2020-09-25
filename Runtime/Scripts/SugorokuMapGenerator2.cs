using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FieldGenerator;

namespace SugorokuGenerator
{
	[System.Serializable]
	public class SugorokuMapGenerator2
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
		}

		public IEnumerator SugorokuMapCreate(int maxNumSugorokuPoints)
		{
			lastInterruptionTime = System.DateTime.Now;

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

			while (sugorokuPoints.Count < maxNumSugorokuPoints)
			{
				yield return CoroutineUtility.CoroutineCycle(GenerateAisles(3));
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

		IEnumerator GenerateAisles(int numAisles)
		{
			int index = random.Next(extendablePoints.Count);

			for (int i0 = 0; i0 < numAisles; ++i0)
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

			GenerateRoom(index);
		}

		void GenerateRoom(int basePointIndex, int roomSize = 3)
		{
			int size = Mathf.Max(roomSize, 3);
			var roomPoints = new List<FieldConnectPoint>();
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

			bool foundCommonPoint = false;
			while (candidates.Count > 0 && foundCommonPoint == false)
			{
				int randomIndex = random.Next(candidates.Count);
				FieldConnectPoint nextPointSrc = candidates[randomIndex];
				List<FieldConnectPoint> connectedPoints2 = nextPointSrc.ConnectionList;

				for (int i0 = 0; i0 < connectedPoints.Count; ++i0)
				{
					FieldConnectPoint connectedPoint = connectedPoints[i0];
					if (connectedPoint.Index != nextPointSrc.Index)
					{
						for (int i1 = 0; i1 < connectedPoints2.Count; ++i1)
						{
							FieldConnectPoint connectedPoint2 = connectedPoints2[i1];
							if (connectedPoint.Index == connectedPoint2.Index && usedList[connectedPoint.Index] == false)
							{
								var sugorokuPoint = new FieldConnectPoint();
								sugorokuPoint.Initialize(nextPointSrc.Position, 0);
								sugorokuPoint.Index = nextPointSrc.Index;
								ConnectPoints(basePoint, sugorokuPoint);
								AddSugorokuPoint(sugorokuPoint, 1);
								roomPoints.Add(sugorokuPoint);

								var sugorokuPoint2 = new FieldConnectPoint();
								sugorokuPoint2.Initialize(connectedPoint.Position, 0);
								sugorokuPoint2.Index = connectedPoint.Index;
								ConnectPoints(basePoint, sugorokuPoint2);
								AddSugorokuPoint(sugorokuPoint2, 1);
								roomPoints.Add(sugorokuPoint2);

								ConnectPoints(sugorokuPoint, sugorokuPoint2);

								int baseSugorokPointIndex = sugorokuPoints.FindIndex(point => point.Index == basePoint.Index);
								masuTypes[baseSugorokPointIndex] = 1;

								extendablePoints.RemoveAt(basePointIndex);

								foundCommonPoint = true;
								break;
							}
						}
					}

					if (foundCommonPoint != false)
					{
						break;
					}
				}

				candidates.RemoveAt(randomIndex);
			}
		}

		void ConnectPoints(FieldConnectPoint point1, FieldConnectPoint point2)
		{
			point1.SetConnection(point2);
			point2.SetConnection(point1);
		}

		void AddSugorokuPoint(FieldConnectPoint sugorokuPoint, int masuType)
		{
			usedList[sugorokuPoint.Index] = true;
			extendablePoints.Add(sugorokuPoint);
			sugorokuPoints.Add(sugorokuPoint);
			masuTypes.Add(masuType);
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
			return null;
		}
		public void SetSerializedData( SugorokuSerializedData data)
		{
		}

		bool ShouldInterrupt()
		{
			return System.DateTime.Now.Subtract(lastInterruptionTime).TotalMilliseconds >= kElapsedTimeToInterrupt;
		}

		public static readonly float kElapsedTimeToInterrupt = 16.7f;

		System.DateTime lastInterruptionTime;
		System.Random random;
		List<FieldConnectPoint> allPoints;
		List<bool> usedList;
		List<FieldConnectPoint> sugorokuPoints;
		List<int> masuTypes;
		List<SugorokuPointData> pointDataList;
		List<FieldConnectPoint> extendablePoints;
		int startIndex;
	}
}
