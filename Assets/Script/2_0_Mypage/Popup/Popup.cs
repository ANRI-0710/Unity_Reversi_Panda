using UnityEngine;
using DG.Tweening;

/// <summary>
/// ポップアップのオープン・クローズのアニメーション
/// </summary>
public class Popup : MonoBehaviour
{

    public void PopupStart(GameObject popupcanvas)
    {

        SoundManager.Instance.SE_Popup();
        popupcanvas.transform.localScale = new Vector3(0f,0f,0f);
        popupcanvas.transform.DOScale(1f, 0.2f);


    }


    public void Popup_Close(GameObject popupcanvas) {

        SoundManager.Instance.SE_Cansel();
        Sequence seq = DOTween.Sequence();
        seq.Append(popupcanvas.transform.DOScale(0f, 0.2f));
        seq.OnComplete(() => FalseWindow(popupcanvas));
     
        seq.Play();

    
    }

    public void FalseWindow(GameObject popupcanvas) {
     
        popupcanvas.SetActive(false);
        
    }


    

}
