using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class MainGame : MonoBehaviour
{
	[SerializeField] private GameObject _borderHorizontal;
	[SerializeField] private GameObject _borderVertical;
	[SerializeField] private Items _items;
	[SerializeField] private int _countItems;
	private float _sizeBorder;
	private Vector3 _positionField = new Vector3(0,0,0);
	private Vector3 positionField => _positionField;

	  [SerializeField] public int[] Arr; 
		 int[] ArrBefore;	

	 private void SortArr( int[] arr )
	 {
		 int empty = 0;
		 int i = arr.Length-1;
		while( i > 0 )
		{   
			int count = 0;
			int buff;
			if( arr[i] == empty && arr[i-1] != empty ) 
			{
			   if( i > 0 )
			   {
				buff = arr[i-1];
				arr[i-1] = arr[i];
				arr[i] = buff;
			   }
			   count++;

			}
			if(count > 0 ) {  i = arr.Length-1; continue; }
			i--;
		}
	 }

	 
      private void Awake()
    {

	  var camera = Camera.main;
	  var sizeScreen = camera.orthographicSize;
	  Screen.orientation = ScreenOrientation.Portrait;
	  _sizeBorder = sizeScreen;
  	  var _Border = new Border( _borderHorizontal, _borderVertical, _positionField, (int)sizeScreen , 0.1f, Color.red);
      _Border?.Create();
     _items.Init( _sizeBorder, positionField, _countItems );
	  
	  ArrBefore = new int[Arr.Length];

		 for(int i = 0 ; i < ArrBefore.Length; i++)
		  ArrBefore[i] = Arr[i];

		SortArr(Arr);

    }

	 private void OnGUI()
	 {


		// GUILayout.SelectionGrid(-1, ArrBefore.Select(i => i.ToString()).ToArray(),13);
		 
	    // GUILayout.SelectionGrid(-2, Arr.Select(i => i.ToString()).ToArray(),13);
	 }

     private void Update()
     {
  	   if( Input.GetAxis("Cancel") > 0 )
  	  {
	    Application.Quit();
	  }
    }
}
