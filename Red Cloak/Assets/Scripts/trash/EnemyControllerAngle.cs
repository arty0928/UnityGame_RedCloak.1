using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class EnemyControllerAngle : MonoBehaviour
{
	//raycast
	//public bool follow; //���� �ߴ���
	public float meshResolution;
	//public MeshFilter viewMeshFilter;
	Mesh viewMesh;

	// �þ� ������ �������� �þ� ����
	public float viewRadius;
	[Range(0, 360)]
	public float viewAngle;

	// ����ũ 2��
	public LayerMask targetMask, obstacleMask;

	// Target mask�� ray hit�� transform�� �����ϴ� ����Ʈ
	public List<Transform> visibleTargets = new List<Transform>();

	//public Transform player;
	private Transform player;

	public float playerDistance;
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


	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;

		//if (GameManager.I.isPlay == true && GameManager.I.isDead == false && GameManager.I.LevlSet == true)
		{

			//enemyHealth = 100;
			UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

			Debug.Log("EnemyControllerAngle Start()");
			//GameManager.I.isPlay = true;
			Debug.Log("isPlay: " + GameManager.I.isPlay);
			Debug.Log("isDead: " + GameManager.I.isDead);

			//ó�� ������ ����

			/*var items = GameObject.FindGameObjectsWithTag("LunchToPut").Select(ItemToPut => ItemToPut.transform.position).ToArray();
			items = items.OrderBy(item => Random.Range(-1.0f, 1.0f)).ToArray();
			itemPos = items[0];
			Debug.Log("ItemToput");
			target = Instantiate(ItemPrefab, new Vector3(itemPos.x, itemPos.y, itemPos.z), transform.rotation).transform;
*/
			var navPoints = GameObject.FindGameObjectsWithTag("patrolPoint").Select(ItemToPut => ItemToPut.transform.position).ToArray();
			navPoints = navPoints.OrderBy(navPoint => Random.Range(-1.0f, 1.0f)).ToArray();


			for (var i = 0; i < navPoints.Length; i++)
			{
				//Debug.Log("navPoints"+i + "EnemyControllerAngle" +navPoints[i]);
				navPoint[i] = navPoints[i];
			}


			randPos = Random.Range(0, navPoint.Length + 1);
			destPoint = (randPos) % navPoint.Length;
			agent.destination = navPoint[destPoint];


			agent.autoBraking = false;

			//raycast
			//Ʈ�������� �޾ƿ´�
			StartCoroutine(FindTargetsWithDelay(0.2f));
			GameManager.I.LevlSet = false;

			Debug.Log("EnemyControllerAngle Start() after LevelSet");
			Debug.Log("isPlay: " + GameManager.I.isPlay);
			Debug.Log("isDead: " + GameManager.I.isDead);


		}

	}

	void DrawFieldOfView()
	{
		int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
		float stepAngleSize = viewAngle / stepCount;
		List<Vector3> viewPoints = new List<Vector3>();

		for (int i = 0; i <= stepCount; i++)
		{
			float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
			//Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * viewAngle, Color.red);
			ViewCastInfo newViewCast = ViewCast(angle);
			viewPoints.Add(newViewCast.point);
		}

		int vertexCount = viewPoints.Count + 1;
		Vector3[] vertices = new Vector3[vertexCount];
		int[] triangles = new int[(vertexCount - 2) * 3];

		vertices[0] = Vector3.zero;
		for (int i = 0; i < vertexCount - 1; i++)
		{
			vertices[i + 1] = viewPoints[i];

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
	void EnemyMove()
	{
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
		// viewRadius�� ���������� �� �� ���� �� targetMask ���̾��� �ݶ��̴��� ��� ������
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

		for (int i = 0; i < targetsInViewRadius.Length; i++)
		{
			Transform target = targetsInViewRadius[i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;

			// �÷��̾�� forward�� target�� �̷�� ���� ������ ���� �����
			if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
			{
				float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

				// Ÿ������ ���� ����ĳ��Ʈ�� obstacleMask�� �ɸ��� ������ visibleTargets�� Add
				if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
				{

					Chase();
					visibleTargets.Add(target);
				}

			}
			/*if (transform.CompareTag("EnemySpeedUp"))
			//if (transform.tag == "EnemySpeedUp")
			{
				agent.speed = 3;
			}
*/
		}
	}

	// y�� ���Ϸ� ���� 3���� ���� ���ͷ� ��ȯ�Ѵ�.
	// ������ ������ ��¦ �ٸ��� ����. ����� ����.
	public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal)
		{
			angleDegrees += transform.eulerAngles.y;
		}

		return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
	}

	void Update()
	{
		//DrawFieldOfView();

	}

	void FixedUpdate()
	{


		float distanceFormGoal = Vector3.Distance(transform.position, agent.destination);

		//Debug.Log(distanceFormGoal + "�̰���" + 0.1f);

		//if (distanceFormGoal < 0.1f)
		if (agent.remainingDistance < 0.5f)
		{
			//Debug.Log(distanceFormGoal + "GotoNext");
			GotoNextPoint();
			/*checkPos = true;
			StartCoroutine(WaitForIt());*/
		}

	}

	/*IEnumerator WaitForIt()
    {
		yield return new WaitForSeconds(2.0f);
		checkPos = false;
    }*/

	void LookAtPlayer()
	{
		transform.LookAt(player);
	}


	void GotoNextPoint()
	{
		if (navPoint.Length == 0)
			return;

		agent.destination = navPoint[destPoint];

		//int xcount = Random.Range(1, 6);
		//int randPos = Random.Range(0, navPoint.Length + 1);
		//destPoint = (randPos) % navPoint.Length;

		randPos = Random.Range(0, navPoint.Length + 1);
		destPoint = (randPos) % navPoint.Length;
		agent.destination = navPoint[destPoint];

	}

	void Chase()
	{
		agent.destination = player.position;
		//transform.Translate(player.position * AIMoveSpeed * Time.deltaTime);
	}

}