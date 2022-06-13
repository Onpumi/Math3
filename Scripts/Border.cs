using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour
{
   private List<GameObject> _frame;
   private GameObject   _borderHorizontal;
   private GameObject   _borderVertical;	
   private float _width;
   private float _height;
   private float _sizeBlock;
   private Color _colorBlock;
   private Vector3 _positionBorder;
   private Vector2 _Vector2( float x, float y) => new Vector2(x,y);
     public  Border( GameObject borderHorizontal, GameObject borderVertical, Vector3 positionBorder, int sizeBorder, float scale, Color color ) 
   {
	   _width = _height = sizeBorder;
	   _sizeBlock = scale;
	   _colorBlock = color;
	   _borderHorizontal = borderHorizontal;
	   _borderVertical = borderVertical;
	   _positionBorder = positionBorder;
	  _frame = new List<GameObject>();
   }
    private void InitItem( GameObject item, Vector3 positionBorder, float scalex, float scaley )
   {
	   item.transform.position = positionBorder;
	   item.transform.localScale = _Vector2(scalex,scaley);
	   item.GetComponent<SpriteRenderer>().color = _colorBlock;
	  _frame.Add( Instantiate(item, new Vector3(positionBorder.x,positionBorder.y,0), Quaternion.identity) );
	 }
     private void CreateItemUp( )
   {
	    Vector3 positionBorder = _positionBorder;
	    positionBorder.y += _height/2;
   	  InitItem( _borderHorizontal, positionBorder, _width, _sizeBlock );
   }
     private void CreateItemDown( )
   {
	    Vector3 positionBorder = _positionBorder;
	    positionBorder.y -= _height/2;
	    InitItem( _borderHorizontal, positionBorder, _width, _sizeBlock );
   }
     private void CreateItemLeft( )
   {
	    Vector3 positionBorder = _positionBorder;
	    positionBorder.x -= _width/2 -_sizeBlock/2;
	    InitItem( _borderVertical, positionBorder, _sizeBlock, _height );
   }
     private void CreateItemRight()
   {
	   Vector3 positionBorder = _positionBorder;
	   positionBorder.x += _width/2 -_sizeBlock/2;
	   InitItem( _borderVertical, positionBorder, _sizeBlock, _height );
   }
     public void Create( )
   {
	   CreateItemUp();
	   CreateItemDown();
	   CreateItemLeft();
	   CreateItemRight();
   }
     public void DestroyThis()
   {
	   for( int i = 0 ; i < _frame.Count; i++ ) Destroy(_frame[i]);
   }
 }
