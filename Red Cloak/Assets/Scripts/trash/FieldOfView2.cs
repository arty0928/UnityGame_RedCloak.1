using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FieldOfView2 : MonoBehaviour
{
	//raycast
	public bool isSeen2 = false;

	//public bool follow; //감지 했는지
	public float meshResolution;
	public int edgeResolveIterations;
	public float edgeDstThreshold;

	public MeshFilter viewMeshFilter;
	Mesh viewMesh;

	// 시야 영역의 반지름과 시야 각도
	public float viewRadius;
	[Range(0, 360)]
	public float viewAngle;

	// 마스크 2종
	public LayerMask targetMask, obstacleMask;

	// Target mask에 ray hit된 transform을 보관하는 리스트
	public List<Transform> visibleTargets = new List<Transform>();

	//public Transform player;
	/*private Transform player;*/

	/*public float playerDistance;
	public float awareAI = 5f;
	public float OriginalAIMoveSpeed;
	//public float SpeedUpAIMoveSpeed;
	public float damping = 6.0f;

	//enemy patrol, chase

	//public Transform[] navPoint;
	public Vector3[] navPoint;
	//private Transform navPos;
	public Vector3 navPos;

	public UnityEngine.AI.NavMeshAgent agent;

	public int destPoint = 0;
	private int randPos;
	public bool range;
*/

	void Start()
	{
		
			viewMesh = new Mesh();
			viewMesh.name = "View mesh";
			viewMeshFilter.mesh = viewMesh;

		//raycast
			//트랜스폼을 받아온다
			StartCoroutine(FindTargetsWithDelay(0.2f));
			GameManager.I.LevlSet = false;


	}

	void DrawFieldOfView()
	{
		int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
		float stepAngleSize = viewAngle / stepCount;
		List<Vector3> viewPoints = new List<Vector3>();
		ViewCastInfo oldViewCast = new ViewCastInfo();
		for (int i = 0; i <= stepCount; i++)
		{
			float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
			ViewCastInfo newViewCast = ViewCast(angle);

			if (i > 0)
			{
				bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
				if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
				{
					EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
					if (edge.pointA != Vector3.zero)
					{
						viewPoints.Add(edge.pointA);
					}
					if (edge.pointB != Vector3.zero)
					{
						viewPoints.Add(edge.pointB);
					}
				}

			}


			viewPoints.Add(newViewCast.point);
			oldViewCast = newViewCast;
		}

		int vertexCount = viewPoints.Count + 1;
		Vector3[] vertices = new Vector3[vertexCount];
		int[] triangles = new int[(vertexCount - 2) * 3];

		vertices[0] = Vector3.zero;
		for (int i = 0; i < vertexCount - 1; i++)
		{
			vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

			if (i < vertexCount - 2)
			{
				triangles[i * 3] = 0;
				triangles[i * 3 + 1] = i + 1;
				triangles[i * 3 + 2] = i + 2;
			}
		}

		viewMesh.Clear();

		viewMesh.vertices = vertices;
		viewMesh.triangles = triangles;
		viewMesh.RecalculateNormals();
	}
	EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
	{
		float minAngle = minViewCast.angle;
		float maxAngle = maxViewCast.angle;
		Vector3 minPoint = Vector3.zero;
		Vector3 maxPoint = Vector3.zero;

		for (var i = 0; i < edgeResolveIterations; i++)
		{
			float angle = (minAngle + maxAngle) / 2;
			ViewCastInfo newViewCast = ViewCast(angle);
			bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;


			if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
			{
				minAngle = angle;
				minPoint = newViewCast.point;
			}
			else
			{
				maxAngle = angle;
				maxPoint = newViewCast.point;

			}
		}
		return new EdgeInfo(minPoint, maxPoint);

	}
	public struct ViewCastInfo
	{
		public bool hit;
		public Vector3 point;
		public float dst;
		public float angle;

		public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
		{
			hit = _hit;
			point = _point;
			dst = _dst;
			angle = _angle;
		}


	}

	public struct EdgeInfo
	{
		public Vector3 pointA;
		public Vector3 pointB;

		public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
		{
			pointA = _pointA;
			pointB = _pointB;
		}

	}

	ViewCastInfo ViewCast(float globalAngle)
	{
		Vector3 dir = DirFromAngle(globalAngle, true);
		RaycastHit hit;

		if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
		{
			return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
		}
		else
		{
			return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
		}
	}

	IEnumerator FindTargetsWithDelay(float delay)
	{
		Debug.Log("FindTargetWithDelay");
		Debug.Log("isPlay: " + GameManager.I.isPlay);
		Debug.Log("isDead: " + GameManager.I.isDead);

		//if (GameManager.I.isPlay == true && GameManager.I.isDead==false)
		{
			while (true)
			{
				yield return new WaitForSeconds(delay);
				FindVisibleTargets();
			}
		}

	}

	void FindVisibleTargets()
	{
		visibleTargets.Clear();
		// viewRadius를 반지름으로 한 원 영역 내 targetMask 레이어인 콜라이더를 모두 가져옴
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

		for (int i = 0; i < targetsInViewRadius.Length; i++)
		{
			Transform target = targetsInViewRadius[i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;

			// 플레이어와 forward와 target이 이루는 각이 설정한 각도 내라면
			if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
			{
				float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

				// 타겟으로 가는 레이캐스트에 obstacleMask가 걸리지 않으면 visibleTargets에 Add
				if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
				{
					isSeen2 = true;
					visibleTargets.Add(target);
				}
				isSeen2 = false;

			}
			isSeen2 = false;

		}
	}

	// y축 오일러 각을 3차원 방향 벡터로 변환한다.
	// 원본과 구현이 살짝 다름에 주의. 결과는 같다.
	public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal)
		{
			angleDegrees += transform.eulerAngles.y;
		}

		return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
	}

	void LateUpdate()
	{
		DrawFieldOfView();

	}

}