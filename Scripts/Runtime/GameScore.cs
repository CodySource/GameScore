using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace CodySource
{
    /// <summary>
    /// A simple game score tracker
    /// </summary>
    public class GameScore : MonoBehaviour
    {

        #region PROPERTIES

        /// <summary>
        /// The current visual score
        /// </summary>
        public int score = 0;

        /// <summary>
        /// Triggered whenever the score roll updates
        /// </summary>
        public UnityEvent<int> OnScoreUpdate = new UnityEvent<int>();

        /// <summary>
        /// Is the score roll-up animation used
        /// </summary>
        [Header("SCORE ROLL-UP")]
        public bool useScoreRoll = true;

        /// <summary>
        /// The amount the score is incrmented each time the roll step occurs
        /// </summary>
        public int rollFrameStep = 1;

        /// <summary>
        /// The number of times to update the score roll per second
        /// </summary>
        [Range(1, 60)] public int rollStepsPerSecond = 10;

        /// <summary>
        /// The internal animation time
        /// </summary>
        private float _stepTime = 0f;

        /// <summary>
        /// Triggered whenever the roll animation begins
        /// </summary>
        public UnityEvent OnScoreRollStart = new UnityEvent();

        /// <summary>
        /// The actual target score for the class
        /// </summary>
        private int _targetScore = 0;

        /// <summary>
        /// The optional labels available
        /// </summary>
        [Header("OPTIONAL LABEL")]
        [SerializeField] private List<Label> labels = new List<Label>();

        /// <summary>
        /// An optional label
        /// </summary>
        [System.Serializable]
        public struct Label
        {
            public string prefaceText;
            public TMP_Text text;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Sets the target score value & whether or not to use the roll-up animation
        /// </summary>
        public void SetScore(int pScore, bool pUseRollUp = false)
        {
            _targetScore = pScore;
            score = (pUseRollUp && useScoreRoll) ? score : pScore;
            _UpdateTextLabel();
        }

        /// <summary>
        /// Adds points to the score & begins the roll-up animation if desired
        /// </summary>
        public void AddPoints(int pValue)
        {
            //  Set target score
            _targetScore += pValue;
            if (_targetScore < 0) _targetScore = 0;
            if (useScoreRoll)
            {
                OnScoreRollStart?.Invoke();
                return;
            }

            //  Add points
            score += pValue;
            if (score < 0) score = 0;

            //  Update text
            _UpdateTextLabel();
            OnScoreUpdate?.Invoke(score);
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Performs text roll
        /// </summary>
        private void Update()
        {
            //  Breakout if complete / roll is not used
            if (!useScoreRoll || score == _targetScore) return;

            //  Increment step time
            _stepTime += Time.deltaTime;

            //  Roll the text
            if (_stepTime > (1f / rollStepsPerSecond))
            {
                _stepTime = 0f;

                //  Roll in the appropriate direction
                score += (score < _targetScore)? rollFrameStep : -rollFrameStep;

                //  Update label
                if (score == _targetScore) OnScoreUpdate?.Invoke(score);
                _UpdateTextLabel();
            }
        }

        /// <summary>
        /// Updates the optional text label
        /// </summary>
        private void _UpdateTextLabel()
        {
            labels.ForEach(l => l.text.text = $"{l.prefaceText}{score}");
        }

        #endregion

    }
}