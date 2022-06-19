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
	private bool _isBlock = false;
	private bool _isAnimation = false;
	public PlayItem _destroyItem;
	public bool blockInput => _isBlock;

	public event UnityAction<PlayItem,PlayItem> OnSwapItems;
	public event UnityAction<PlayItem> OnMoveItem;
	public event UnityAction<PlayItem> CutItem;

	
	  private void OnEnable()
	{
	   _countCells = _gridItem.countCells;
	   _items = _gridItem._playItems;
	   _countEmptyItems = new int[_items.GetLength(0)];

	   	spawnItems = new PlayItem[(int)_countCells,(int)_countCells];

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

	public void EnableAnimation()
	{
		_isAnimation = true;
	}
		public void DisableAnimation()
	{
		_isAnimation = false;
	}


	private void PressItem(PlayItem item)
	{ 
		if( blockInput ) return;
		if( _isAnimation == true ) return;

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
		Debug.Log($"Индексы элемента {item.indexColumn},{item.indexRow}  Его имя {item.name} Его координаты {item.transform.position.x}, {item.transform.position.y}  IsRemove={item.isRemove}");

		if( _firstItem.change == 1 && _firstItem != item  && isAnimation == false ) 
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
		int lengthRow = _items.GetLength(0);
		PlayItem[] itemsHorizontal = Enumerable.Range(0,lengthRow).Select(x => _items[x,centerRow]).ToArray();
		PlayItem[] itemsVertical = Enumerable.Range(0,lengthRow).Select(y => _items[centerColumn,y]).ToArray();
		return DeleteSeveralItems( itemsVertical, centerRow ) || DeleteSeveralItems( itemsHorizontal, centerColumn );
	}

	private bool FullCutIdenticalItems()
	{
		bool isFindItemCut = false;
		int count = 0;
		bool TestDelete = false;
		  for( int i = 0 ; i < _items.GetLength(0); i++ )
		  {
		    for( int j = 0 ; j < _items.GetLength(1); j++ )
		    {
		  	    TestDelete = CutIdenticalItems( _items[i,j] );
				isFindItemCut |= TestDelete;
		    }
		  }
		return isFindItemCut;
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

    IEnumerator ProcessingCycleItems( PlayItem item )
	{

		_isTestDelete = CutIdenticalItems( item );

				
		if( _isTestDelete )
		{
		   bool isContinue = false;

		   do
		   {
			   //isContinue = FullCutIdenticalItems();
			   yield return new WaitForSeconds(0.5f);
		 	   MoveDownItems();
		       CreateNewItems();
   			   FallSpawnItems( _items, spawnItems);
			  yield return new WaitForSeconds(0.5f);
			  
			 isContinue = FullCutIdenticalItems();
			 yield return new WaitForSeconds(0.5f);
		   }while(isContinue);

		   isAnimation = false;
	    }
		else
		{
			TrySwapNeighboor();
		}
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

	private void FallSpawnItems( PlayItem[,] gridItems, PlayItem[,] spawnItems)
	{
      for(int i = 0 ; i < gridItems.GetLength(0); i++ )
      {
		  for( int j = 0 ; j < _countEmptyItems[i] ; j++ )
	    {
			var spawnRow = _countEmptyItems[i] - 1 -j;			
 	 	    gridItems[i,spawnRow].transform.gameObject.SetActive(false);
 		    gridItems[i,spawnRow].name = "empty";
		    gridItems[i,spawnRow] = spawnItems[i,j];
		    gridItems[i,spawnRow].SetIndexes(i,spawnRow);
		    gridItems[i,spawnRow].ReadyNewPosition(i,spawnRow);
		    OnMoveItem?.Invoke(gridItems[i,spawnRow]);
		}
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
			}
	  }

		return spawnItems;

	}


	private void FindEmptyItems( PlayItem[] items, int column )
	{
		int j = items.Length - 1;
		int countEmptyItems = 0;
		int countFull = 0;
		_countEmptyItems[column] = 0;

		while( j >= 0 )
		{
			if( items[j].isRemove == true )
			{
			 countEmptyItems++;
			_countEmptyItems[column]++;

			}
			else if( items[j].isRemove == false )
			{
				int offsetdown = countEmptyItems;
				MoveNewPositionItem( _items, items[j], column, j+offsetdown+countFull);
				countEmptyItems--;
				offsetdown++;
				countFull++;
				if(countEmptyItems == 0 && j > 0) j = items.Length - 1;
			}
			j--;
		}

		for(int k = 0 ; k < items.Length ; k++ )
		{
			if(items[k].isRemove)
			{
			  items[k].transform.gameObject.SetActive(false);
			}
		}
	}


    private void MoveDownItems( )
	{

      for(int i = 0 ; i < _items.GetLength(0); i++ )
     {
		PlayItem[] items = Enumerable.Range(0,_items.GetLength(1)).Select(x => _items[i,x]).ToArray();

			var countOffset = 0;
			int indexFirstEmpty = 0;
			int indexLastEmpty = 0;
			int indexFirstFull = 0;
			int indexLastFull = 0;
			int countFull = 0;
			var j = 0;
			int count = 0;
			
			   while( j < items.Length) 
			{

				count++;
				if( count > 500) { Debug.Log($"зацикливание цикла "); break; }


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
					   if( j-1 == 0 ) 
					   {
						 _countEmptyItems[i] = 1;
					   }
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
					if(  (j < items.Length-1 && items[j+1].isRemove == false) || j == items.Length-1  )  
					{
					   indexLastEmpty = j;
					   if( indexLastFull > 0 )
					   {
						 _countEmptyItems[i] = indexLastEmpty - indexLastFull;
					   }
					   else if( indexLastFull == 0 )
					   {
						 _countEmptyItems[i] = indexLastEmpty + 1;
					   }
					   countFull = 1 + indexLastFull - indexFirstFull;
					   var saveIndexLastFull = indexLastFull;
					   var saveIndexLastEmpty = indexLastEmpty;

					    if( _countEmptyItems[i] > 0 )
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
								Debug.Log($"indexfirstFul {indexFirstFull} indexLastFul {indexLastFull} countEmpty{_countEmptyItems[i]}");
								
						}
					}
				}
				j++;


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
 	   ReplaceItems(_firstItem,_secondItem);
	   StartCoroutine(ProcessingCycleItems( _firstItem ));
	   //ProcessingCycleItems( _firstItem );
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



   private GameObject CreateItem2( int index, Vector3 positionItem )
	{
		 GameObject randomItemObject = parentItems.GetChild(index).gameObject; 
		 var nameObject = randomItemObject.name;
		 var itemObject = Instantiate(  randomItemObject, positionItem, Quaternion.identity);
		 itemObject.transform.position = positionItem;
		 itemObject.transform.SetParent(this.transform);
		 itemObject.transform.localScale = _gridItem.scaleSprite;
		 itemObject.name = nameObject;
	     return itemObject;
	}




	private void NewItem( int i, int j )
	{
	   _items[i,j] = CreateItem2( 0, _gridItem.positionCells[i,j] ).GetComponent<PlayItem>() as PlayItem;
	   _items[i,j].transform.localPosition = _gridItem.positionCells[i,j];
	   _items[i,j].SetState(i, j, _gridItem.sizeCellX, _gridItem.sizeCellY, _gridItem.positionCells);
	   _items[i,j].mouseDown += PressItem;
	   _items[i,j].mouseMove += MoveElement;
	}




	int[] indexes1 = {2,5};
	int[] indexes2 = {6,9};


  	  private void Update()
	{
	    if(Input.GetKeyDown(KeyCode.Space))
		{
			CutIdenticalItems( _items[0,indexes1[0]] );

			CutIdenticalItems( _items[0,indexes2[0]] );

			int k = 0;
		    PlayItem[] items = Enumerable.Range(0,_items.GetLength(1)).Select(x => _items[k,x]).ToArray();
			FindEmptyItems( items, k );

			CreateNewItems();
			FallSpawnItems(_items,spawnItems);

		}


		if(Input.GetKeyDown(KeyCode.D))
		{
			for( int i = indexes1[0] ; i < indexes1[1] ; i++ )
			{ if(_items[0,i])
			  Destroy(_items[0,i].transform.gameObject);
			}

			for( int i = indexes2[0] ; i < indexes2[1] ; i++ )
			{ if(_items[0,i])
			  Destroy(_items[0,i].transform.gameObject);
			}



		}


		if(Input.GetKeyDown(KeyCode.Z))
		{

			Vector3 startPosition = _gridItem.startPosition;
			Vector3 createPosition = startPosition;
			var scaleItem = new Vector2(_gridItem.sizeCellX, _gridItem.sizeCellY);
			var Position = createPosition;
			for( int i = indexes1[0] ; i < indexes1[1] ; i++ ) 
			{
			 NewItem( 0, i);
			}
			for( int i = indexes2[0] ; i < indexes2[1] ; i++ ) 
			{
			 NewItem( 0, i);
			}

		}

		if(Input.GetKeyDown(KeyCode.S))
		{
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