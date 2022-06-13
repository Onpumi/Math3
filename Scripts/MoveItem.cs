using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Threading;
using System.Threading.Tasks;

public class MoveItem : MonoBehaviour
{
	[SerializeField] private Transform parentItems;
	[SerializeField] private Items _gridItem;
	[SerializeField] private Transform _parentSpawnItems;
	private PlayItem[,] _items;
	private PlayItem _firstItem;
	private PlayItem _secondItem;
	private int _stepMove = (int)StatesMove.Stop;
	private float _countCells;
	private bool _isUpdateItem = false;
	private bool _isTestDelete = false;
	public int statusMove => _stepMove;	
	public bool isUpdateItem => _isUpdateItem;
	public float countCells => _countCells;
	private bool _isBlock = false;
	public PlayItem _destroyItem;
	public bool blockInput => _isBlock;

	public event UnityAction<PlayItem,PlayItem> OnSwapItems;
	public event UnityAction<PlayItem> OnMoveItem;

	
	  private void OnEnable()
	{
	   _countCells = _gridItem.countCells;
	   _items = _gridItem._playItems;

	  for( int j = 0 ; j < _countCells; j++ )	   
	  {
	   for( int i = 0 ; i < _countCells; i++ )	   
	   {
		 _items[i,j].mouseDown += PressItem;
		 _items[i,j].mouseMove += MoveElement;
	   }
	 }
		_firstItem = _items[0,0];
		_secondItem = _items[0,0];
	}

	private void OnDisable()
	{
	   for( int j = 0 ; j < _countCells; j++ )	   
	   for( int i = 0 ; i < _countCells; i++ )	   
	   {
		 _items[i,j].mouseDown -= PressItem;
		 _items[i,j].mouseMove -= MoveElement;
	   }
	}


	
	private void Init(PlayItem[,] items)
	{
		_items = items;
	}

	private void PressItem(PlayItem item)
	{ 

		if( blockInput ) return;

		if( _firstItem.change == 0 )
	   { 
			  _firstItem = item;
		 	  _firstItem.SetChange(1);

	   }
	   else if( _firstItem.change == 1 && _firstItem != item ) 
	   {
		   _secondItem = item;
		   _secondItem.SetChange(2);
		   _firstItem.SetChange(1);
			if(ItNeighboor( _firstItem, _secondItem ) && _secondItem.change == 1 + _firstItem.change) 
			{
			    StartSwap();
			    TrySwapNeighboor();
		    }
	   }
	}


	 private void MoveElement( PlayItem item )
	{ 
		
		if( blockInput ) return;

		var i = item.indexColumn;
		var j = item.indexRow;
		//Debug.Log($"Индексы элемента {item.indexColumn},{item.indexRow}  Его имя {item.name} Его координаты {item.transform.position.x}, {item.transform.position.y}  IsRemove={item.isRemove}");

		if( _firstItem.change == 1 && _firstItem != item   ) 
	   {
		   _secondItem = item;
		   _secondItem.SetChange(2);
		   _firstItem.SetChange(1);
			if(ItNeighboor( _firstItem, _secondItem ) && _secondItem.change == 1 + _firstItem.change) 
			{
			    StartSwap();
				TrySwapNeighboor();
		    }
	   }
	}

	  private bool ItNeighboor( PlayItem change, PlayItem target )
	{
		var positionChange = change.transform.localPosition;
		var positionTarget = target.transform.localPosition;
	    var testX = ( Mathf.Abs(change.positionItem.x-target.positionItem.x) <= 0.5 ) && (change.positionItem.y == target.positionItem.y) ? (true) : (false);
	    var testY = ( Mathf.Abs(change.positionItem.y-target.positionItem.y) <= 0.5 ) && (change.positionItem.x == target.positionItem.x) ? (true) : (false);
		return testX || testY;
	}

	  private void GoSwapNeighboor()
	{
		   StartSwap();
		  _firstItem.SetMoveItem(true);
		  _secondItem.SetMoveItem(true);
   	      OnSwapItems?.Invoke(_firstItem,_secondItem);
  	      _stepMove++;
	}

