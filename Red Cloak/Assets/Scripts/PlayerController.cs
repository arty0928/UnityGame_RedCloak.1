using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    //트랜스폼을 담을 변수
    //public Rigidbody m_ro;
    public Transform m_tr;

    //레이 길이를 지정할 변수
    public float distance = 10.0f;

    //충돌 정보를 가져올 레이케스트 히트
    public RaycastHit hit;

    //레이어 마스크를 지정할 변수\
    //-1: 모든 obj
    public LayerMask m_layerMask = -1;

    //충돌 정보를 여러개 담을 레이캐스트 히트 배열
    public RaycastHit[] hits;
    public int checkWall = 0;


    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private FixedJoystick _joystick;
    [SerializeField] private Animator _animator;

    [SerializeField] private float _moveSpeed;
    
    //스테이지 단계 
    public int stageNum;

    //아이템 먹기
    //public Transform item;
    public int lunchitem =0;
    public int timeItem;
    public int saveItem;

    public int maxItem;
    public int maxTimeItem;
    public int maxSaveItem;

    //아이템 먹으면 숫자 증가
    Text lunchCount;
    public static int lunchAmount;
    
    public Transform[] ItemPoint;

    //Audio
    public AudioSource CoinSound;


    Vector3 LookDir;
    private bool isWall = false;
    
    private void Start()
    {
        //var enemies = GameObject.FindGameObjectsWithTag("Enemy").Select(enemy => enemy.transform.position).ToList();
        //lunchitem = GameManager.I.stage * 2;

        //raycast
        //트랜스폼을 받아온다
        m_tr = GetComponent<Transform>();

    }

    /*private void FixedUpdate()
    { _rigidbody.velocity = new Vector3(_joystick.Horizontal * _moveSpeed, 0, _joystick.Vertical * _moveSpeed);
    }*/

    private void LateUpdate()
    {
        LookDir = _joystick.Vertical * Vector3.forward + _joystick.Horizontal * Vector3.right;
        transform.rotation = Quaternion.LookRotation(LookDir);

        
        Ray ray = new Ray();

        //시작점 세팅
        ray.origin = m_tr.position;

        //방향 설정
        ray.direction = m_tr.forward;

        //사용 방법

        Vector3 rayforward = new Vector3(0,0,1);
        Vector3 rayright = new Vector3(1, 0, 0);

        Vector3 rayleft = new Vector3(-1, 0, 0);

        Vector3 rayback = new Vector3(0, 0, -1);


        //forward
        if (Physics.Raycast(transform.position, rayforward, out hit, 0.5f))
        {
            if (hit.transform.tag == "Wall")
            {
                checkWall = 1;
                isWall = true;
                Debug.DrawLine(m_tr.position, m_tr.position + m_tr.forward * hit.distance, Color.red);
                PlayerMove();

            }

        }

        //right
        else if (Physics.Raycast(transform.position, rayright, out hit, 0.5f))
        {
            if (hit.transform.tag == "Wall")
            {
                checkWall = 2;
                isWall = true;
                Debug.DrawLine(m_tr.position, m_tr.position + m_tr.forward * hit.distance, Color.red);
                PlayerMove();

            }
        }
        //back
        else if (Physics.Raycast(transform.position, rayback, out hit, 0.5f))
        {
            if (hit.transform.tag == "Wall")
            {
                checkWall = 3;
                isWall = true;
                Debug.DrawLine(m_tr.position, m_tr.position + m_tr.forward * hit.distance, Color.red);
                PlayerMove();

            }
        }

        //left
        else if (Physics.Raycast(transform.position,rayleft, out hit, 0.5f))
        {
            if (hit.transform.tag == "Wall")
            {
                checkWall = 4;
                isWall = true;
                Debug.DrawLine(m_tr.position, m_tr.position + m_tr.forward * hit.distance, Color.red);
                PlayerMove();

            }
        }

        

        else 
        {
            checkWall = 0;
            isWall = false;
            PlayerMove();

        }
    }

    private void PlayerMove()
    {
        if (checkWall == 1 && isWall == true)
        {
            Debug.Log("1");
            _rigidbody.velocity = new Vector3(_joystick.Horizontal  * _moveSpeed, 0, 0 * _moveSpeed);

        }
        else if(checkWall == 2 && isWall == true)
        {
            Debug.Log("2");
            _rigidbody.velocity = new Vector3(0 * _moveSpeed, 0, _joystick.Vertical * _moveSpeed);
        }
        else if(checkWall == 3 && isWall == true )
        {
            Debug.Log("3");
            _rigidbody.velocity = new Vector3(_joystick.Horizontal  * _moveSpeed, 0, 0 * _moveSpeed);
        }
        else if(checkWall == 4 && isWall == true)
        {
            Debug.Log("4");
            _rigidbody.velocity = new Vector3(0 * _moveSpeed, 0, _joystick.Vertical  * _moveSpeed);
        }
        else 
        {
            _rigidbody.velocity = new Vector3(_joystick.Horizontal * _moveSpeed, 0, _joystick.Vertical * _moveSpeed);
        }
    }

    //아이템 입수
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.tag == "Item")
        {

            //아이템 스크립트 가져오기
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.LunchItem:
                    //CoinSound.Play();
                    lunchitem += item.value;
                    if (lunchitem > maxItem)
                        lunchitem = maxItem;
                    break;

                //물약
                case Item.Type.SaveItem:
                    saveItem += item.value;
                    if (saveItem > maxSaveItem)
                        saveItem = maxSaveItem;
                    break;

                //시간 늘리기
                case Item.Type.TimeItem:
                    timeItem += item.value;
                    if (timeItem > maxTimeItem)
                        timeItem = maxTimeItem;
                    break;

            }
            //먹으면 삭제
            GameManager.I.ItemToPut();
            Destroy(other.gameObject);
            //GameManager.I.ItemToPut();
            //도시락 획득 시 lunch.cs의 1씩 증가되는 값을 문자로 변환하여 text 변수에 저장
            //lunchCount.text = lunchAmount.ToString();
        }

        else if (other.tag == "Enemy")
        {
            GameManager.I.GameOver();
        }
        else if (other.tag == "KillingPlant")
        {
            GameManager.I.GameOver();
        }

        if (other.tag == "Wall")
        {
            isWall = true;
        }

       
    }

    /* private void OnTriggerStay(Collider other)
     {
         if(other.tag == "Wall")
         {
             var Collider = other.transform.position;
             Debug.Log("Collider.x: " + Collider.x);
             //Debug.Log("Collider.y: " + Collider.y);
             Debug.Log("Collider.z: " + Collider.z);

             Debug.Log("_joystick.Horizontal: " + _joystick.Horizontal);
             Debug.Log("_joystick.Vertical: " + _joystick.Vertical);

             var distx = (Collider.x - _rigidbody.transform.position.x);
             var disty = (Collider.y - _rigidbody.transform.position.y);

             Debug.Log("distx: " + distx);
             Debug.Log("disty: " + disty);


             var Joystick_H = Mathf.Abs(_joystick.Horizontal);
             var Joystick_V = Mathf.Abs(_joystick.Vertical);

             if (Joystick_H > Joystick_V)
             {
                 //_rigidbody.velocity = new Vector3(0 * _moveSpeed, 0, _joystick.Vertical * _moveSpeed);
                 _rigidbody.velocity = new Vector3(0 * _moveSpeed, 0, 1 * _moveSpeed);
                 Debug.Log("Joystick_H > Joystick_V -> move to vertical ");
             }
             else
             {
                 _rigidbody.velocity = new Vector3(1 * _moveSpeed, 0, 0 * _moveSpeed);
                 //_rigidbody.velocity = new Vector3(_joystick.Horizontal * _moveSpeed, 0, 0 * _moveSpeed);
                 Debug.Log("Joystick_H < Joystick_V -> move to horizontal ");
             }

         }
     }*/

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Wall")
        {
            isWall = false;
        }
    }
}