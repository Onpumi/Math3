using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimationItem : MonoBehaviour
{
     [SerializeField] private MoveItem _moveitem;

                      private const int countMaxSwap = 2;

      private void OnEnable()
    {
        _moveitem.OnSwapItems+=DoSwapItem;
        _moveitem.OnMoveItem+=MoveItem;
        _moveitem.CutItem+=PlayCut;

    }
    
     private void OnDisable()
    {
        _moveitem.OnSwapItems-=DoSwapItem;
        _moveitem.OnMoveItem-=MoveItem;
        _moveitem.CutItem-=PlayCut;
    }

    private void PlayCut(PlayItem item)
    {
      item.SetTransparent(0.5f);
    }

    private void DoSwapItem(PlayItem firstItem, PlayItem secondItem)
    {
        Sequence posSeq;
        posSeq = DOTween.Sequence();
        posSeq
         .AppendCallback(() =>
        {
          firstItem.transform.localPosition = new Vector3(firstItem.transform.localPosition.x, firstItem.transform.localPosition.y, 10);
        })
         .Join(firstItem.transform.DOLocalMove(firstItem.target, 0.3f))
         .Join(secondItem.transform.DOLocalMove(secondItem.target, 0.3f))
         .SetEase(Ease.Linear)
  	   .OnComplete(() => 
  	   {
            _moveitem.FinishSwap();
  	   });
    }

    private void MoveItem(PlayItem item)
    {
       if( item )  
       {
        DOTween.Sequence()
          .AppendCallback(() =>
          {
            if( item ) 
            {
              item.transform.localPosition = new Vector3(item.transform.localPosition.x, item.transform.localPosition.y, 10);
            }
          })
          .Join(item.transform.DOLocalMove(item.target,0.8f))
          .SetEase(Ease.Linear)
          .OnComplete(() =>
          {
            
          });
       }
    }
}
