using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject gameManager;
    [SerializeField] GameObject menu;
    [SerializeField] UILabel m_Lable;
    // [SerializeField] Text m_Text;

    [Header("場景")]
    [SerializeField] string continueScene;

    [Header("轉場黑幕")]
    [Space(10), SerializeField, Range(1, 20)]
    private int speed;
    int i = 0;
    Scene nowScene;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager._instance.gameObject;
        gameManager.SetActive(false);
        nowScene = SceneManager.GetActiveScene();
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void MenuActive()
    {
        menu.SetActive(false);
        
    }

    public void NewGameButton()
    {
        gameManager.SetActive(true);
        menu.GetComponent<TweenAlpha>().enabled = true;
        // StartCoroutine(LoadScene("TutorialScene"));
    }

    public void ContinueButton()
    {
        if (continueScene != "")
        { StartCoroutine(LoadScene(continueScene)); }
        else
        {
            m_Lable.text = "No save file!";
            m_Lable.gameObject.GetComponent<TweenAlpha>().enabled = true;
        }
    }

    public void ResetText()
    {
        m_Lable.text = "";
    }

    public void ChangeScene(string ns)
    {
        StartCoroutine(LoadScene(ns));
    }

    public void ExitButton()
    { Application.Quit(); }

    IEnumerator LoadScene(string ns)
    {
        yield return null;

        //Begin to load the Scene you specify
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(ns);
        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;
        Debug.Log("Pro :" + asyncOperation.progress);
        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {
            //Output the current progress
            m_Lable.text = " " + (asyncOperation.progress * 100) + "%";

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                m_Lable.text = "Done!";
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
