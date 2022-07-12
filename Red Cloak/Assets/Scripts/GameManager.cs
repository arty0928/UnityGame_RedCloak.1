using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager I;
    public Transform dirrectArrow;//화살표
    public int dirrectArrowSpeed;//화살표 속도

    public GameObject menuCam;
    public GameObject gameCam;
    public PlayerController player; //player 스크립트로 받기
    //public Enemy boss; //적 스크립트로 받기
    //public EnemyControllerAngle enemyScript;

    public int stage;
    public float playTime;//게임시간

    public int EnemyCnt1; //몬스터A가 얼마나 남았나
    public int EnemyCnt2; //몬스터A가 얼마나 남았나

    //UI
    public GameObject manuPanel;
    public GameObject gamePanel;
    //public Text maxScoreTxt; //manuPanel

    //gamePanel
    public Text StageTxt; //stage 단계
    public Text ItemTxt;  //먹은 아이템 
    public Text Enemy1Txt; //적들의 숫자
    public Text Enemy2Txt; //적들의 숫자
    // public Text SaveTxt;  //목숨 값
    public Text playTimeTxt;  //남은 시간
    //public bool time;

    //아이템
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

    //levelUp에 따른 enemy 변화
    public int count = 0;

    public bool isPlay; //지금 싸우고 있는가
    //Game Over
    public bool isDead; //죽었는가
    public bool LevlSet; //level 세팅이 완료되었는가
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

        //메뉴 관련 오브젝트 비활성화, 게임 관련 오브젝트 활성화
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

        //Level6부터
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

    //Level6부터
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
        //이거 마지막에 지워야 함 그래야 도시락 개수 이상하게 반영되서 레벨업 되는 것 없앨 수 있음
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
            playTime += Time.deltaTime;//제한시간 60초 카운트다운

            //아이템 가리키는 화살표 
            Vector3 targetPosition = target.transform.position;
            targetPosition.y = transform.position.y;
            dirrectArrow.LookAt(targetPosition);
            dirrectArrow.transform.rotation = new Quaternion(0, dirrectArrow.transform.rotation.y, 0, dirrectArrow.transform.rotation.w);

        }
        

    }


    //Update()가 끝난 후 호출되는 생명주기
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


            //남은 play 시간
            int hour = (int)(playTime / 3600);
            int min = (int)((playTime - hour * 3600) / 60);
            int sec = 60 - (int)(playTime % 60);

            playTimeTxt.text = string.Format("{0:00}", sec);

            //제한시간이 초과되면 종료
            if (playTime >= 60)
            {
                GameOver();
                Debug.Log("Play Time Over");
            }

            

            //먹은 아이템 개수
            //ItemTxt.text = "Item" + string.Format("{0:n0}", player.lunchitem); //아이템 먹은 개수 반영하기
            ItemTxt.text = string.Format("{0:n0}", player.lunchitem) + "/" + string.Format("{0:n0}", (stage)); //아이템 먹은 개수 반영하기
            EnemyCnt1 = stage * 1;
            Enemy1Txt.text = "x" + string.Format("{0:n0}", EnemyCnt1); //해당 stage의 enemy 개수
            
            //EnemyPlant
            EnemyCnt2 = (stage - 4);
            if(EnemyCnt2 <= 0)
            {
                EnemyCnt2 = 0;
            }
            Enemy2Txt.text = "x" + string.Format("{0:n0}", EnemyCnt2); //해당 stage의 enemy 개수

            //적들의 숫자
            Enemy1Txt.text = "x " + EnemyCnt1.ToString();
            Enemy2Txt.text = "x " + EnemyCnt2.ToString();

        }

    }


}