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
	   private float[] _sizeGrid;
	   private float _countCellsRow; 
	   private float _countCellsColumn; 
   	   private Vector2  _scaleItem;
	   private float _deltaField = 1f;
	   private float _deltaSpace = 0.1f;
	   public Vector3[,] positionCells => _positionCells;
	   public float sizeCellX => (_scaleSprite.x * _scaleItem.x);
	   public float sizeCellY => (_scaleSprite.y * _scaleItem.y);
	   public float countCellsRow => _countCellsRow;
	   public float countCellsColumn => _countCellsColumn;
	   public float sizeGridColumn => _sizeGrid[0];
	   public float sizeGridRow => _sizeGrid[1];
	   public Transform parentItem => _parentItem;
	   public Vector2 scaleSprite => _scaleSprite;
	   public Vector3 startPosition => _startPosition;

	   private Vector2 GetScaleSprite
         ( Vector2 scaleSprite, Vector2 scaleItem, float sizeBorder, float numberColumn,
		   float numberRow, float scaleIndex )  
	    { 
			  scaleSprite = new Vector2(1f,1f);
			  var width = numberColumn * scaleItem.x;
			  var height = numberRow * scaleItem.y;
			  var scaleX = sizeBorder / width;
			  var scaleY = sizeBorder / height;
			  var scale = Mathf.Min(scaleX,scaleY);
			  var minSize = Mathf.Min(width,height);
			  scaleSprite.x = scale;
			  scaleSprite.y = scale;
			  _deltaSpace = scale * 0.05f * scaleIndex;

			  //_deltaSpace = 0;
	  	  	  return scaleSprite;
	    }

	  public void Init( float sizeBorder, Vector2 positionItem, float countCellsColumn, float countCellsRow )
	{
		 CanvasScaler canvasScaler = _parentItem.gameObject.GetComponent<CanvasScaler>();
		 var referencePixelsPerUnit = canvasScaler.referencePixelsPerUnit;
		 var scaleIndex = referencePixelsPerUnit;
	    _startPosition = positionItem;
		 _scaleItem = new Vector2(scaleIndex,scaleIndex);
	    _countCellsRow = countCellsRow;
		_countCellsColumn = countCellsColumn;
        _scaleSprite = GetScaleSprite( _scaleSprite, _scaleItem, sizeBorder, 
		                               countCellsColumn, countCellsRow, scaleIndex );
	   _sizeGrid = new float[2];
	   _sizeGrid[0] = _countCellsColumn * _scaleSprite.x * _scaleItem.x ;
	   _sizeGrid[1] = _countCellsRow * _scaleSprite.y * _scaleItem.y ;
	   _startPosition.x -= _sizeGrid[0] / 2 - _scaleSprite.x * _scaleItem.x / 2 + _deltaSpace/2 * countCellsColumn;
	   _startPosition.y += _sizeGrid[0] / 2 - _scaleSprite.y * _scaleItem.y / 2 - _deltaSpace/2 * countCellsRow;
	   _currentPos = _startPosition;
	   _playItems = new PlayItem[(int)_countCellsColumn, (int)_countCellsRow];
	   _positionCells = new Vector3[(int)_countCellsColumn, (int)_countCellsRow];
	   
	   
	   for( int j = 0; j < _countCellsRow ; j++)
	  {
	     _currentPos.x = _startPosition.x;
	     _currentPos.z = 0;
	      for( int i = 0; i < _countCellsColumn ; i++ )
         {  
			_playItems[i,j] = CreateItem( _parentItem, _currentPos, _scaleSprite, i, j );
			_positionCells[i,j] = _playItems[i,j].transform.localPosition;
			_currentPos.x += _deltaSpace;
	        _currentPos.x += (_scaleSprite.x * _scaleItem.x);
	     }
		 _currentPos.y -= _deltaSpace;
	     _currentPos.y -= (_scaleSprite.y * _scaleItem.y);
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