      public void TrySwapNeighboor( )
    {
      if( statusMove < 2  )
	  {
		if( _firstItem.isMove == false && _secondItem.isMove == false )
		{
		  //BlockItems(true);
		  GoSwapNeighboor();
		}
	  }
	  else
	  {
		_stepMove = 0;
	  }
    }

	 	private bool DeleteSeveralItems( PlayItem[] items, int indexCenterItem )
	{
		List<PlayItem> itemsForDelete = new List<PlayItem>();
		int countSizeItem = 0;
		int countItemRemove = 1;
		bool isRemove = false;

  	  	  for( int i = indexCenterItem+1 ; i < items.Length ; i++ )
		{
			if( items[i].name != items[i-1].name )
			{
				break;
			}
			else
			{
			  itemsForDelete.Insert(countSizeItem++, items[i] );
			  countItemRemove++; 
			}
		}

		for( int i = indexCenterItem-1 ; i >= 0 ; i-- )
		{
			if( items[i].name != items[i+1].name)
			{
				break;
			}
			else
			{
			  itemsForDelete.Insert(countSizeItem++, items[i] );
			  countItemRemove++; 
			}
		}

		if(countSizeItem + 1 >= 3)
		{
			isRemove = true;
			for(int i = 0 ; i < countSizeItem ; i++)
			{
			   itemsForDelete[i].MarkForRemove( true );
			}
		}
		return isRemove;
	}

	 	private bool DeleteIdenticalAll( PlayItem item )
	{
		int row = item.indexRow;
		int column = item.indexColumn;
		int lengthRow = _items.GetLength(0);
		PlayItem[] itemsHorizontal = Enumerable.Range(0,lengthRow).Select(x => _items[x,row]).ToArray();
		PlayItem[] itemsVertical = Enumerable.Range(0,lengthRow).Select(y => _items[column,y]).ToArray();
		return DeleteSeveralItems( itemsHorizontal, column ) | DeleteSeveralItems( itemsVertical, row );
	}

    private bool RemoveItemsDo( PlayItem item )
	{
		_isTestDelete = DeleteIdenticalAll( item );
		if( _isTestDelete )
		{
		   item.MarkForRemove( true );
		   UpdateItems();
	    }
		else
		{
			TrySwapNeighboor();
		}
		var test = _isTestDelete;
		return test;
	}

	 public void ReplaceItem(PlayItem firstItem, PlayItem secondItem)
    {
	   PlayItem bufferItem;
	   int column1   = firstItem.indexColumn;
	   int row1      = firstItem.indexRow;
	   int column2   = secondItem.indexColumn;
	   int row2      = secondItem.indexRow;
 	    bufferItem    =  firstItem;
	    firstItem = secondItem;
	    secondItem = bufferItem;
        firstItem.SetIndexes(column1,row1);
        secondItem.SetIndexes(column2,row2);
		_items[column1,row1] = firstItem;
		_items[column2,row2] = secondItem;
		
	}



	private void MoveNewPositionItem( PlayItem[,] gridItems, PlayItem item, int column, int row )
	{
		item.ReadyNewPosition(column,row);
		int row2 = item.indexRow;
		int column2 = item.indexColumn;
		item.SetIndexes(column,row);
		gridItems[column,row].SetIndexes(column2,row2);
		OnMoveItem?.Invoke(item);
		PlayItem buffItem;
		buffItem = gridItems[column,row];
		gridItems[column,row] = gridItems[column2,row2];
		gridItems[column2,row2] = buffItem;
	}


	public void DestroyEmptyItems()
	{
		for(int i = 0 ; i < _parentSpawnItems.childCount ; i++)
		{
			if(_parentSpawnItems.GetChild(i).transform.gameObject.activeSelf == false)
			{
				Destroy(_parentSpawnItems.GetChild(i).transform.gameObject,10f);
			}
		}
	}
	


