// Attach this script to the ScrollRect
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
 
namespace ReupVirtualTwin.behaviours
{
    [RequireComponent (typeof (ScrollRect))]
    public class AutoHideUIScrollbar : MonoBehaviour {
    
        public bool alsoDisableScrolling;
    
        private float disableRange = 0.99f;
        private ScrollRect scrollRect;
        private ScrollbarClass scrollbarVertical = null;
        private ScrollbarClass scrollbarHorizontal = null;
    
        private class ScrollbarClass
        {
            public Scrollbar bar;
            public bool active;
        }
    
    
        void Start ()
        {
            scrollRect = gameObject.GetComponent < ScrollRect >();
            if ( scrollRect.verticalScrollbar != null )
            {
                scrollbarVertical = new ScrollbarClass() { bar = scrollRect.verticalScrollbar, active = true };
            }
            if ( scrollRect.horizontalScrollbar != null )
            {
                scrollbarHorizontal = new ScrollbarClass() { bar = scrollRect.horizontalScrollbar, active = true };
            }
    
            if ( scrollbarVertical == null && scrollbarHorizontal == null )
                Debug.LogWarning ("Must have a horizontal or vertical scrollbar attached to the Scroll Rect for AutoHideUIScrollbar to work");
        }
    
        void Update ()
        {
            if ( scrollbarVertical != null )
                SetScrollBar( scrollbarVertical, true );
            if ( scrollbarHorizontal != null )
                SetScrollBar( scrollbarHorizontal, false );
        }
    
        void SetScrollBar ( ScrollbarClass scrollbar, bool vertical )
        {
            if ( scrollbar.active && scrollbar.bar.size > disableRange )
            {
                SetBar( scrollbar, false, vertical );
            }
            else if ( !scrollbar.active && scrollbar.bar.size < disableRange )
            {
                SetBar( scrollbar, true, vertical );
            }
        }
    
        void SetBar ( ScrollbarClass scrollbar, bool active, bool vertical )
        {
            if ( alsoDisableScrolling )
            {
                if ( vertical )
                    scrollRect.vertical = active;
                else
                    scrollRect.horizontal = active;
            }
            scrollbar.bar.gameObject.SetActive( active );
            scrollbar.active = active;
        }
    }
}