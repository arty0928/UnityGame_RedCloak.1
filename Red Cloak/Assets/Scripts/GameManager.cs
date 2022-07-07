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
    public EnemyControllerAngle enemyScript;

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
    //public GameObject EnemyC_Prefab;


    //levelUp에 따른 enemy 변화
    public int count = 0;

    public bool isPlay; //지금 싸우고 있는가
    //Game Over
    public bool isDead; //죽었는가
    public bool LevlSet; //level 세팅이 완료되었는가
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
        Debug.Log("isPlay: " + isPlay);
        Debug.Log("isDead: " + isDead);

        //if (isPlay == true && isDead == false && LevlSet == true)
        {
            //Stage text
            StageTxt.text = "STAGE " + stage;


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

            if (player.lunchitem >= stage + 1)
            {
                Debug.Log("Eat all Lunch at This Stage");
                StageEnd();
            }

            //먹은 아이템 개수
            //ItemTxt.text = "Item" + string.Format("{0:n0}", player.lunchitem); //아이템 먹은 개수 반영하기
            ItemTxt.text = string.Format("{0:n0}", player.lunchitem) + "/" + string.Format("{0:n0}", (stage * 2)); //아이템 먹은 개수 반영하기
            EnemyCnt1 = stage * 2;
            Enemy1Txt.text = "x" + string.Format("{0:n0}", EnemyCnt1); //해당 stage의 enemy 개수

            //적들의 숫자
            Enemy1Txt.text = "x " + EnemyCnt1.ToString();
            Enemy2Txt.text = "x " + EnemyCnt2.ToString();

        }

    }


}