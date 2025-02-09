using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    public List<QuestionAndAnswers> QnA;
    [SerializeField] private List<Image> lifeImageList;

    int totalQuestions = 0;
    public int currentQuestion, score;
    private int lifeRemaining = 3;

    //this is for buttons of multiple choices
    public GameObject[] options;
    public GameObject quizPanel, FinishPanel, gameOverPnl, switcherPnl;
    public GameObject GameOverPanel { get { return gameOverPnl; } }

    public Text QuestionTxt, finalScore, collectedScore;

    private GameStatus gameStatus;

    [SerializeField] Image timeImage;
    [SerializeField] Text timeText;
    [SerializeField] float duration, currentTime;

    private void Start ()
    {
        totalQuestions = QnA.Count;
        quizPanel.SetActive(true);
        GenerateQuestion();

        //life counter
        lifeRemaining = 3;
        gameStatus = GameStatus.Playing;

        currentTime = duration;
        timeText.text = currentTime.ToString();
        StartCoroutine(TimeIEn());
    }
    public void correct()
    {
        //when the answer is correct
        score += 1;
        QnA.RemoveAt(currentQuestion);
        StartCoroutine(WaitForNext());
    }
    public void wrong()
    {
        //when the answer is wrong
        lifeRemaining--;
        ReduceLife(lifeRemaining);

        QnA.RemoveAt(currentQuestion);
        StartCoroutine(WaitForNext());

        if (lifeRemaining <= 0)
        {
            gameStatus = GameStatus.Next;
            gameOvrScene();
        }
    }
    IEnumerator TimeIEn() //timer
    {
        while (currentTime>=0)
        {
            timeImage.fillAmount = Mathf.InverseLerp(0, duration, currentTime);
            timeText.text = currentTime.ToString();
            yield return new WaitForSeconds(1f);
            currentTime--;
        }
            OpenPanel();
    }
    //when the game is disqualified
    public void gameOvrScene()
    {
        quizPanel.SetActive(false);
        gameOverPnl.SetActive(true);
        collectedScore.text = score + "/" + totalQuestions;
        Time.timeScale = 0;
    }
    //when all levels are completed
    void finishScene()
    {
        quizPanel.SetActive(false);
        FinishPanel.SetActive(true);
        finalScore.text = score + "/" + totalQuestions++;
        Time.timeScale = 0;
    }   
    public void retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void ending()
    {
        FinishPanel.SetActive(false);
        switcherPnl.SetActive(true);
    }
    public void next()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
        Time.timeScale = 1;
    }
    public void quit()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 0;
    }
    void setAnswers()
    {
            for (int i = 0; i < options.Length; i++)
            {
                options[i].GetComponent<AnswerScript>().isCorrect = false;
                options[i].transform.GetChild(0).GetComponent<Text>().text = QnA[currentQuestion].Answers[i];

                //for changing button colors
                options[i].GetComponent<Image>().color = options[i].GetComponent<AnswerScript>().colorChange;

                if (QnA[currentQuestion].CorrectAnswer == i + 1)
                {
                    options[i].GetComponent<AnswerScript>().isCorrect = true;
                }
            }   
    }
    void GenerateQuestion()
    {
            if (QnA.Count > 0)
            {
                currentQuestion = Random.Range(0, QnA.Count);

                QuestionTxt.text = QnA[currentQuestion].Question;
                setAnswers();
            }
            else
            {
                Debug.Log("You just finished this level!");
                finishScene();
            }
    }
    [System.Serializable]
    public enum GameStatus
    {
        Next,
        Playing
    }
    void OpenPanel()
    {
        timeText.text = "";
        gameOvrScene();
    }
    public void ReduceLife(int index)
    {
        lifeImageList[index].color = Color.clear;
    }
    //option's color generator
    IEnumerator WaitForNext()
    {
        yield return new WaitForSeconds(0.5f);
        GenerateQuestion();
    }
}