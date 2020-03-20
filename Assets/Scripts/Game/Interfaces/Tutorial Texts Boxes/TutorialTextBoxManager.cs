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