	private void FallSpawnItems( PlayItem[,] gridItems, PlayItem item, int column, int row)
	{
		gridItems[column,row].transform.gameObject.SetActive(false);
		gridItems[column,row].name = "empty";

		gridItems[column,row] = item;
		gridItems[column,row].SetIndexes(column,row);
		gridItems[column,row].ReadyNewPosition(column,row);
		OnMoveItem?.Invoke(gridItems[column,row]);
	
	}



     private GameObject RandomNewItem( Transform parentItems )
      {
	    int index = Random.Range(0,parentItems.childCount);
   	    index = Random.Range(0,parentItems.childCount);
	    return parentItems.GetChild(index).gameObject;
      }

   
     private GameObject CreateItem( Vector3 positionItem )
	{
		 GameObject randomItemObject = RandomNewItem( parentItems );
		 var nameObject = randomItemObject.name;
		 var itemObject = Instantiate(  randomItemObject, positionItem, Quaternion.identity);
		 itemObject.transform.position = positionItem;
		 itemObject.transform.SetParent(this.transform);
		 itemObject.transform.localScale = _gridItem.scaleSprite;
		 itemObject.name = nameObject;
	     return itemObject;
	}


    private void CreateNewItems( PlayItem[] items, int spawnColumn, int countEmptyItems )
	{
		PlayItem[] spawnItems = new PlayItem[countEmptyItems];
		Vector3 startPosition = _gridItem.startPosition;
		Vector3 createPosition = startPosition;

				
		var scaleItem = new Vector2(_gridItem.sizeCellX, _gridItem.sizeCellY);
		
		//createPosition = new Vector3(_gridItem.sizeCellX, _gridItem.sizeCellY + scaleItem.y, 0 );
		//createPosition = new Vector3(0,0,0);

		for( int j = 0 ; j < countEmptyItems ; j++ )
		{
			var spawnRow = countEmptyItems - 1 -j;			
			createPosition.y += scaleItem.y;
		    createPosition.x = items[j].transform.position.x;
			var itemObject = CreateItem(createPosition);
		    spawnItems[j] = itemObject.GetComponent<PlayItem>() as PlayItem;
			spawnItems[j].SetState(spawnColumn, spawnRow, _gridItem.sizeCellX, _gridItem.sizeCellY, _gridItem.positionCells);
			Vector3 spawnPositionItem = spawnItems[j].transform.localPosition;
			spawnPositionItem.y -= scaleItem.y * countEmptyItems;
			spawnItems[j].mouseDown += PressItem;
			spawnItems[j].mouseMove += MoveElement;
			FallSpawnItems( _items, spawnItems[j], spawnColumn, spawnRow);
			
			//Debug.Log(seq.Count);

			//Thread.Sleep(500);
		}

	}


	private void MoveSetFullItem( PlayItem firstItem, PlayItem secondItem )
	{
		 firstItem.SetTarget(secondItem);
		 OnMoveItem?.Invoke(firstItem);
	}




