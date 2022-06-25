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
	[SerializeField] private Color _colorBorder;
	private float _sizeBorder;
	private Vector3 _positionField = new Vector3(0,0,0);
	private Vector3 positionField => _positionField;

      private void Awake()
    {
	  var camera = Camera.main;
	  var sizeScreen = camera.orthographicSize;
	  Screen.orientation = ScreenOrientation.Portrait;
	  _sizeBorder = sizeScreen;
  	//  var _Border = new Border( _borderHorizontal, _borderVertical, _positionField, (int)sizeScreen , 0.1f, _colorBorder);
    //  _Border?.Create();
     _items.Init( _sizeBorder, positionField, _countItems );
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
