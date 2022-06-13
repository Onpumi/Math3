using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Items : MonoBehaviour
{
	  [SerializeField] private Transform _parentItem;
	   public PlayItem[,] _playItems;
	   private Vector3[,] _positionCells;
	   private Vector3 _startPosition;
	   private Vector2 _scaleSprite;
	   private Vector3 _currentPos;
	   private float _sizeGrid;
	   private float _countCells; 
   	   private Vector2  _scale;
	   private float _deltaField = 0.5f;
	   public Vector3[,] positionCells => _positionCells;
	   public float sizeCellX => (_scaleSprite.x * scaleItem.x);
	   public float sizeCellY => (_scaleSprite.y * scaleItem.y);
	   public  Vector2 scaleItem => _scale;
	   public float countCells => _countCells;
	   public float sizeGrid => _sizeGrid;
	   public Transform parentItem => _parentItem;
	   public Vector2 scaleSprite => _scaleSprite;
	   public Vector3 startPosition => _startPosition;

	  public void Init( float sizeBorder, Vector2 positionItem, float countCells )
	{
		  CanvasScaler canvasScaler = _parentItem.gameObject.GetComponent<CanvasScaler>();
		 var referencePixelsPerUnit = canvasScaler.referencePixelsPerUnit;
		 var scaleIndex = referencePixelsPerUnit;
	    _startPosition = positionItem;
	    _scaleSprite = new Vector2(1f,1f);
		 _scale = new Vector2(scaleIndex,scaleIndex);
 	    _scaleSprite.x *= scaleItem.x;  
   	    _scaleSprite.y *= scaleItem.y;  
	    _countCells = countCells;
	    _scaleSprite.x = (sizeBorder - _deltaField) / (_countCells * scaleItem.x);
	    _scaleSprite.y = (sizeBorder - _deltaField) / (_countCells * scaleItem.y);
	   _sizeGrid = _countCells * _scaleSprite.x * scaleItem.x ;
	   _startPosition.x -= _sizeGrid / 2 - _scaleSprite.x * scaleItem.x / 2;
	   _startPosition.y += _sizeGrid / 2 - _scaleSprite.y * scaleItem.y / 2;
	   _currentPos = _startPosition;
	   _playItems = new PlayItem[(int)_countCells, (int)_countCells];
	   _positionCells = new Vector3[(int)_countCells, (int)_countCells];

	   
	   
	   for( int j = 0; j < _countCells ; j++)
	  {
	     _currentPos.x = _startPosition.x;
	     _currentPos.z = 0;
	      for( int i = 0; i < _countCells ; i++ )
         {  
			_playItems[i,j] = CreateItem( _parentItem, _currentPos, _scaleSprite, i, j );
			_positionCells[i,j] = _playItems[i,j].transform.localPosition;
	        _currentPos.x += (_scaleSprite.x * scaleItem.x);
	     }
	     _currentPos.y -= (_scaleSprite.y * scaleItem.y);
	  }

	}

	private bool Test3( string name, int i, int j )
	{
	   var findIdentityItem = ( i >=2 && name == _playItems[i-1,j].name && name == _playItems[i-2,j].name);
	   findIdentityItem |= ( j >=2 && name == _playItems[i,j-1].name && name == _playItems[i,j-2].name);
	   return findIdentityItem;
	}

	private PlayItem CreateItem( Transform parentItems, Vector3 positionItem, Vector2 scaleItem, int i, int j )
	{
		 GameObject randomItemObject = GetRandomItem( parentItems, i, j );
		 var nameObject = randomItemObject.name;
		 var itemObject = Instantiate(  randomItemObject, positionItem, Quaternion.identity);
		 itemObject.transform.localPosition = positionItem;
		 //itemObject.transform.position = positionItem;
		 itemObject.transform.SetParent(this.transform);
		 itemObject.transform.localScale = scaleItem;
		 PlayItem playItem = itemObject.GetComponent<PlayItem>() as PlayItem;
		 playItem.SetState(i, j, sizeCellX, sizeCellY, _positionCells);
		 playItem.name = nameObject;
	     return playItem;
	}


	 private GameObject GetRandomItem( Transform parentItems, int i, int j )
      {
	     int index = Random.Range(0,parentItems.childCount);
	      while( Test3(parentItems.GetChild(index).name, i, j) ) 
		 {
			  index = Random.Range(0,parentItems.childCount);
	     }
	    return parentItems.GetChild(index).gameObject;
      }

}
