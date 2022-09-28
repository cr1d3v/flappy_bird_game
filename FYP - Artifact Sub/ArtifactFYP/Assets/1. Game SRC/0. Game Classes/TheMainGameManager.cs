using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TheMainGameManager : MonoBehaviour
{

    public GamePlayerSettings GamePlayer; // GamePlayerSettings class
    public GameObject ScoringPerimeterObj; // ref to ScoringPerimeter object
    public GameObject greenPipeObj; // ref to greenPipeObj for the purpose of dynamically instantiating it in the MainGameManager
    public GameObject ABoxOfGreenPipes; // to store all of the GreenPipes
    public float inBetweenGreenPipesSize; // gap size between greenPiping from the Ceiling to floor
    public float AverageGapSizeInBetweenGreenPipes;
    public float pipingGapScallingFix; // ref to a gap between piping to fix the scalling issue

    public float distanceBetweenElements; // the distance between creating obstacles from each other
    public float scallingFixForDistance;

    // declaring the references of type Text and Button through UI pack
    public Text gameTitleTxt;
    public Text playerScoreTxt;
    public Text highestScoreTxt;

    public Text GameOverMessageTxt;
    public Image Background_Cover; // GameOverMessage background Cover
    public Button leftClickToPlayButton;
    public Button switchPlayerSkinButton;
    public Button leaderBoardButton; // not implemented!
    public Button quitTheGameButton;

    private float invisibleObjObstacleSpawn = 0f; // an invisible Game spawnStartPoint - where the last point used for spawning an obstacle was
    private int aScoreCounter = 0; // var to keep track of the current ongoing score
    private int highestScore = 0; // var to keep track of the current highestScore
    private bool gameEnd;

    // method Start() used for initialization
    void Start()
    {
        invisibleObjObstacleSpawn = -distanceBetweenElements * 2;
        highestScore = PlayerPrefs.GetInt("highestScore", 0);

        gameTitleTxt.gameObject.SetActive(true); // enabled by default
        leftClickToPlayButton.gameObject.SetActive(true);
        switchPlayerSkinButton.gameObject.SetActive(true);
        leaderBoardButton.gameObject.SetActive(true);
        quitTheGameButton.gameObject.SetActive(true);

        playerScoreTxt.gameObject.SetActive(false);
        highestScoreTxt.gameObject.SetActive(false);
        GameOverMessageTxt.gameObject.SetActive(false);
        Background_Cover.gameObject.SetActive(false);

        GamePlayer.whenToScorePts += whenScorePtsEventTriggered; // TheMainGameManager gets assigned to the GamePLayerSettings Event
        GamePlayer.whenToKillTheBird += whenToKillEventTriggered; // TheMainGameManager gets assigned to the GamePLayerSettings Event
        highestScoreTxt.text = "BEST: " + highestScore.ToString(); // highestScoreTxt display
    }

    // method Update()
    void Update()
    {
        if (GamePlayer != null) // if Gameplayer is not null = ( it exists ) then
        {
            // CreateNewObstacles. Find where the invisibleObjObstacleSpawn is and compare it to where the GamePlayer is
            // if it's shorter than distanceBetweenElements then createTheObstacle
            // different vertical positions are added so it ads a challenge to the game as per original FlappyBird concept
            if (invisibleObjObstacleSpawn - GamePlayer.transform.position.x < distanceBetweenElements)
            {
                invisibleObjObstacleSpawn += distanceBetweenElements; // move invisibleObjObstacleSpawn to the front

                CreateGreenPipes(invisibleObjObstacleSpawn + scallingFixForDistance); // create a new Green Pipe at the invisibleObjObstacleSpawn + scallingFixForDistance so that the camera will hide the new obstacles being generated
            }

            // Clear extremely far obstacles for better memory allocation, but also to ensure that the gameplay won't slow down
            // such feature will help optimize the infinite game so there's no requirement to read obstacles over and over
            // make a loop to return all indexes of all childs that are under the ABoxOfGreenPipes.transform
            for (int j = 0; j < ABoxOfGreenPipes.transform.childCount; j++)
            {
                GameObject currentGreePipe = ABoxOfGreenPipes.transform.GetChild(j).gameObject; // get the currentGreePipe in the index j
                if (GamePlayer.transform.position.x - currentGreePipe.transform.position.x > scallingFixForDistance) // check if Pipes are too far from the GamePlayer
                {
                    Destroy(currentGreePipe); // destroy the pipes which are too far away 
                }
            }
        }

        if (gameEnd == true) // otherwise
        {
            if (Input.GetAxis("Fire1") == 1f) // button to play is pressed i.e., space bar or left click on the mouse
            {
                SceneManager.LoadScene("Game");
            }
        }
    }

    void CreateGreenPipes(float x) // float x - where the Green Pipe will appear horizontally
    {
        float gapDistance = Random.Range(-AverageGapSizeInBetweenGreenPipes, AverageGapSizeInBetweenGreenPipes); // get a random nur between 2x val

        GameObject ceilingObstacle = Instantiate(greenPipeObj);
        ceilingObstacle.transform.SetParent(ABoxOfGreenPipes.transform); // SetParent as it has to be inside the ABoxOfGreenPipes
        ceilingObstacle.transform.position = new Vector2(x, inBetweenGreenPipesSize / 2 + gapDistance + pipingGapScallingFix // xyz val
        );
        ceilingObstacle.transform.localEulerAngles = new Vector3(0, 0, 180);

        GameObject floorObstacle = Instantiate(greenPipeObj);
        floorObstacle.transform.SetParent(ABoxOfGreenPipes.transform); // SetParent as it has to be inside the ABoxOfGreenPipes
        floorObstacle.transform.position = new Vector2(x, -inBetweenGreenPipesSize / 2 + gapDistance + pipingGapScallingFix
        );

        GameObject scoreArea = Instantiate(ScoringPerimeterObj);
        scoreArea.transform.SetParent(ABoxOfGreenPipes.transform); // SetParent as it has to be inside the ABoxOfGreenPipes
        scoreArea.transform.position = new Vector2(x, gapDistance); // no depth axis i.e. z / x val is exactly x / y value is gapDistance
    }
    // whenScorePtsEventTriggered() method responsible for
    void whenScorePtsEventTriggered()
    {
        aScoreCounter++; // increment

        playerScoreTxt.text = " " + aScoreCounter; // print the current score on screen

        if(highestScore < aScoreCounter) // if highestScore is less than current score
        { 
        PlayerPrefs.SetInt("highestScore", aScoreCounter); // set the highestScore as Current score
        }

    }
    // whenToKillEventTriggered() method responsible for
    void whenToKillEventTriggered()
    {
        // hide the on screen gameObjects when player is killed
        playerScoreTxt.gameObject.SetActive(false); 
        highestScoreTxt.gameObject.SetActive(false);
        GameOverMessageTxt.gameObject.SetActive(true);
        Background_Cover.gameObject.SetActive(true);

        GameOverMessageTxt.text = string.Format(GameOverMessageTxt.text, aScoreCounter); // display gameOver message along with the current counter achieved

        gameEnd = true; 
    }

    // Method responsible for whenLeftClickToPlayButtonIsSelected()
    public void whenLeftClickToPlayButtonIsSelected()
    {
        gameTitleTxt.gameObject.SetActive(false);
        leftClickToPlayButton.gameObject.SetActive(false);
        switchPlayerSkinButton.gameObject.SetActive(false);
        leaderBoardButton.gameObject.SetActive(false);
        quitTheGameButton.gameObject.SetActive(false);

        playerScoreTxt.gameObject.SetActive(true);
        highestScoreTxt.gameObject.SetActive(true);

        GamePlayer.beginToMove();
    }
    // Method responsible for whenSwitchPlayerSkinButtonIsSelected()
    public void whenSwitchPlayerSkinButtonIsSelected()
    {
        GamePlayer.changeTheSkin(); // call changeTheSkin() method from GamePlayerSettings class
    }
    // Method responsible to close the application
    public void wheneQuitTheGameButtonIsSelected()
    {
        Application.Quit(); // termiante the application when quit button is pressed
    }
}