    private void UpdateItems()
	{
		for(int i = 0 ; i < _items.GetLength(0); i++ )
		{
			
			PlayItem[] items = Enumerable.Range(0,_items.GetLength(1)).Select(x => _items[i,x]).ToArray();
			
			var countOffset = 0;


			int indexFirstEmpty = 0;
			int indexLastEmpty = 0;
			int indexFirstFull = 0;
			int indexLastFull = 0;
			int countEmpty = 0;
			int countFull = 0;
			var j = 0;
			int NumExitWhile = 0;
			
			   while( j < items.Length) 
			{
				  NumExitWhile++;
				  if(NumExitWhile > 1000) 
				{ 
					
					Debug.Log($"NumExitWhile {NumExitWhile}   i = {i}  countEmpty = {countEmpty} countFull = {countFull}"); return; 
					Debug.Log($"indexFirstFull={indexFirstFull} indexLastFull={indexLastFull}");
				}

				if( j == 0 && items[j].isRemove == true )
				{
					 j++;
					continue;
				}
				else if( j == 0 && items[j].isRemove == false )
				{
					indexFirstFull = 0;
				}

				if( j > 0 && items[j].isRemove == false ) 
				{
					if( items[j-1].isRemove == true ) 
					{
					   indexFirstFull = j;
					}
					if( ( j < items.Length-1 && items[j+1].isRemove == true)  )
					{
					   indexLastFull = j;
					}
				}

			     if( j > 0 && items[j].isRemove == true )
				{
					if( items[j-1].isRemove == false ) 
					{
					   indexFirstEmpty = j;
					}
					if( j == items.Length-1 && items[j].isRemove == true )
					{
						indexLastEmpty = j;
					}
					if(  (j < items.Length-1 && items[j+1].isRemove == false) || j == items.Length-1  )  // тут ругается на нуль если j>Length, нужно передеать!
					{
					   indexLastEmpty = j;
					   if( indexLastFull > 0 )
					   {
					     countEmpty = indexLastEmpty - indexLastFull;
					   }
					   else if( indexLastFull == 0 )
					   {
						 countEmpty = indexLastEmpty + 1;
					   }
					   countFull = 1 + indexLastFull - indexFirstFull;
					   var saveIndexLastFull = indexLastFull;
					   var saveIndexLastEmpty = indexLastEmpty;

						if(countEmpty > 0)
						{
					   	    while( indexLastFull >= 0 )
					       {
						     if(items[indexLastFull].transform.gameObject.activeSelf)
						    {
						  	   MoveNewPositionItem( _items, items[indexLastFull], i, indexLastEmpty);
						       indexLastEmpty--;
						     }
						     indexLastFull--;
					       }
						 
						}
					}
					//Debug.Log($"countEmpty={countEmpty} indexLastFull={indexLastFull} indexFirstFull={indexFirstFull} indexFirstEmpty={indexFirstEmpty} indexLastEmpty={indexLastEmpty} j={j}");
				}
				j++;
	    	}
			if( countEmpty > 0 ) 
			{
				//Debug.Log(countStartEmpty);
			CreateNewItems( items, i, countEmpty );
			}
		}

	}


     public void StartSwap()
    {
      _firstItem.SetTarget(_secondItem);
	  _secondItem.SetTarget(_firstItem);
    }

    private void ResetItem()
	{
       _firstItem.SetChange(0);
   	   _secondItem.SetChange(0);
       _firstItem.SetMoveItem(false);
	   _secondItem.SetMoveItem(false);
	}


     public void FinishSwap()
    {
	   int column   = _secondItem.indexColumn;
	   int row      = _secondItem.indexRow;

	  ResetItem();  
 	  ReplaceItem(_firstItem,_secondItem);
	   _isTestDelete = RemoveItemsDo( _firstItem );
	  if( _isTestDelete ) 
	  {
		  _stepMove = (int)StatesMove.Stop;
	  }
	  _isUpdateItem = false;
	  _isTestDelete = false;
	  BlockItems(false);
	  DestroyEmptyItems();

    }

	public void SetStateMove( int state )
	{
		_stepMove = state;
	}

  	  private void Update()
	{
	    if(Input.GetKey(KeyCode.Space))
		{
			//UpdateItems();
			Debug.Log(_parentSpawnItems.childCount);
		}

		if(Input.GetKey(KeyCode.Z))
		{
			DestroyEmptyItems();
		}
		
		if(Input.GetKey(KeyCode.A))
		{
	      for(int i = 0 ; i < _items.GetLength(0); i++ )
		{
			for(int j = 0; j < _items.GetLength(1); j++)
			{
				
				if(_items[i,j].isRemove) {
					_items[i,j].MarkForRemove(false);
					//Debug.Log($"{i},{j}");
				}
				if(_items[i,j] == null)
				{
					Debug.Log($"{i},{j}нулевой");
				}
				if(i != _items[i,j].indexColumn || j != _items[i,j].indexRow)
				{
					//_items[i,j].SetIndexes(i,j);
					Debug.Log($"несоответствие индексов {i},{j} {_items[i,j].indexColumn},{_items[i,j].indexRow} ");
				}

				}
				}
				}

			
	
		}
	

	public void BlockItems( bool value )
	{
	
		_isBlock = value;
	}

}


enum StatesMove
{
	Stop,
	stepOne,
	stepTwo
}