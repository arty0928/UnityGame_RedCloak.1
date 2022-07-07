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


    
    private void Start()
    {
        //var enemies = GameObject.FindGameObjectsWithTag("Enemy").Select(enemy => enemy.transform.position).ToList();
        //lunchitem = GameManager.I.stage * 2;

    }

    private void FixedUpdate()
    { _rigidbody.velocity = new Vector3(_joystick.Horizontal * _moveSpeed, 0, _joystick.Vertical * _moveSpeed);
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
                    //manager.SfxPlay(AudioManager.Sfx.coin);
                    //AudioManager.I.SfxPlay(AudioManager.Sfx.coin);

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
        //else if (other.tag == "Enemy")
        {
            GameManager.I.GameOver();
        }
    }
}