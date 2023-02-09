using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; 

namespace GGJ2023
{
    public class UIManager : MonoBehaviour
    {
        private static readonly string scoreTextValue = "You made {0} Points!"; 

        #region Fields and Properties
        public static UIManager Instance = null;

        [Header("In Game")]
        [SerializeField] private Image previsualisationImage;
        [SerializeField, Range(0f,1f)] private float transitionDuration = .25f;

        [Header("Canvas")]
        [SerializeField] private CanvasGroup mainMenu;
        [SerializeField] private CanvasGroup inGameMenu;
        [SerializeField] private CanvasGroup endGameMenu;

        [Header("Endgame")]
        [SerializeField] private TMPro.TextMeshProUGUI scoreText; 
        #endregion


        #region Private Methods
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else 
                Destroy(this);

            GameManager.OnGameReady += HideMainMenu; 
            GameManager.OnGameStarted += DisplayInGameMenu;
            GameManager.OnGameEnded += DisplayEndGameMenu; 
        }

        #endregion

        #region Public Methods
        private Sequence previsualisationSequence; 
        public void SetPrevisualisation(Sprite _tileSprite)
        {
            previsualisationImage.sprite = _tileSprite; 
            previsualisationImage.enabled= true;
            AudioManager.Instance.PlaySFX(AudioManager.Instance.DrawClip, 0.8f);
            if (previsualisationSequence.IsActive()) previsualisationSequence.Kill(true); 
            previsualisationSequence = DOTween.Sequence();
            previsualisationSequence.Join(previsualisationImage.transform.DOLocalMoveX(0f, transitionDuration).SetEase(Ease.InOutBack));
            previsualisationSequence.Join(previsualisationImage.DOFade(1f, transitionDuration).SetEase(Ease.InSine));
        }

        public Sequence RemovePrevisualisation()
        {
            previsualisationSequence = DOTween.Sequence();
            previsualisationSequence.Append(previsualisationImage.DOFade(0f, transitionDuration).SetEase(Ease.OutSine));
            previsualisationSequence.Join(previsualisationImage.transform.DOLocalMoveY(previsualisationImage.rectTransform.rect.height * 2, transitionDuration).SetEase(Ease.InOutBack));
            previsualisationSequence.AppendCallback(OnPrevisualisationRemoved);
            void OnPrevisualisationRemoved()
            {
                previsualisationImage.enabled = false;
                previsualisationImage.transform.localPosition = new Vector3(previsualisationImage.rectTransform.rect.width * 2,0f,0f);
            }

            return previsualisationSequence; 
        }

        public void ResetPrevisualisationRotation() => previsualisationImage.transform.localRotation = Quaternion.identity; 

        public Sequence RotatePrevisualisation(int _rotation)
        {
            previsualisationSequence = DOTween.Sequence();
            previsualisationSequence.Append(previsualisationImage.transform.DOLocalRotate(Vector3.forward * _rotation, transitionDuration).SetEase(Ease.InOutBack));
            
            return previsualisationSequence; 
        }

        public void SetScore(int _score)
        {
            scoreText.text = string.Format(scoreTextValue, _score); 
        }

        private void HideMainMenu()
        {
            Sequence _transition = DOTween.Sequence();
            _transition.Join(mainMenu.DOFade(0f, .75f));
        }
        public void DisplayInGameMenu()
        {
            Sequence _transition = DOTween.Sequence();
            _transition.Join(inGameMenu.DOFade(1f, .75f)); 
        }

        public void DisplayEndGameMenu()
        {
            Sequence _transition = DOTween.Sequence();
            _transition.Join(inGameMenu.DOFade(0f, .75f));
            _transition.Join(endGameMenu.DOFade(1f, .75f)); 
        }
        #endregion
    }
}
