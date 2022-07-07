using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager I;
    public Transform dirrectArrow;//ȭ��ǥ
    public int dirrectArrowSpeed;//ȭ��ǥ �ӵ�

    public GameObject menuCam;
    public GameObject gameCam;
    public PlayerController player; //player ��ũ��Ʈ�� �ޱ�
    //public Enemy boss; //�� ��ũ��Ʈ�� �ޱ�
    public EnemyControllerAngle enemyScript;

    public int stage;
    public float playTime;//���ӽð�

    public int EnemyCnt1; //����A�� �󸶳� ���ҳ�
    public int EnemyCnt2; //����A�� �󸶳� ���ҳ�

    //UI
    public GameObject manuPanel;
    public GameObject gamePanel;
    //public Text maxScoreTxt; //manuPanel

    //gamePanel
    public Text StageTxt; //stage �ܰ�
    public Text ItemTxt;  //���� ������ 
    public Text Enemy1Txt; //������ ����
    public Text Enemy2Txt; //������ ����
    // public Text SaveTxt;  //��� ��
    public Text playTimeTxt;  //���� �ð�
    //public bool time;

    //������
    public Vector3 itemPos;
    public GameObject ItemPrefab;
    public Transform target;

    //enemy
    public Transform[] EnemyPoint;
    public Vector3 EnemyPos;
    public GameObject EnemyB_Prefab;
    //public GameObject EnemyC_Prefab;


    //levelUp�� ���� enemy ��ȭ
    public int count = 0;

    public bool isPlay; //���� �ο�� �ִ°�
    //Game Over
    public bool isDead; //�׾��°�
    public bool LevlSet; //level ������ �Ϸ�Ǿ��°�
    public GameObject overPanel;

    private void Awake()
    {
        I = this;
    }

    public void GameStart()
    {
        Debug.Log("GameStart");
        Debug.Log("isPlay: " + isPlay);
        Debug.Log("isDead: " + isDead);

        //�޴� ���� ������Ʈ ��Ȱ��ȭ, ���� ���� ������Ʈ Ȱ��ȭ
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        manuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
        StageStart();
        //isPlay = true;

    }

    public void StageStart()
    {
        Debug.Log("StageStart()");
        Debug.Log("isPlay: " + isPlay);
        Debug.Log("isDead: " + isDead);
        isPlay = true;
        isDead = false;
        LevlSet = true;



        Debug.Log("isPlay: " + isPlay);
        Debug.Log("isDead: " + isDead);
        //enemyScript.EnemyMove();

        //isDead = false;
    }
    public void StageEnd()
    {
        Debug.Log("StageEnd");
        Debug.Log("isPlay: " + isPlay);
        Debug.Log("isDead: " + isDead);
        //isPlay = true;
        //isDead = false;
        playTime = 0;
        //player.lunchitem = stage * 2;
        player.lunchitem = 0;
        player.transform.position = Vector3.zero;
        //isPlay = false;
        stage++;
        count = 0;

        var items = GameObject.FindGameObjectsWithTag("Enemy");
        for (var i = 0; i < items.Length; i++)
        {
            Destroy(items[i]);
        }

        EnemyToPut();


    }

    /*IEnumerator InBattle()
    {
        yield return new WaitForSeconds(5);
        StageEnd();
    }*/


    public void GameOver()
    {
        if (isPlay == true && isDead == false)
        {
            Debug.Log("Game Over");
            Debug.Log("isPlay: " + isPlay);
            Debug.Log("isDead: " + isDead);
            //isDead = true;
            OnDie();

            //isPlay = false;

            isPlay = false;
            isDead = true;
            Debug.Log("after Game Over");
            Debug.Log("isPlay: " + isPlay);
            Debug.Log("isDead: " + isDead);
            gamePanel.SetActive(false);
            overPanel.SetActive(true);
            stage = 0;
        }

        count = 0;


    }

    public void OnDie()
    {
        //isDead = true;
        //isDead = true;
        //isPlay = false;
    }

    public void ReStart()
    {
        Debug.Log("ReStart");
        Debug.Log("isPlay: " + isPlay);
        Debug.Log("isDead: " + isDead);
        SceneManager.LoadScene(0);
        count = 0;
    }

    private void Start()
    {
        //StageStart();

        ItemToPut();
        EnemyToPut();

    }

    public void ItemToPut()
    {
        var items = GameObject.FindGameObjectsWithTag("LunchToPut").Select(ItemToPut => ItemToPut.transform.position).ToArray();
        items = items.OrderBy(item => Random.Range(-1.0f, 1.0f)).ToArray();
        itemPos = items[0];


        target = Instantiate(ItemPrefab, new Vector3(itemPos.x, itemPos.y, itemPos.z), transform.rotation).transform;
        Debug.Log("ItemToput");
        Debug.Log("isPlay: " + isPlay);
        Debug.Log("isDead: " + isDead);

        //target = GameObject.FindGameObjectWithTag("Item").transform;
    }

    public void EnemyToPut()
    {
        var Enemies = GameObject.FindGameObjectsWithTag("LunchToPut").Select(EnemyToPut => EnemyToPut.transform.position).ToArray();
        Enemies = Enemies.OrderBy(Enemy => Random.Range(-1.0f, 1.0f)).ToArray();

        for (var j = 0; j < (stage * 2); j++)
        {
            {
                Instantiate(EnemyB_Prefab, Enemies[j], transform.rotation);
            }
        }

        /* else if(stage >= 3)
         {

             for (var j = 0; j < (stage * 2); j++)
             {
                 if (count < stage * 1)
                 {
                     Instantiate(EnemyC_Prefab, Enemies[j], transform.rotation);
                     count++;
                 }
                 else
                 {
                     Instantiate(EnemyB_Prefab, Enemies[j], transform.rotation);
                 }

             }
         }*/

    }



    private void Update()
    {

        Debug.Log("Update");
        Debug.Log("isPlay: " + isPlay);
        Debug.Log("isDead: " + isDead);

        //if (isPlay == true && isDead == false && LevlSet==true)
        {
            playTime += Time.deltaTime;//���ѽð� 60�� ī��Ʈ�ٿ�

            //������ ����Ű�� ȭ��ǥ 
            Vector3 targetPosition = target.transform.position;
            targetPosition.y = transform.position.y;
            dirrectArrow.LookAt(targetPosition);
            dirrectArrow.transform.rotation = new Quaternion(0, dirrectArrow.transform.rotation.y, 0, dirrectArrow.transform.rotation.w);

        }
    }


    //Update()�� ���� �� ȣ��Ǵ� �����ֱ�
    private void LateUpdate()

    {
        Debug.Log("LateUpDate");
        Debug.Log("isPlay: " + isPlay);
        Debug.Log("isDead: " + isDead);

        //if (isPlay == true && isDead == false && LevlSet == true)
        {
            //Stage text
            StageTxt.text = "STAGE " + stage;


            //���� play �ð�
            int hour = (int)(playTime / 3600);
            int min = (int)((playTime - hour * 3600) / 60);
            int sec = 60 - (int)(playTime % 60);

            playTimeTxt.text = string.Format("{0:00}", sec);

            //���ѽð��� �ʰ��Ǹ� ����
            if (playTime >= 60)
            {
                GameOver();
                Debug.Log("Play Time Over");
            }

            if (player.lunchitem >= stage + 1)
            {
                Debug.Log("Eat all Lunch at This Stage");
                StageEnd();
            }

            //���� ������ ����
            //ItemTxt.text = "Item" + string.Format("{0:n0}", player.lunchitem); //������ ���� ���� �ݿ��ϱ�
            ItemTxt.text = string.Format("{0:n0}", player.lunchitem) + "/" + string.Format("{0:n0}", (stage * 2)); //������ ���� ���� �ݿ��ϱ�
            EnemyCnt1 = stage * 2;
            Enemy1Txt.text = "x" + string.Format("{0:n0}", EnemyCnt1); //�ش� stage�� enemy ����

            //������ ����
            Enemy1Txt.text = "x " + EnemyCnt1.ToString();
            Enemy2Txt.text = "x " + EnemyCnt2.ToString();

        }

    }


}