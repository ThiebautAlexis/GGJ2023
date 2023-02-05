using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; 

namespace GGJ2023
{
    public class UIManager : MonoBehaviour
    {
        #region Fields and Properties
        public static UIManager Instance = null;

        [Header("In Game")]
        [SerializeField] private Image previsualisationImage;
        [SerializeField, Range(0f,1f)] private float transitionDuration = .25f; 
        #endregion


        #region Private Methods
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else 
                Destroy(this); 
        }
        #endregion

        #region Public Methods
        public void SetPrevisualisation(Sprite _tileSprite)
        {
            previsualisationImage.sprite = _tileSprite; 
            previsualisationImage.enabled= true;
            AudioManager.Instance.PlaySFX(AudioManager.Instance.DrawClip, 0.8f); 
            Sequence _sequence = DOTween.Sequence();
            _sequence.Join(previsualisationImage.transform.DOLocalMoveX(0f, transitionDuration).SetEase(Ease.InOutBack));
            _sequence.Join(previsualisationImage.DOFade(1f, transitionDuration).SetEase(Ease.InSine));
        }

        public Sequence RemovePrevisualisation()
        {
            Sequence _sequence = DOTween.Sequence();
            _sequence.Append(previsualisationImage.DOFade(0f, transitionDuration).SetEase(Ease.OutSine));
            _sequence.Join(previsualisationImage.transform.DOLocalMoveY(previsualisationImage.rectTransform.rect.height * 2, transitionDuration).SetEase(Ease.InOutBack)); 
            _sequence.AppendCallback(OnPrevisualisationRemoved);
            void OnPrevisualisationRemoved()
            {
                previsualisationImage.enabled = false;
                previsualisationImage.transform.localPosition = new Vector3(previsualisationImage.rectTransform.rect.width * 2,0f,0f);
            }

            return _sequence; 
        }

        public Sequence RotatePrevisualisation()
        {
            Sequence _sequence = DOTween.Sequence();
            float _targetRotation = previsualisationImage.transform.localEulerAngles.z + 90;
            if (_targetRotation >= 360) _targetRotation = 0f; 
            _sequence.Append(previsualisationImage.transform.DOLocalRotate(Vector3.forward * _targetRotation, transitionDuration).SetEase(Ease.InOutBack));
            
            return _sequence; 
        }
        #endregion
    }
}
