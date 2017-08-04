using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour {



    public Transform ground;
    bool countDownState;
    int firstTile, secondTile, thirdTile = -1, counter, toClick;
    public int level=1;
    int touchState = 0;
    float countDownTimer=0;
    public GameObject countDownPanel;
    bool tutorialComplete = true;
    bool next;

    public int maxTiles = 2;
    public int score=-1;
    public static GameController instance;
    public Text scoreText, tutorialText, loseText, causeText, levelText;
    bool firstWhite, firstBlack, firstRed, firstGreen=false, firstBlue, firstComplexThree, firstLocked, firstTimeIncrease, locked, gameOver;
    public bool tutorial, pause;
    public AudioSource error, coin;
    public int[] scoreLimits = new int[11] { -1, 4, 9, 14, 19, 24, 29, 34, 39, 100000, 10000000 };
    float timeLimit=0, minimumTimeLimit=1.5f;
    public Slider timeSlider;
    public GameObject tutorialWindow, loseWindow;
    Camera mainCamera;
    SectionController[] sections;
    string wrong = "Wrong Tap", timeUp = "Time Up";
  
    public int ToClick
    {
        get { return toClick; }
        set { toClick =value; }
    }

   
    public enum ColoursEnum
    {
        green=0, red=1, white=2, black=3, lightBlue=4
    }
  

    void Awake()
    {
        mainCamera = FindObjectOfType<Camera>();
        Screen.orientation = ScreenOrientation.Portrait;
        instance = this;
        sections = FindObjectsOfType<SectionController>();

    }

	// Use this for initialization
	void Start () {
	}

    // Update is called once per frame
    void Update()
    {
        int state = 0;
        if (tutorial && !countDownState)
        {
            Time.timeScale = 0;
            tutorialWindow.SetActive(true);
        }


        StartCoroutine(SwitchColour());
        if (timeLimit == 0)
            timeLimit = 2;
        if (Input.GetMouseButtonDown(0))
        {
            if (gameOver) {
                Restart();
            }
            else if (tutorial) { CloseTutorial(); }
            else
            {
                //Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(t).position);
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), -Vector2.zero);

                if (hit)
                {
                    if (hit.transform.name.Contains("Terrain"))
                    {
                        //  CheckCorrect(hit);
                        state = sections[0].CheckCorrect(hit);

                        if (state == 1)
                        {
                            coin.Play();
                            state = 0;
                        }
                        if (state != -1 && level > 7)
                        {
                            state = sections[1].CheckCorrect(hit);
                            if (state == 1)

                            {
                                coin.Play();
                                state = 0;
                            }
                        }
                    }
                }
            }
        }


        if (Input.touchCount == 1)
        {


            if (Input.GetTouch(0).phase == TouchPhase.Began && touchState == 0)
            {
                if (gameOver)
                {
                    touchState = 1;
                    Restart();

                }
                else if (tutorial)
                {
                    touchState = 1;
                    CloseTutorial();
                }
                else
                {
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), -Vector2.zero);
                    touchState = 1;
                    if (hit)
                    {


                        if (hit.transform.name.Contains("Terrain"))
                        {
                            //CheckCorrect(hit);
                            state = sections[0].CheckCorrect(hit);
                            if (state == 1)
                            {
                                coin.Play();
                                state = 0;
                            }
                            if (state != -1 && level > 7)
                            {
                                state = sections[1].CheckCorrect(hit);
                                if (state == 1)

                                {
                                    coin.Play();
                                    state = 0;
                                }
                            }
                        }
                    }
                }
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                touchState = 0;

            }

        }

        timeSlider.value -= Time.deltaTime;

        if (state == -1)
        {
            error.Play();
            GameOver(wrong);
        }
       
    }

    
    

    public void CloseTutorial()
    {
      if (next)
            {
            level = 8;
            Application.LoadLevel(1);
            }
        countDownState = true;

        tutorialWindow.SetActive(false);
        //tutorial = false;
        countDownPanel.SetActive(true);
        Resume();
    }

    void Resume()
    {
        Time.timeScale = 1;
        //tutorial = false;
        pause = false;
    }


  
    IEnumerator SwitchColour()
    {
      
        yield return new WaitForSeconds(timeLimit);

        if (toClick > 0 && tutorialComplete) {
            if (!countDownState)
                GameOver(timeUp);
        }

        if (countDownState )
        {
            countDownPanel.SetActive(false);
            countDownState = false;
        }
            timeSlider.maxValue = timeLimit;
        timeSlider.value = timeLimit;
        toClick = 0;
        {

            if (score == scoreLimits[1])
            {
                if (tutorial)
                {
                    sections[0].ManualGeneration(2, new int[2] { (int)ColoursEnum.green, (int)ColoursEnum.red }, (int)ColoursEnum.black);
                    tutorial = false;
                    tutorialComplete = true;

                }
                else if ( tutorialComplete)
                {
                    tutorialComplete = false;
                    toClick = 1;
                    tutorialWindow.SetActive(true);
                    tutorialText.text = "With black background\n only tap \n the RED tiles";
                    level = 2;
                    levelText.text = "Level: " + score;

                    tutorial = true;

                }
            }

           else if (score == scoreLimits[2])
            {
                if (tutorial)
                {
                    sections[0].ManualGeneration(2, new int[2] { (int)ColoursEnum.white, (int)ColoursEnum.black }, (int)ColoursEnum.green);
                    tutorial = false;
                    tutorialComplete = true;

                }
                else if (tutorialComplete)
                {
                    tutorialComplete = false;
                    toClick = 2;
                    tutorialWindow.SetActive(true);
                    tutorialText.text = "With green background\n tap ALL tiles";
                    level = 3;
                    levelText.text = "Level: " + score;
                    tutorial = true;

                }
            }
            else if (score == scoreLimits[3])
            {
                if (tutorial)
                {
                    sections[0].ManualGeneration(2, new int[2] { (int)ColoursEnum.black, (int)ColoursEnum.white }, (int)ColoursEnum.red);
                    tutorial = false;
                    tutorialComplete = true;

                }
                else if (tutorialComplete)
                {
                    tutorialComplete = false;
                    toClick = 0;
                    tutorialWindow.SetActive(true);
                    tutorialText.text = "With red background \n Don't tap anything";
                    level = 4;
                    levelText.text = "Level: " + score;

                    tutorial = true;

                }
            }
            else if (score == scoreLimits[4])
            {
                if (tutorial)
                {
                    sections[0].ManualGeneration(3, new int[3] { (int)ColoursEnum.lightBlue, (int)ColoursEnum.white, (int)ColoursEnum.black }, (int)ColoursEnum.red);
                    tutorial = false;
                    tutorialComplete = true;

                }
                else if (tutorialComplete)
                {
                    tutorialComplete = false;
                    toClick = 1;
                    tutorialWindow.SetActive(true);
                    tutorialText.text = "ALWAYS TAP\n blue tiles \n on ANY background";
                    level = 5;
                    levelText.text = "Level: " + score;

                    tutorial = true;

                }
            }

            //else if (score == scoreLimits[4])
            //{
            //    tutorial = true;
            //    tutorialWindow.SetActive(true);
            //    if (tutorialComplete)
            //    {
            //        sections[0].ManualGeneration(3, new int[3] { (int)ColoursEnum.lightBlue, (int)ColoursEnum.white, (int)ColoursEnum.black }, (int)ColoursEnum.red);
            //        tutorialComplete = false;
            //    }
            //    toClick = 1;
            //    level = 5;
            //    levelText.text = "Level: " + level;
            //    tutorialText.text = "ALWAYS TAP\n blue tiles \n on ANY background";
            //    countDownState = true;

            //}
            else
            {


                if (sections.Length == 1)
                {
                    toClick += sections[0].SwitchColour(maxTiles);
                }
                else
                {
                    int splits = Random.Range(0, maxTiles + 1);
                    for (int s = 0; s < sections.Length; s++)
                    {
                        toClick += sections[s].SwitchColour(splits);
                        splits = maxTiles - splits;

                    }
                }

            }
            //  else

     /*        if (score == scoreLimits[5])
            {
                if (tutorial)
                {
                    tutorial = false;
                    tutorialComplete = true;

                }
                else if (tutorialComplete)
                {
                    tutorialComplete = false;
                    tutorialWindow.SetActive(true);
                    tutorialText.text = "Let's add\n one more tile";
                    level = 6;
                    levelText.text = "Level: " + score;

                    tutorial = true;

                }
            }


          /*  if (score == scoreLimits[5])
            {
                if (tutorial)
                {
                    tutorial = false;
                    tutorialComplete = true;

                }
                else if (tutorialComplete)
                {
                    tutorialComplete = false;
                    tutorialWindow.SetActive(true);
                    tutorialText.text = "Going faster now";
                    level = 6;
                    timeLimit -= 0.1f;
                    levelText.text = "Level: " + score;

                    tutorial = true;

                }
            }*/
            if (score == scoreLimits[5])
            {
                firstTimeIncrease = false;
                tutorial = true;
                level = 6;
                tutorialWindow.SetActive(true);
                levelText.text = "Level: " + level;
                tutorialText.text = "Let's split the screen";
                countDownState = true;

                next = true;
            }
            //}
            //if (firstLocked)
            //        {
            //            if (k == firstTile)
            //            {
            //                firstLocked = false;
            //                tiles[k].Locked = true;
            //                locked = true;
            //                tutorial = true;
            //                tutorialWindow.SetActive(true);
            //                levelText.text = "Level: " + level;
            //                tutorialText.text = "Tiles with a STAR \n (with correct colour) \n must be tapped TWICE \n ";
            //            }
            //        }

            //    if (level > 6)
            //    {
            //        if (!locked)
            //        {
            //            locked = tiles[k].TryLock(Random.Range(0, 11));
            //        }
            //    }
            //}


            //}

            if(tutorialComplete)
            {
                scoreText.text = ++score + "";

                if (timeLimit > 1.5f)
                {
                    timeLimit -= 0.01f;
                }
            }


            if (tutorial)
            {
                if (score == 0)
                {
                    sections[0].ManualGeneration(2, new int[2] { (int)ColoursEnum.green, (int)ColoursEnum.red }, (int)ColoursEnum.white);
                    tutorial = false;
                    tutorialComplete = true;

                }
            }
            else  if (score == 0 && tutorialComplete)
              {
                firstWhite = true;
                tutorialComplete = false;
                toClick = 1;
                tutorialWindow.SetActive(true);
                tutorialText.text = "Tap the GREEN tiles";
                level = 1;
              
                    tutorial = true;

            }
        }

     
        

        StopAllCoroutines();
        }
    

    void GameOver(string cause)
    {
        if (!gameOver)
        {
            loseWindow.SetActive(true);
            causeText.text = cause;
            loseText.text = "Your Score: " + score;
            Time.timeScale = 0;
            gameOver = true;
        }
    }




  
    public void Restart()
    {
        Time.timeScale = 1;
        Application.LoadLevel(Application.loadedLevel);
    }

   //void CountDown()
   // {
   //     if (countDownTimer > 0)
   //     {
   //         countDownText.text = "" + countDownTimer;
   //         countDownTimer--;
   //         Invoke("CountDown", 1);
   //     }
   //     else
   //     { countDownState = false;
   //         countDownPanel.SetActive(false);
   //     }
   // }

}



