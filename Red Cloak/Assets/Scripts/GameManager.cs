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
    //public EnemyControllerAngle enemyScript;

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
    public GameObject EnemyC_Prefab; //SizeUp
    public GameObject EnemyPlant_Prefab;
    public GameObject EnemyD_Prefab; //double RayCast
    //Transform[] EnemyPlants;

    //HideZonde
    public GameObject HideZone_Prefab;
    private int HideZoneCallCount = 0;

    //levelUp�� ���� enemy ��ȭ
    public int count = 0;

    public bool isPlay; //���� �ο�� �ִ°�
    //Game Over
    public bool isDead; //�׾��°�
    public bool LevlSet; //level ������ �Ϸ�Ǿ��°�
    public GameObject overPanel;

    //audio
    public AudioSource GameOverSound;

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
        //Debug.Log("isPlay: " + isPlay);
        //Debug.Log("isDead: " + isDead);
        //isPlay = true;
        //isDead = false;
        playTime = 0;
        player.lunchitem = 0;
        
        
        
        //isPlay = false;
        
        count = 0;
        stage++;

        if(stage <= 10)
        {
            var Enemy = GameObject.FindGameObjectsWithTag("Enemy");
            for (var i = 0; i < Enemy.Length; i++)
            {
                Destroy(Enemy[i]);
            }

            EnemyToPut();

        }


        if (stage > 6)
        {
            var KillingPlant = GameObject.FindGameObjectsWithTag("KillingPlant");
            for (var i = 0; i < KillingPlant.Length; i++)
            {
                Destroy(KillingPlant[i]);
            }

            var HideZoneExist = GameObject.FindGameObjectsWithTag("HideZone");
            for (var j = 0; j < HideZoneExist.Length; j++)
            {
                Destroy(HideZoneExist[j]);
            }

            HideZoneToPut();
            PlantToPut();
        }

        if (stage > 10)
        {
            var DoubleRayCasts = GameObject.FindGameObjectsWithTag("DoubleRayCast");
            for (var i = 0; i < DoubleRayCasts.Length; i++)
            {
                Destroy(DoubleRayCasts[i]);
            }

            DoubleRayCastEnemyToPut();
        }

        Debug.Log("playerPositionReset");
        player.transform.position = new Vector3(0, 0.74f , -0.68f);
        Debug.Log("LevelUp: " + stage);
        Debug.Log("player.lunchitem: " + player.lunchitem);
        Debug.Log("=========================================");
        Debug.Log("stage: "+ stage);
        Debug.Log("player.lunchitem: " + player.lunchitem);
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
            GameOverSound.Play();
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
        HideZoneCallCount = 0;


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
        //Debug.Log("isPlay: " + isPlay);
        //Debug.Log("isDead: " + isDead);
        SceneManager.LoadScene(0);
        count = 0;
    }

    private void Start()
    {
        //StageStart();

        ItemToPut();
        EnemyToPut();

        /*if(stage >= 10)
        {
            Invoke("HideZoneToPut", 3.0f);
        }*/

        //HideZoneToPut();
    }

    public void ItemToPut()
    {
        var items = GameObject.FindGameObjectsWithTag("LunchToPut").Select(ItemToPut => ItemToPut.transform.position).ToArray();
        items = items.OrderBy(item => Random.Range(-1.0f, 1.0f)).ToArray();
        itemPos = items[0];

        //Debug.Log("ItemToPut");
        target = Instantiate(ItemPrefab, new Vector3(itemPos.x, itemPos.y, itemPos.z), transform.rotation).transform;
        Debug.Log("stage: "+stage);
        Debug.Log("After_ItemToput");
        //Debug.Log("isPlay: " + isPlay);
        //Debug.Log("isDead: " + isDead);

        //target = GameObject.FindGameObjectWithTag("Item").transform;
    }


    public void EnemyToPut()
    {
        Debug.Log("EnemyToPut");
        var Enemies = GameObject.FindGameObjectsWithTag("LunchToPut").Select(EnemyToPut => EnemyToPut.transform.position).ToArray();
        Enemies = Enemies.OrderBy(Enemy => Random.Range(-1.0f, 1.0f)).ToArray();
        

        int count = 0;

        //Level6����
        PlantToPut();
        HideZoneToPut();
        
        if (stage >= 5)
        {   
            if(stage >= 10)
            {
                DoubleRayCastEnemyToPut();
            }
            else
            {
                for (var j = 0; j < (stage * 1); j++)
                {
                    if (stage * 1 <= Enemies.Length)
                    {
                        if (count < Mathf.FloorToInt(stage / 2))
                        {
                            Instantiate(EnemyC_Prefab, Enemies[j], transform.rotation);
                            count++;
                        }
                        else
                        {
                            Instantiate(EnemyB_Prefab, Enemies[j], transform.rotation);
                        }
                    }

                }
            }
           
        }
        else {
            for (var j = 0; j < (stage * 1); j++)
                if(stage * 1 <= Enemies.Length)
                {
                    Instantiate(EnemyB_Prefab, Enemies[j], transform.rotation);
                }
                
        }

        

    }

    //Level6����
    public void PlantToPut()
    {
        
        if (stage >= 6)
        {
            var Plants = GameObject.FindGameObjectsWithTag("KillingPlantToPut").Select(PlantToPut => PlantToPut.transform.position).ToArray();
            Plants = Plants.OrderBy(item => Random.Range(-1.0f, 1.0f)).ToArray();
            //EnemyPlants[i]

            //Instantiate(EnemyPlant_Prefab, new Vector3(Plants[0].x, Plants[0].y, Plants[0].z);
            //Instantiate(EnemyPlant_Prefab, Plants[0], transform.rotation);

            for (var j = 0; j < (stage - 4); j++)
            {
                {
                    if(0< stage-4 && stage-4< 9)
                    {
                        //Instantiate(EnemyPlant_Prefab, new Vector3(itemPos.x, itemPos.y, itemPos.z), transform.rotation).transform;
                        //Instantiate(EnemyB_Prefab, Enemies[j], transform.rotation);
                        Instantiate(EnemyPlant_Prefab, Plants[j], transform.rotation);
                    }
                    
                }
            }
        }


    }

    public void HideZoneToPut()
    {

        if (stage >=10)
        {
            /*if (HideZoneCallCount != 0)
            {
                Debug.Log("HideZoneCallCount: " + HideZoneCallCount);
                var HideZoneExist = GameObject.FindGameObjectsWithTag("HideZoneToPut");
                for (var j = 0; j < HideZoneExist.Length; j++)
                {
                    Destroy(HideZoneExist[j]);
                }
            }*/

            var HideZone = GameObject.FindGameObjectsWithTag("HideZoneToPut").Select(HideZone => HideZone.transform.position).ToArray();
            HideZone = HideZone.OrderBy(item => Random.Range(-1.0f, 1.0f)).ToArray();
            Instantiate(HideZone_Prefab, HideZone[0], transform.rotation);
            Instantiate(HideZone_Prefab, HideZone[1], transform.rotation);
            Instantiate(HideZone_Prefab, HideZone[2], transform.rotation);

            /*for (var i = 0; i < 3; i++)
            {
                Instantiate(HideZone_Prefab, HideZone[i], transform.rotation);

            }*/
            HideZoneCallCount++;
        }
      
    }

    public void DoubleRayCastEnemyToPut()
    {

        Debug.Log("DoubleRayCastEnemyToPut");
        var DoubleRayCatEnemies = GameObject.FindGameObjectsWithTag("LunchToPut").Select(EnemyToPut => EnemyToPut.transform.position).ToArray();
        DoubleRayCatEnemies = DoubleRayCatEnemies.OrderBy(Enemy => Random.Range(-1.0f, 1.0f)).ToArray();

        if (stage >= 10)
        {
            
                
                    for(var i = 0; i <3; i++)
                    {
                        Instantiate(EnemyD_Prefab, DoubleRayCatEnemies[i], transform.rotation);
                        
                    }
                    /*for(var k =0; k < stage - 3; k++)
                    {
                        Instantiate(EnemyB_Prefab, DoubleRayCatEnemies[k], transform.rotation);
                    }*/
                

            
        }

        
    }
    private void Update()
    {
        //�̰� �������� ������ �� �׷��� ���ö� ���� �̻��ϰ� �ݿ��Ǽ� ������ �Ǵ� �� ���� �� ����
        if (player.lunchitem >= stage)
        {
            Debug.Log("Eat all Lunch at This Stage");
            player.lunchitem = 0;
            StageEnd();
        }

        Debug.Log("Update");
        //Debug.Log("isPlay: " + isPlay);
        //Debug.Log("isDead: " + isDead);

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
        //Debug.Log("isPlay: " + isPlay);
        //Debug.Log("isDead: " + isDead);

        //if (isPlay == true && isDead == false && LevlSet == true)
        {
            //Stage text
            if(stage >= 10)
            {
                StageTxt.text = "STAGE" + stage;
            }
            else
            {
                StageTxt.text = "STAGE" + stage;

            }


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

            

            //���� ������ ����
            //ItemTxt.text = "Item" + string.Format("{0:n0}", player.lunchitem); //������ ���� ���� �ݿ��ϱ�
            ItemTxt.text = string.Format("{0:n0}", player.lunchitem) + "/" + string.Format("{0:n0}", (stage)); //������ ���� ���� �ݿ��ϱ�
            EnemyCnt1 = stage * 1;
            Enemy1Txt.text = "x" + string.Format("{0:n0}", EnemyCnt1); //�ش� stage�� enemy ����
            
            //EnemyPlant
            EnemyCnt2 = (stage - 4);
            if(EnemyCnt2 <= 0)
            {
                EnemyCnt2 = 0;
            }
            Enemy2Txt.text = "x" + string.Format("{0:n0}", EnemyCnt2); //�ش� stage�� enemy ����

            //������ ����
            Enemy1Txt.text = "x " + EnemyCnt1.ToString();
            Enemy2Txt.text = "x " + EnemyCnt2.ToString();

        }

    }


}