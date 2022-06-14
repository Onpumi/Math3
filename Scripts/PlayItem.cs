using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;



     
	public class PlayItem : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
    {
	    private int _indexColumn;
	    private int _indexRow;
	    private Vector3 _target;
		private Vector3 _direction = Vector3.zero;
	    private bool _isMove;
		private int  _statusChange = 0;
	    private SpriteRenderer _sprite;
		private bool _isRemove = false;
		private bool _isDestroy = false;
		private Image _image;
		private float _stepHorizontal;
		private float _stepVertical;
		private Vector3[,] _gridMatrix;
		public  bool isRemove => _isRemove;
		public bool isDestroy => _isDestroy;
		public int change => _statusChange;
	    public Vector3 target    => _target;
	    public bool  isMove	     => _isMove;
	    public  int  indexColumn => _indexColumn;
	    public  int  indexRow    => _indexRow;
		public Vector3  positionItem => this.transform.localPosition;
  	    public event UnityAction<PlayItem> mouseDown;
 	    public event UnityAction<PlayItem> mouseUp;
	    public event UnityAction<PlayItem> mouseMove;
		public event UnityAction<PlayItem,PlayItem> OnSwapItems;
	    public event UnityAction<PlayItem> OnMoveItem;
		public event UnityAction<PlayItem> CutItem;

	  private void OnEnable()
	  {

	  }

	  private void OnDisable()
	  {

	  }

	  public void AnimationCut()
	  {
		CutItem?.Invoke(this);
	  }


	  public void ReadyNewPosition( int column, int row)
	{
		_target = _gridMatrix[column,row];
	}
      public void SetState( int column, int row, float sizeCellX, float sizeCellY, Vector3[,] positionCells )
	 {
		 _gridMatrix = positionCells;
		_stepHorizontal = sizeCellX;
		_stepVertical = sizeCellY;
	    _indexColumn = column;
	    _indexRow = row;
		_image = GetComponent<Image>();
		_isMove = false;
		_isRemove = false;
		_isDestroy = false;
 	 }
	 public void SetTarget( PlayItem item )
	 {
		_target = item.transform.localPosition;
	 }
      public void SetTarget( Vector3 positionItem )
	 {
		_target = positionItem;
	 }
	  public void SetChange( int change )
	 {
		 _statusChange = change;
	 }
	 public void SetMoveItem( bool state )
	 {
	   _isMove = state;
	 }

	 public void DestroyIt()
	 {
		if(this.gameObject)
		{
		 Destroy(this.gameObject);
		}
	 }
	  
     public void SetIndexes(PlayItem item)
     {
		_indexRow = item.indexRow;
		_indexColumn = item.indexColumn;
	 }

	 public void SpriteOrder( string value )
	 {
	   _sprite.sortingLayerName = value;
     }

	 public void RemoveItem()
	 {
		 if(isRemove)
		 {
			 DestroyIt();
		 }
	 }

	
	 public void SetTransparent( float delta )
	 {
	    var Color = _image.color;
	    Color.a = delta;
		Color.a = Mathf.Clamp(Color.a,0,1);
		_image.color = Color;
	 }

	  
	  
	  public void MarkForRemove( bool value )
	 {
	    _isRemove = value;
		//SetTransparent(0.5f);
		//this.transform.gameObject.SetActive(false);
	 }

	 public void SetIndexes( int column, int row )
	 {
         _indexColumn = column;
		 _indexRow = row;
	 }

	 public void SetPosition( Vector3 offset )
	 {
		 transform.localPosition += offset;
	 }
    

	 public void OnPointerDown(PointerEventData e)
	 {
		mouseDown?.Invoke(this);
	 }

	 public void OnPointerEnter(PointerEventData e)
	 {
		mouseMove?.Invoke(this);
	 }

	  public void OnPointerUp(PointerEventData e)
	 {
		mouseUp?.Invoke(this);
	 }
	  private void OnMouseDown()
	 {
	    mouseDown?.Invoke(this);
     }

	  private void OnMouseUp()
	 {
   	    mouseUp?.Invoke(this);
	 }

	  private void OnMouseEnter()
	 {
	   mouseMove?.Invoke(this);
	 }
	 private void OnDestroy()
	 {
		 _isDestroy = true;
	 }

	 private void Update() {
	 }
}





