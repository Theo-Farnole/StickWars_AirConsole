using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialTextBoxManager : MonoBehaviour
{
    #region Fields
    [SerializeField] private TextMeshProUGUI _textBox;
    [Space]
    [SerializeField] private float _displayDuration;
    [SerializeField] private float _timeBetweenTwoQueuedMessages = 0.3f;

    private Queue<string> _queuedTextsBoxes = new Queue<string>();
    private float _timerTextBoxDisplay = 0;

    private string _currentTextBoxContent = string.Empty;
    private bool _pauseTextBox = false;
    #endregion

    #region Properties
    public bool IsMessageDisplaying => !string.IsNullOrEmpty(_currentTextBoxContent);
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Start()
    {
        _textBox.enabled = false;    
    }

    void Update()
    {
        ManageTextBoxDisplay();
    }

    void OnEnable()
    {
        LevelLayoutManager.Instance.OnLevelLayoutAnimationStart += OnLevelLayoutAnimationStart;
        LevelLayoutManager.Instance.OnLevelLayoutAnimationEnd += OnLevelLayoutAnimationEnded;
    }

    void OnDisable()
    {
        if (LevelLayoutManager.Instance != null)
        {
            LevelLayoutManager.Instance.OnLevelLayoutAnimationStart += OnLevelLayoutAnimationStart;
            LevelLayoutManager.Instance.OnLevelLayoutAnimationEnd += OnLevelLayoutAnimationEnded;
        }
    }
    #endregion

    #region Events Handler
    void OnLevelLayoutAnimationStart(LevelLayoutManager levelLayoutManager)
    {
        _pauseTextBox = true;
        _textBox.enabled = false;
    }

    void OnLevelLayoutAnimationEnded(LevelLayoutManager levelLayoutManager)
    {
        _pauseTextBox = false;
        _timerTextBoxDisplay = 0; // reset timer
        _textBox.enabled = true;
    }
    #endregion

    #region Public methods
    public void EnqueueTutorialTextBox(string message)
    {
        // don't have to wait to display message
        if (_queuedTextsBoxes.Count == 0 && !IsMessageDisplaying)
        {
            DisplayTextBox(message);
        }
        else
        {
            // otherwise, add message to queue
            _queuedTextsBoxes.Enqueue(message);
        }
    }
    #endregion

    #region Methods managing messages display
    void ManageTextBoxDisplay()
    {
        if (_pauseTextBox)
            return;

        // don't manage text box, if no one is displaying
        if (!IsMessageDisplaying)
            return;

        _timerTextBoxDisplay += Time.deltaTime;

        // is timer over ?
        if (_timerTextBoxDisplay >= _displayDuration)
        {
            _timerTextBoxDisplay = 0;
            DisplayNextTextBox();
        }
    }

    void DisplayNextTextBox()
    {
        // is a message is waiting to be displayed ?
        if (_queuedTextsBoxes.Count > 0)
        {
            this.ExecuteAfterTime(_timeBetweenTwoQueuedMessages, () =>
            {
                var nextMessage = _queuedTextsBoxes.Dequeue();
                DisplayTextBox(nextMessage);
            });
        }
        else
        {
            // disable text component
            if (_textBox.enabled)
                _textBox.enabled = false;

            _currentTextBoxContent = string.Empty;
        }
    }

    void DisplayTextBox(string message)
    {
        if (!_textBox.enabled)
            _textBox.enabled = true;

        _currentTextBoxContent = message;
        _textBox.text = _currentTextBoxContent;
    }
    #endregion
    #endregion
}
