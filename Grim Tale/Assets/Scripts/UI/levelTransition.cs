using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class levelTransition : MonoBehaviour
{
    Scene scene;
    GameObject enemies;
    GameObject player;
    public TMP_Text end;
    public TMP_Text congrats;
    private bool check = true;
    private const int maxIndex = 10;
    private float transitionTime = 3.0f;
    private AsyncOperation sceneAsync;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        scene = SceneManager.GetActiveScene();
        enemies = GameObject.FindGameObjectWithTag("enemies");
    }

    // Update is called once per frame
    void Update()
    {
        if (enemies.transform.childCount <= 0 && check)
        {

            if (scene.buildIndex < maxIndex)
            {
                check = false;
                StartCoroutine(transitionLength());
                end.gameObject.SetActive(true);
            }
            else
                congrats.gameObject.SetActive(true);

        }

    }

    // Coroutine that tracks how much time is left to the transition.
    IEnumerator transitionLength()
    {

        float normTime = 0.0f;
        while (normTime <= 1.0f) 
        {
            normTime += Time.deltaTime / transitionTime;
            yield return null;
        }
        end.gameObject.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

   

}
