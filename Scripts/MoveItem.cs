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
	public IGameState  _stateGame;
	private WaitingState stopMoveItem;
	private MovingState startMoveItem;
	private PlayItem[,] _items;
	private PlayItem[,] spawnItems;
	private PlayItem _firstItem;
	private PlayItem _secondItem;
	private int[] _countEmptyItems;
	private int _stepMove = (int)StatesMove.Stop;
	private float _countCells;
	private bool isAnimation = false;
	private bool _isUpdateItem = false;
	private bool _isTestDelete = false;
	public int statusMove => _stepMove;	
	public bool isUpdateItem => _isUpdateItem;
	public float countCells => _countCells;
	private bool _isClick = false;
	public PlayItem _destroyItem;

	public event UnityAction<PlayItem,PlayItem> OnSwapItems;
	public event UnityAction<PlayItem,PlayItem> OnMoveItem;
	public event UnityAction<PlayItem> CutItem;

	  public void freezeControlPlay()
	{
		_stateGame = startMoveItem;
	}
	 public void OpenControlPlay()
	{
		_stateGame = stopMoveItem;
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

	 public void ReplaceItems(PlayItem firstItem, PlayItem secondItem)
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

	  private void Awake()
	{
		 stopMoveItem = new WaitingState();
		 startMoveItem = new MovingState();
		_stateGame = stopMoveItem;
	}
	
	  private void OnEnable()
	{
	   _items = _gridItem._playItems;
	  // _countCells = _items.GetLength(0);

		var countRow = _items.GetLength(1);
		var countColumn = _items.GetLength(0);

	   _countEmptyItems = new int[countColumn];
	   	spawnItems = new PlayItem[countColumn,countRow];

	  for( int j = 0 ; j < countRow; j++ )	   
	  {
	   for( int i = 0 ; i < countColumn; i++ )	   
	   {
		 _items[i,j].mouseDown += PressItem;
		 _items[i,j].mouseMove += MoveElement;
		 _items[i,j].mouseUp += ReturnWaitingState;
	   }
	 }
		_firstItem = _items[0,0];
		_secondItem = _items[0,0];
	}

	private void OnDisable()
	{
	   for( int j = 0 ; j < _items.GetLength(1); j++ )	   
	   for( int i = 0 ; i < _items.GetLength(0); i++ )	   
	   {
		 _items[i,j].mouseDown -= PressItem;
		 _items[i,j].mouseMove -= MoveElement;
		 _items[i,j].mouseUp -= ReturnWaitingState;		 
	   }
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

    private void ReturnWaitingState( PlayItem item )
	{
		_isClick = false;

	 	if( _stateGame.isPlay() == true )
		{
			ResetItem();
			_stepMove = 0;
	   	    _isUpdateItem = false;
		    _isTestDelete = false;
			OpenControlPlay();
		}
	}

	private void PressItem(PlayItem item)
	{ 

		//Debug.Log($"_isClick={_isClick} _firstItem.change={_firstItem.change} _secondItem.change={_secondItem.change} _stateGame.isPlay()={_stateGame.isPlay()}");
		if(_stateGame.isPlay() == false) return;

		_isClick = true;


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
	//	Debug.Log($"_isClick={_isClick} _firstItem.change={_firstItem.change} _secondItem.change={_secondItem.change} _stateGame.isPlay()={_stateGame.isPlay()}");
	Debug.Log($" Индексы элемента {item.indexColumn},{item.indexRow}  Его имя {item.name} Его координаты {item.transform.position.x}, {item.transform.position.y}  IsRemove={item.isRemove}");
		if(_stateGame.isPlay() == false || _isClick == false) return;

		

		if( _firstItem.change == 1 && _firstItem != item  ) 
	   {
			if(ItNeighboor( _firstItem, item )) 
			{
			   _secondItem = item;
			   _secondItem.SetChange(2);
 	 	       _firstItem.SetChange(1);
			   if(_secondItem.change == 1 + _firstItem.change)
			   {
			    StartSwap();
				TrySwapNeighboor();
			   }
		    }
	   }
	}

	  private bool ItNeighboor( PlayItem itemFirst, PlayItem itemSecond )
	{
		var testX = ( Mathf.Abs(itemFirst.indexColumn-itemSecond.indexColumn) == 1 ) ? (true) : (false);
		var testY = ( Mathf.Abs(itemFirst.indexRow-itemSecond.indexRow) == 1 ) ? (true) : (false);
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
		  freezeControlPlay();
		  GoSwapNeighboor();
		}
	  }
	  else
	  {
		_stepMove = 0;
		OpenControlPlay();
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
			   items[indexCenterItem].MarkForRemove( true );
			   CutItem?.Invoke(itemsForDelete[i]);
			   CutItem?.Invoke(items[indexCenterItem]);
			}
		}
		return isRemove;
	}

	 	private bool CutIdenticalItems( PlayItem centerItem )
	{
		int centerRow = centerItem.indexRow;
		int centerColumn = centerItem.indexColumn;
		int lengthRow = _items.GetLength(1);
		int lengthColumn = _items.GetLength(0);
		PlayItem[] itemsHorizontal = Enumerable.Range(0,lengthColumn).Select(x => _items[x,centerRow]).ToArray();
		PlayItem[] itemsVertical = Enumerable.Range(0,lengthRow).Select(y => _items[centerColumn,y]).ToArray();
		return DeleteSeveralItems( itemsVertical, centerRow ) || DeleteSeveralItems( itemsHorizontal, centerColumn );
	}


	private bool FullFindToRemoveItems( PlayItem [,] items )
	{
	  bool resultFindRemove = false;
	   for( int i = 0; i < items.GetLength(0); i++ )
	   {
		 for( int j = 0 ; j < items.GetLength(1); j++ )
		 {
            if( items[i,j].isRemove == false )
			{
		       resultFindRemove = resultFindRemove | CutIdenticalItems( items[i,j] );
			}
		 }
	   }
	   return resultFindRemove;
	}


	void EffectRemove( PlayItem[] items )
	{
		for( int i = 0 ; i < items.Length ; i++ )
		{
			if( items[i].isRemove )
			{
		     CutItem?.Invoke(items[i]);
			}
		}
	}

	private void MoveNewPositionItem( PlayItem[,] gridItems, PlayItem item, int column, int row )
	{
		item.ReadyNewPosition(column,row);
		int row2 = item.indexRow;
		int column2 = item.indexColumn;
		item.SetIndexes(column,row);
		gridItems[column,row].SetIndexes(column2,row2);
		OnMoveItem?.Invoke(item, gridItems[column,row]);
		PlayItem buffItem;
		buffItem = gridItems[column,row];
		gridItems[column,row] = gridItems[column2,row2];
		gridItems[column2,row2] = buffItem;
	}


	private void FindMoveEmptyItems( PlayItem[,] items )
	{
      for( int column = 0 ; column < items.GetLength(0); column++ )
	  {
		int j = items.GetLength(1) - 1;
		int countEmptyItems = 0;
		int countFull = 0;
		_countEmptyItems[column] = 0;
		int count = 0;

		while( j >= 0 )
		{
			if(items[column,j].isRemove == true)
			{
				_countEmptyItems[column]++;
			}
		  j--;
		}

		j = items.GetLength(1) - 1;

		while( j >= 0 )
		{
			if( items[column,j].isRemove == true )
			{
			   countEmptyItems++;
			}
			else if( items[column,j].isRemove == false && countEmptyItems > 0 )
			{
				  MoveNewPositionItem( items, items[column,j], column, j+countEmptyItems);	
			}
			j--;
		}

		for(int k = 0 ; k < items.GetLength(1) ; k++ )
		{
			if(items[column,k].isRemove)
			{
			  items[column,k].transform.gameObject.SetActive(false);
			}
		}
	  }
	}

  private PlayItem[,] CreateNewItems(  )
	{

		PlayItem[,] itm;
       for(int i = 0 ; i < _items.GetLength(0); i++ )
      {
		PlayItem[] items = Enumerable.Range(0,_items.GetLength(1)).Select(x => _items[i,x]).ToArray();
			Vector3 startPosition = _gridItem.startPosition;
			Vector3 createPosition = startPosition;
			var scaleItem = new Vector2(_gridItem.sizeCellX, _gridItem.sizeCellY);
			var countEmptyItems =  _countEmptyItems[i];	
			var spawnColumn = i;

			for( int j = 0 ; j < countEmptyItems ; j++ )
			{
				var spawnRow = countEmptyItems - 1 -j;			
				createPosition.y += scaleItem.y;
		    	createPosition.x = items[j].transform.position.x;
				var itemObject = CreateItem(createPosition);
		    	spawnItems[spawnColumn,j] = itemObject.GetComponent<PlayItem>() as PlayItem;
				spawnItems[spawnColumn,j].SetState(i, spawnRow, _gridItem.sizeCellX, _gridItem.sizeCellY, _gridItem.positionCells);
				Vector3 spawnPositionItem = spawnItems[spawnColumn,j].transform.localPosition;
				spawnPositionItem.y -= scaleItem.y * countEmptyItems;
				spawnItems[spawnColumn,j].mouseDown += PressItem;
				spawnItems[spawnColumn,j].mouseMove += MoveElement;
		 		spawnItems[spawnColumn,j].mouseUp += ReturnWaitingState;
			}
	  }
		return spawnItems;
	}

	private void FallSpawnItems( PlayItem[,] gridItems, PlayItem[,] spawnItems)
	{
      for(int i = 0 ; i < gridItems.GetLength(0); i++ )
      {
		  for( int j = 0 ; j < _countEmptyItems[i] ; j++ )
	    {
			var spawnRow = _countEmptyItems[i] - 1 -j;
 	 	      gridItems[i,spawnRow].transform.gameObject.SetActive(false);
			  PlayItem itemEmpty = gridItems[i,spawnRow];
 		      gridItems[i,spawnRow].name = "empty";
		      gridItems[i,spawnRow] = spawnItems[i,j];
		      gridItems[i,spawnRow].SetIndexes(i,spawnRow);
		      gridItems[i,spawnRow].ReadyNewPosition(i,spawnRow);
		    OnMoveItem?.Invoke(gridItems[i,spawnRow], itemEmpty);
		}
	  }
	}

    public void FinishSwap()
    {
 	   ResetItem();  
 	   ReplaceItems(_firstItem,_secondItem);
	   StartCoroutine(ProcessingCycleItems( _firstItem ));
	  if( _isTestDelete ) 
	  {
		  _stepMove = (int)StatesMove.Stop;
	  }
	  _isUpdateItem = false;
	  _isTestDelete = false;
	  _isClick = false;
    }

    IEnumerator ProcessingCycleItems( PlayItem item )
	{

		_isTestDelete = CutIdenticalItems( item );

		if( _isTestDelete )
		{
		   bool isContinue = false;
		   float deltaTime = 0.5f;
		   float stepDecreaseTime = 0.3f;

		   do
		   {
			   yield return new WaitForSeconds(deltaTime);
			   FindMoveEmptyItems( _items );
		       CreateNewItems();
   			   FallSpawnItems( _items, spawnItems);
			   yield return new WaitForSeconds(deltaTime);
			   isContinue = FullFindToRemoveItems( _items );
			   deltaTime -= stepDecreaseTime;
			   deltaTime = Mathf.Clamp(deltaTime,0.2f,1);
		   }while(isContinue);
		   OpenControlPlay();
	    }
		else
		{
			TrySwapNeighboor();
		}
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

 

  	  private void Update()
	{


		  if(Input.GetKeyDown(KeyCode.Q))
		{
		}



	
	}
	


}


enum StatesMove
{
	Stop,
	stepOne,
	stepTwo
}