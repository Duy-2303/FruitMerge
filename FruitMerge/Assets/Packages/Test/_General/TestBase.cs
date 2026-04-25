using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link
{
    public class TestBase : MonoBehaviour
    {
        protected Sprite _hint = null;
        public static TestBase instance { get; private set; }

        public bool IsAllowInteract => true;


        private Cooking.TestUI _testUI;
        private bool _isHaveUI => _testUI != null;

        protected virtual void Awake()
        {
            instance = this;
            _testUI = GameObject.FindObjectOfType<Cooking.TestUI>();
        }

        protected virtual void Start()
        {
            HeartLevel = 10;
            if (_isHaveUI) _testUI.SetHeart(HeartLevel);
        }
        protected virtual void Update()
        {
            
        }
        public void WinGame()
        {
            Debug.LogError("Winnnnnnnnnnnnnnnn!");
        }

        private int _heartLevel = 0;
        public int HeartLevel
        {
            get => _heartLevel;
            set => _heartLevel = value;
        }

        public void LoseFullHeart() => HeartLevel -= 2;
        public void LoseFullHeart(Vector3 position, float scale = 1f)
        {
            HeartLevel -= 2;
            // TODO: show heart break anim at position and with specificc scale (if need)
            if (_isHaveUI) _testUI.SetHeart(HeartLevel);
        }

        public void LoseHalfHeart() => HeartLevel -= 1;
        public void LoseHalfHeart(Vector3 position, float scale = 1f)
        {
            HeartLevel -= 1;
            // TODO: show heart break anim at position and with specificc scale (if need)
            if (_isHaveUI) _testUI.SetHeart(HeartLevel);
        }

        public void AddFullHeart() => HeartLevel += 2;
        public void AddFullHeart(Vector3 position, float scale = 1f)
        {
            HeartLevel += 2;
            // TODO: show heart anim at position and with specificc scale (if need)
            if (_isHaveUI) _testUI.SetHeart(HeartLevel);
        }

        public void AddHalfHeart() => HeartLevel += 1;
        public void AddHalfHeart(Vector3 position, float scale = 1f)
        {
            HeartLevel += 1;
            // TODO: show heart anim at position and with specificc scale (if need)
            if (_isHaveUI) _testUI.SetHeart(HeartLevel);
        }

        private int _currentStep;
        public event System.Action<int> onStepChanged = null;
        public void IncreaseStep() => SetStep(_currentStep + 1);
        public void SetStep(int value)
        {
            if (_currentStep == value) return;
            _currentStep = value;
            onStepChanged?.Invoke(value);
        }
        public int CurrentStep => _currentStep;

    }
}