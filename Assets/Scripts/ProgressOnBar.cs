using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressOnBar : MonoBehaviour
{
    public float progress;
    private float _targetValue;
    private float _increaseSpeed;
    private GameVariableConnector _gameVariableConnector;
    private CreateJob _createJobScript;

    public int taskID = 0;

    void Start()
    {
        progress = 0;
        _gameVariableConnector = GameVariableConnector.instance;
        _createJobScript = CreateJob.instance;
    }

    // Update is called once per frame
    void Update()
    {
        progress = Mathf.Lerp(progress, _targetValue, Time.deltaTime * _increaseSpeed);
        progress = Mathf.Clamp(progress, 0, 100);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            int random = Random.Range(1, 10);
            IncreaseProgress(20f, random);
        }

        GetComponent<Slider>().value = progress;

        if (CheckIfProjectIsFinished())
        {
            _gameVariableConnector.economyManagerScript.AddToBalance(1000);
            progress = 0;
            Slider thisSlider = gameObject.GetComponent<Slider>();
            _createJobScript.CompleteTask(thisSlider, taskID);
        }
    }

    public void ChangeTaskID(int newID)
    {
        taskID = newID;
    }

    public void IncreaseProgress(float percentageAmount, float increaseSpeed)
    {
        _targetValue = progress + percentageAmount;
        _increaseSpeed = increaseSpeed;
    }

    public bool CheckIfProjectIsFinished()
    {
        if (progress >= 100)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
