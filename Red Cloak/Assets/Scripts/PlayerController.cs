using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class PlayerController : MonoBehaviour
{

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private FixedJoystick _joystick;
    [SerializeField] private Animator _animator;

    [SerializeField] private float _moveSpeed;
    
    //�������� �ܰ� 
    public int stageNum;

    //������ �Ա�
    //public Transform item;
    public int lunchitem =0;
    public int timeItem;
    public int saveItem;

    public int maxItem;
    public int maxTimeItem;
    public int maxSaveItem;

    //������ ������ ���� ����
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

    }

    /*private void FixedUpdate()
    { _rigidbody.velocity = new Vector3(_joystick.Horizontal * _moveSpeed, 0, _joystick.Vertical * _moveSpeed);
    }*/

    private void LateUpdate()
    {
        LookDir = _joystick.Vertical * Vector3.forward + _joystick.Horizontal * Vector3.right;
        transform.rotation = Quaternion.LookRotation(LookDir);

        PlayerMove();
    }
    
    private void PlayerMove()
    {
        if (isWall)
        {
            
            var Joystick_H = Mathf.Abs(_joystick.Horizontal);
            var Joystick_V = Mathf.Abs(_joystick.Vertical);

            float verticalDirection = 1;
            float horizontalDirection = 1;
            if (_joystick.Vertical < 0) verticalDirection = -1;
            if (_joystick.Horizontal < 0) horizontalDirection = -1;

            _rigidbody.velocity = new Vector3(horizontalDirection * _moveSpeed, 0, verticalDirection * _moveSpeed);

            /*if (Joystick_H > Joystick_V)
            {
                Debug.Log(" _joystick.Vertical: " + _joystick.Vertical);
                Debug.Log(" _joystick.Horizontal: " + _joystick.Horizontal);
                _rigidbody.velocity = new Vector3(0 * _moveSpeed, 0, _joystick.Vertical*5 * _moveSpeed);
                //_rigidbody.velocity = new Vector3(0 * _moveSpeed, 0, 1 * _moveSpeed);
                Debug.Log("Joystick_H > Joystick_V -> move to vertical ");
            }
            else
            {
                Debug.Log(" _joystick.Vertical: "+ _joystick.Vertical);
                Debug.Log(" _joystick.Horizontal: " + _joystick.Horizontal);

                //_rigidbody.velocity = new Vector3(1 * _moveSpeed, 0, 0 * _moveSpeed);
                _rigidbody.velocity = new Vector3(_joystick.Horizontal*5 * _moveSpeed, 0, 0 * _moveSpeed);
                Debug.Log("Joystick_H < Joystick_V -> move to horizontal ");
            } */
        }
        else
        {
            _rigidbody.velocity = new Vector3(_joystick.Horizontal * _moveSpeed, 0, _joystick.Vertical * _moveSpeed);
        }
    }

    //������ �Լ�
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.tag == "Item")
        {

            //������ ��ũ��Ʈ ��������
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.LunchItem:
                    //CoinSound.Play();
                    lunchitem += item.value;
                    if (lunchitem > maxItem)
                        lunchitem = maxItem;
                    break;

                //����
                case Item.Type.SaveItem:
                    saveItem += item.value;
                    if (saveItem > maxSaveItem)
                        saveItem = maxSaveItem;
                    break;

                //�ð� �ø���
                case Item.Type.TimeItem:
                    timeItem += item.value;
                    if (timeItem > maxTimeItem)
                        timeItem = maxTimeItem;
                    break;

            }
            //������ ����
            GameManager.I.ItemToPut();
            Destroy(other.gameObject);
            //GameManager.I.ItemToPut();
            //���ö� ȹ�� �� lunch.cs�� 1�� �����Ǵ� ���� ���ڷ� ��ȯ�Ͽ� text ������ ����
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

        /*while(other.tag == "Wall")
        {
            var Collider = other.transform.position;
            Debug.Log("Collider.x: " + Collider.x);
            //Debug.Log("Collider.y: " + Collider.y);
            Debug.Log("Collider.z: " + Collider.z);

            Debug.Log("_joystick.Horizontal: "+_joystick.Horizontal);
            Debug.Log("_joystick.Vertical: "+ _joystick.Vertical);

            var distx = (Collider.x - _rigidbody.transform.position.x);
            var disty = (Collider.y - _rigidbody.transform.position.y);

            Debug.Log("distx: "+ distx);
            Debug.Log("disty: "+ disty);


            var Joystick_H = Mathf.Abs(_joystick.Horizontal);
            var Joystick_V = Mathf.Abs(_joystick.Vertical);

            if(Joystick_H > Joystick_V)
            {
                _rigidbody.velocity = new Vector3(0 * _moveSpeed, 0, _joystick.Vertical * _moveSpeed);
                Debug.Log("Joystick_H > Joystick_V -> move to vertical ");
            }
            else
            {
                _rigidbody.velocity = new Vector3(_joystick.Horizontal * _moveSpeed, 0, 0 * _moveSpeed);
                Debug.Log("Joystick_H < Joystick_V -> move to horizontal ");
            }

        }*/
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