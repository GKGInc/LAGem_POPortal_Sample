import{_ as e}from"./_tslib-6e8ca86b.js";import{D as t,b as s}from"./dx-dropdown-base3-8e15c7c1.js";import{V as i,H as n,a as o,D as r,b as l}from"./dx-scroll-viewer-015d9282.js";import{S as d}from"./scroll-viewer-css-classes-4d6797bc.js";import{b as a,c as h,L as c,a as m,d as u,e as p}from"./dx-list-box-events-c8133a66.js";import{b}from"./browser-d96520d8.js";import{d as I}from"./dom-da46d023.js";import{o as C,q as w}from"./popup-e33cb9f6.js";import{k as v}from"./key-f9cbed1b.js";import{D as y}from"./layouthelper-0c7c89da.js";import{G as E}from"./grid-scroll-utils-2608dc88.js";import{T as g,B as D,a as S}from"./dx-virtual-scroll-viewer-a6e3c836.js";import{containsFocusHiddenAttribute as _,addFocusHiddenAttribute as V,removeFocusHiddenAttribute as f}from"./focus-utils-405a64a8.js";import{C as x}from"./custom-events-helper-e7f279d3.js";import{p as K}from"./constants-86572358.js";import{D as A,H as N,L as T}from"./dx-html-element-pointer-events-helper-b4d2ae7e.js";import{n as H}from"./property-4ec0b52d.js";class P extends a{}P.DropDownRowSelector=`:scope > tr:not(.${h.EmptyDataItemClassName}):not([${g}]):not([${D}])`,P.DropDownTableRowSelector=`tr[${c.VisibleIndexAttributeName}]`,P.DropDownItemSelector=`:scope > li:not(.${h.EmptyDataItemClassName}):not([${g}]):not([${D}])`,P.DropDownListItemSelector=`li[${c.VisibleIndexAttributeName}]`;class M{constructor(e){this._selectedItemIndex=-1,this._selectedVisibleIndex=-1,this._firstVisibleIndex=-1,this._lastVisibleIndex=-1,this._firstAvailableVisibleIndex=0,this._lastAvailableVisibleIndex=-1,this._tabKeyPressed=!1,this._isActive=!1,this._renderContainer=null,this._requireUpdateSelectedItemState=-1,this.boundOnRenderContainerMutationHandler=this.onRenderContainerMutation.bind(this),this.boundOnVisibleElementChangedHandler=this.onVisibleElementChanged.bind(this),this._component=e,this._items=[],this._renderContainerMutationObserver=new MutationObserver(this.boundOnRenderContainerMutationHandler)}get tabKeyPressed(){return this._tabKeyPressed}get items(){return this._items}get itemCount(){return this.items.length}get selectedItemElement(){return this.selectedItemIndex>=0&&this.selectedItemIndex<this.itemCount?this.items[this.selectedItemIndex]:null}get selectedItemIndex(){return this._selectedItemIndex}get isFirstItemSelected(){return this._selectedVisibleIndex<=this._firstAvailableVisibleIndex}get isLastItemSelected(){return this._selectedVisibleIndex>=this._lastAvailableVisibleIndex}get isVirtualRenderModeEnabled(){return this._component.enableVirtualRenderMode}get totalItemCount(){const e=this._component.getScrollViewer();return e&&e instanceof S?e.totalItemCount:this.itemCount}get canHighlightSearchResult(){return this.itemCount>0&&this._component.canHighlightSearchResult}isIndexWithinBoundaries(e){return e>=0&&e<this.itemCount}onPopupShown(){this._isActive=!0,this.initialize(),this.addEventSubscriptions()}onPopupClosed(){this.removeEventSubscriptions(),this.dispose(),this._isActive=!1}processKeyDown(e){switch(v.KeyUtils.getEventKeyCode(e)){case v.KeyCode.Tab:this._tabKeyPressed=!0;break;case v.KeyCode.Enter:case v.KeyCode.Esc:return!this._isActive&&(this._component.dispatchKeyDownEvent(e),!0);case v.KeyCode.Up:return this._component.handleCloseDropDown(e);case v.KeyCode.Down:return this._component.handleOpenDropDown(e)}return!1}processCapturedKeyDown(e){switch(v.KeyUtils.getEventKeyCode(e)){case v.KeyCode.Tab:this._tabKeyPressed=!0;break;case v.KeyCode.Enter:case v.KeyCode.Esc:return this._component.dispatchKeyDownEvent(e),!0;case v.KeyCode.Up:return!(e.altKey||e.metaKey)&&this.handleArrowUpKeyDown();case v.KeyCode.Down:return!(e.altKey||e.metaKey)&&this.handleArrowDownKeyDown();case v.KeyCode.PageUp:return this.handlePageUp();case v.KeyCode.PageDown:return this.handlePageDown();case v.KeyCode.Home:return e.ctrlKey&&e.shiftKey&&this.handleHomeKeyDown();case v.KeyCode.End:return e.ctrlKey&&e.shiftKey&&this.handleEndKeyDown()}return!1}processKeyUp(e){switch(this._tabKeyPressed=!1,v.KeyUtils.getEventKeyCode(e)){case v.KeyCode.Down:case v.KeyCode.Up:case v.KeyCode.PageUp:case v.KeyCode.PageDown:this.notifyFocusedRowChanged();break;case v.KeyCode.Home:case v.KeyCode.End:e.shiftKey&&e.ctrlKey&&this.notifyFocusedRowChanged()}return!1}updateState(){this._isActive?this.updateSelectedItemState():this._requireUpdateSelectedItemState++}getItem(e){return this.items[e]}addFocusHiddenAttribute(){this._renderContainer&&!_(this._renderContainer)&&V(this._renderContainer)}removeFocusHiddenAttribute(){this._renderContainer&&_(this._renderContainer)&&f(this._renderContainer)}containsFocusHiddenAttribute(){return!!this._renderContainer&&_(this._renderContainer)}dispose(){this.removeFocusHiddenAttribute(),this._renderContainerMutationObserver.disconnect(),this.setSelectedItem(-1,!0),this.notifyFocusedRowChanged(),this._firstAvailableVisibleIndex=-1,this._lastAvailableVisibleIndex=-1,this._firstVisibleIndex=-1,this._lastVisibleIndex=-1,this._items=[]}initialize(){this._items=this.queryItems(),this.updateBoundaries(),this._renderContainer=this._component.renderContainer,this._renderContainer&&this._renderContainerMutationObserver.observe(this._renderContainer,{childList:!0,subtree:!0}),this.addFocusHiddenAttribute(),this.updateViewportBoundaries();const e=this.findSelectedDataItemIndex();this.canHighlightSearchResult&&this._requireUpdateSelectedItemState>=0&&(this.updateSelectedItemState(),this._requireUpdateSelectedItemState=-1),this.setSelectedItem(e),this.notifyFocusedRowChanged()}handleArrowUpKeyDown(){return this.removeFocusHiddenAttribute(),this.isFirstItemSelected?(this.addFocusMarkerClass(this.selectedItemElement),!1):(this.isVirtualRenderModeEnabled?this.moveToPrevVirtualItem():this.moveToPrevItem(),!0)}handleArrowDownKeyDown(){return this.removeFocusHiddenAttribute(),this.isLastItemSelected?(this.addFocusMarkerClass(this.selectedItemElement),!1):(this.isVirtualRenderModeEnabled?this.moveToNextVirtualItem():this.moveToNextItem(),!0)}handlePageUp(){return this.removeFocusHiddenAttribute(),this.isFirstItemSelected?(this.addFocusMarkerClass(this.selectedItemElement),!1):(this.isVirtualRenderModeEnabled?this.moveToPrevVirtualPageItem():this.moveToPrevPageItem(),!0)}handlePageDown(){return this.removeFocusHiddenAttribute(),this.isLastItemSelected?(this.addFocusMarkerClass(this.selectedItemElement),!1):(this.isVirtualRenderModeEnabled?this.moveToNextVirtualPageItem():this.moveToNextPageItem(),!0)}moveToNextPageItem(){const e=this.itemCount-1,t=this._component.getScrollViewer();if(t){const s=E.calculateBoundaryItemIndex(this,t,!0);this.setSelectedItem(s>e?e:s),this.scrollToElement(i.Bottom,n.None)}}moveToPrevPageItem(){const e=this._component.getScrollViewer();if(e){const t=E.calculateBoundaryItemIndex(this,e,!1);this.setSelectedItem(t<0?0:t),this.scrollToElement(i.Top,n.None)}}moveToPrevVirtualPageItem(){const e=this._component.getScrollViewer();if(e){const t=E.calculateBoundaryItemIndex(this,e,!1),s=this._selectedVisibleIndex+(t-this.selectedItemIndex);this.isElementInsideViewport(s)?(this.setSelectedItem(t),this.scrollToElement(i.Top,n.None)):this.notifyMakeElementVisible(s,i.Top)}}moveToNextVirtualPageItem(){const e=this._component.getScrollViewer();if(e){const t=E.calculateBoundaryItemIndex(this,e,!0),s=this._selectedVisibleIndex+(t-this.selectedItemIndex);this.isElementInsideViewport(s)?(this.setSelectedItem(t),this.scrollToElement(i.Bottom,n.None)):this.notifyMakeElementVisible(s,i.Bottom)}}handleHomeKeyDown(){return this.removeFocusHiddenAttribute(),this.isFirstItemSelected?(this.addFocusMarkerClass(this.selectedItemElement),!1):(this.isVirtualRenderModeEnabled?this.moveToFirstVirtualItem():this.moveToFirstItem(),!0)}handleEndKeyDown(){return this.removeFocusHiddenAttribute(),this.isLastItemSelected?(this.addFocusMarkerClass(this.selectedItemElement),!1):(this.isVirtualRenderModeEnabled?this.moveToLastVirtualItem():this.moveToLastItem(),!0)}moveToPrevItem(){this.selectedItemIndex>0&&this.setSelectedItem(this.selectedItemIndex-1),this.scrollToElement(null,n.None)}moveToNextItem(){this.selectedItemIndex<this.itemCount-1&&this.setSelectedItem(this.selectedItemIndex+1),this.scrollToElement(null,n.None)}moveToNextVirtualItem(){const e=this._selectedVisibleIndex+1;this.isElementInsideViewport(e)&&this.selectedItemIndex<this.totalItemCount-1?(this.setSelectedItem(this.selectedItemIndex+1),this.scrollToElement(null,n.None)):this.notifyMakeElementVisible(e,i.Bottom)}moveToPrevVirtualItem(){const e=this._selectedVisibleIndex-1;this.isElementInsideViewport(e)&&this.selectedItemIndex>0?(this.setSelectedItem(this.selectedItemIndex-1),this.scrollToElement(null,n.None)):this.notifyMakeElementVisible(e,i.Top)}moveToFirstItem(){this._component.getScrollViewer()&&(this.setSelectedItem(0),this.scrollToElement(i.Top,n.None))}moveToFirstVirtualItem(){this.isElementInsideViewport(0)&&this.selectedItemIndex>0?(this.setSelectedItem(0),this.scrollToElement(i.Top,n.None)):this.notifyMakeElementVisible(0,i.Top)}moveToLastItem(){this._component.getScrollViewer()&&(this.setSelectedItem(this.itemCount-1),this.scrollToElement(i.Bottom,n.None))}moveToLastVirtualItem(){const e=this.totalItemCount-1;this.isElementInsideViewport(e)&&this.selectedItemIndex<this.itemCount-1?(this.setSelectedItem(this.itemCount-1),this.scrollToElement(i.Bottom,n.None)):this.notifyMakeElementVisible(e,i.Bottom)}updateViewportBoundaries(){if(0===this.itemCount)return;const e=this.items[0];this._firstVisibleIndex=e?this.getElementVisibleIndex(e):-1;const t=this.items[this.itemCount-1];this._lastVisibleIndex=t?this.getElementVisibleIndex(t):-1}updateBoundaries(){let e=0;0!==this._firstAvailableVisibleIndex&&(-1!==this._firstAvailableVisibleIndex&&e++,this._firstAvailableVisibleIndex=0);const t=this.totalItemCount-1;return this._lastAvailableVisibleIndex!==t&&(-1!==this._lastAvailableVisibleIndex&&e++,this._lastAvailableVisibleIndex=t),e>0}isElementInsideViewport(e){return e>=this._firstVisibleIndex&&e<=this._lastVisibleIndex}findItemIndex(e){for(let t=0;t<this.itemCount;t++){const s=this.items[t];if(s&&this.getElementVisibleIndex(s)===e)return t}return-1}getElementVisibleIndex(e){return y.getAttributeIntValue(e,c.VisibleIndexAttributeName,-1)}findSelectedDataItemIndex(){for(let e=0;e<this.itemCount;e++){const t=this.items[e];if(t&&t.matches(a.SelectedItemSelector))return e}return-1}notifyFocusedRowChanged(){this._component.notifyFocusedRowChanged(this._selectedVisibleIndex)}notifyMakeElementVisible(e,t=null){e<0||this._component.notifyMakeElementVisible(e,m.getVerticalEdge(t))}setSelectedItem(e,t=!1){(this.selectedItemIndex!==e||t)&&(this.removeFocusMarkerClass(this.selectedItemElement),this._selectedItemIndex=e,this.selectedItemElement?(this.addFocusMarkerClass(this.selectedItemElement),this.updateActiveDescendantAttribute(this.selectedItemElement),this._selectedVisibleIndex=this.getElementVisibleIndex(this.selectedItemElement)):(this.removeFocusMarkerClass(this.selectedItemElement),this.updateActiveDescendantAttribute(this.selectedItemElement),this._selectedVisibleIndex=-1))}updateActiveDescendantAttribute(e){const t=e&&e.getAttribute(c.ItemIdAttributeName);this._component.updateActiveDescendantAttributeValue(null!=t?t:"")}removeFocusMarkerClass(e){e&&I.DomUtils.hasClassName(e,h.FocusedItemClassName)&&I.DomUtils.removeClassName(e,h.FocusedItemClassName)}addFocusMarkerClass(e){this.containsFocusHiddenAttribute()||e&&!I.DomUtils.hasClassName(e,h.FocusedItemClassName)&&I.DomUtils.addClassName(e,h.FocusedItemClassName)}queryItems(){const e=this._component.getTable();if(e){const t=e.querySelector(P.TableBodySelector);if(t)return m.queryItemsBySelector(t,P.DropDownRowSelector)}else{const e=this._component.getList();if(e)return m.queryItemsBySelector(e,P.DropDownItemSelector)}return[]}addEventSubscriptions(){const e=this._component.getScrollViewer();e&&e.addEventListener(o.eventName,this.boundOnVisibleElementChangedHandler)}removeEventSubscriptions(){const e=this._component.getScrollViewer();e&&e.removeEventListener(o.eventName,this.boundOnVisibleElementChangedHandler)}updateSelectedItemState(){this.canHighlightSearchResult?(this.removeFocusHiddenAttribute(),this.setSelectedItem(0,!0)):(this.addFocusHiddenAttribute(),this.setSelectedItem(-1,!0)),this.notifyFocusedRowChanged()}onRenderContainerMutation(e,t){if(this._isActive)if(this.removeFocusMarkerClass(this.selectedItemElement),this._items=this.queryItems(),this.updateViewportBoundaries(),this.updateBoundaries())this.updateSelectedItemState();else if(this.isElementInsideViewport(this._selectedVisibleIndex)){const e=this.findItemIndex(this._selectedVisibleIndex);e>-1&&(this.setSelectedItem(e,!0),this.notifyFocusedRowChanged())}}onVisibleElementChanged(e){if(e.detail.isFocusRequired){const t=this.items.indexOf(e.detail.element);this.setSelectedItem(t),this.notifyFocusedRowChanged()}}scrollToElement(e,t){const s=this._component.getScrollViewer();s&&this.selectedItemElement&&r.scrollToElementRelyOnStickyDescendants(this.selectedItemElement,e,t,s)}}class R extends CustomEvent{constructor(e,t,s,i){super(R.eventName,{detail:new F(e,t,s,i),bubbles:!0,composed:!0,cancelable:!0})}}R.eventName=K+".keydown";class F{constructor(e,t,s,i){this.Key=e,this.AltKey=t,this.CtrlKey=s,this.ShiftKey=i}}class O extends CustomEvent{constructor(){super(O.eventName,{bubbles:!0,composed:!0,cancelable:!0})}}O.eventName=K+".dropdownclosed",x.register(R.eventName,(e=>e.detail)),x.register(O.eventName,(e=>e.detail));class k{}var U;k.AriaActiveDescendant="aria-activedescendant",function(e){e.Default="Default",e.None="None",e.AutoSearch="AutoSearch"}(U||(U={}));class B extends t{constructor(){super(),this._columnsInfo=[],this._focusedRowIndex=-1,this._elementHasCalculatedWidthAttribute="has-calculated-width",this.boundOnScrollViewerUpdateHandler=this.onScrollViewerUpdate.bind(this),this.boundOnMouseDownHandler=this.onRenderContainerMouseDown.bind(this),this.boundOnVisibleElementChangedHandler=this.onVisibleElementChanged.bind(this),this.boundOnPopupShownHandler=this.onPopupShown.bind(this),this.boundOnPopupClosedHandler=this.onPopupClosed.bind(this),this._tableHeaderCellSelector=`.${d.ContentContainerClassName} > table > thead > ${a.HeaderRowSelector} > th`,this._tableBodyCellSelector=`.${d.ContentContainerClassName} > table > tbody > ${a.TableRowSelector} > td`,this._hoverTitleElementsSelector=[this._tableBodyCellSelector,this._tableHeaderCellSelector].join(", "),this._kbdNavigationHelper=new M(this),this.dropDownWidthMode=s.ContentOrEditorWidth,this.searchMode=U.Default,this.useCustomInput=!1,this._pointerEventsHelper=new A(this)}connectedCallback(){super.connectedCallback(),this.prepareRenderContainer()}disconnectedCallback(){super.disconnectedCallback(),this.releaseRenderContainer()}ensurePopupElement(){super.ensurePopupElement(),this.popupElement&&(this.popupElement.addEventListener(C.eventName,this.boundOnPopupShownHandler),this.popupElement.addEventListener(w.eventName,this.boundOnPopupClosedHandler))}disposePopupElement(){this.popupElement&&(this.popupElement.removeEventListener(C.eventName,this.boundOnPopupShownHandler),this.popupElement.removeEventListener(w.eventName,this.boundOnPopupClosedHandler)),super.disposePopupElement()}get renderContainer(){return this.popupDOMElement?this.popupDOMElement.querySelector(`.${h.RenderContainer}`):null}getScrollViewer(){return this.renderContainer?this.renderContainer.querySelector(`.${d.ClassName}`):null}getScrollViewerContent(){return this.renderContainer?this.renderContainer.querySelector(`.${d.ContentContainerClassName}`):null}getTable(){return this.renderContainer?this.renderContainer.querySelector(`.${d.ContentContainerClassName} > table`):null}getList(){return this.renderContainer?this.renderContainer.querySelector(a.ListSelector):null}notifyFocusedRowChanged(e){e!==this._focusedRowIndex&&(this.dispatchEvent(new u(e)),this._focusedRowIndex=e)}notifyMakeElementVisible(e,t){e<0||this.dispatchEvent(new p(e,t))}notifyColumnsChanged(e){this._columnsInfo=e}handleOpenDropDown(e){return!(!this.enabled||this.isReadOnly||this.isDropDownOpen||!e.altKey&&!e.metaKey)&&(this.openDropDown(),!0)}handleCloseDropDown(e){return!(!this.enabled||this.isReadOnly||!this.isDropDownOpen||!e.altKey&&!e.metaKey)&&(this.closeDropDown(),!0)}dispatchKeyDownEvent(e){const t=new R(e.key,e.altKey,e.ctrlKey||e.metaKey,e.shiftKey);this.dispatchEvent(t)}updateActiveDescendantAttributeValue(e){this.inputElement&&this.inputElement.hasAttribute(k.AriaActiveDescendant)&&this.inputElement.setAttribute(k.AriaActiveDescendant,e)}get isTableRender(){return!!this.getTable()}get enableVirtualRenderMode(){const e=this.getScrollViewer();return null!==e&&e instanceof S}get isSearchActive(){switch(this.searchMode){case U.None:case U.Default:return!1;default:return!0}}get canHighlightSearchResult(){const e=this.fieldElement?this.fieldElementValue:this.fieldText;return this.isSearchActive&&(e?e.length:-1)>0&&!this.useCustomInput}onTextInput(e){this.inputElement&&(this.ensureDropDownOpened(e),this._kbdNavigationHelper.updateState(),this.raiseFieldText())}raiseFocusOut(e){super.raiseFocusOut(e,this._kbdNavigationHelper.tabKeyPressed)}ensureDropDownOpened(e){!this.isDropDownOpen&&e.data&&e.data.length>0&&this.tryOpenDropDown()}processKeyDown(e){return!!this.tryProcessKeyDown(e)||super.processKeyDown(e)}processKeyUp(e){return!!this.tryProcessKeyUp(e)||super.processKeyUp(e)}tryProcessKeyDown(e){return this._kbdNavigationHelper.processKeyDown(e)}tryProcessKeyUp(e){return this._kbdNavigationHelper.processKeyUp(e)}processCapturedKeyDownAsync(e,t){return this._kbdNavigationHelper.processCapturedKeyDown(e)?(e.preventDefault(),t.handled=!0,Promise.resolve()):super.processCapturedKeyDownAsync(e,t)}onPopupShown(e){setTimeout((()=>{this.prepareRenderContainer(),this._kbdNavigationHelper.onPopupShown()}))}onPopupClosed(e){setTimeout((()=>{this._kbdNavigationHelper.onPopupClosed(),this.releaseRenderContainer()}))}prepareRenderContainer(){this.renderContainer&&(this.renderContainer.addEventListener(l.eventName,this.boundOnScrollViewerUpdateHandler),b.Browser.Firefox&&this.isTableRender&&this.renderContainer.addEventListener("mousedown",this.boundOnMouseDownHandler),this.renderContainer.addEventListener(o.eventName,this.boundOnVisibleElementChangedHandler),this.resizeDropDownElement(),this._pointerEventsHelper.initialize())}onVisibleElementChanged(e){this.resizeDropDownElement()}resizeDropDownElementCore(){const e=this.getTable();if(e){const t=Array.from(e.querySelectorAll(":scope > colgroup > col")),s=[],i=[];if(t.forEach((e=>{(e.style.width.indexOf("%")>0||e.hasAttribute(this._elementHasCalculatedWidthAttribute))&&(e.style.removeProperty("width"),e.removeAttribute(this._elementHasCalculatedWidthAttribute)),e.style.width?s.push(e):i.push(e)})),i.length<1)return;const n=s.reduce(((e,t)=>e+t.getBoundingClientRect().width),0),o=e.style.width;e.style.width="auto";const r=i.reduce(((e,t)=>e+t.getBoundingClientRect().width),0),l=this.dropDownElement.minDesiredWidth?I.DomUtils.pxToInt(this.dropDownElement.minDesiredWidth):0;if(this.needAdjustNarrowContent(n,r,l)){if(i.length>1){const e=(l-n)/r;for(let t=0;t<i.length-1;t++){const s=i[t];s.style.width=y.toPx(Math.ceil(s.getBoundingClientRect().width*e)),s.setAttribute(this._elementHasCalculatedWidthAttribute,"")}}}else i.forEach((e=>{e.style.width=y.toPx(Math.ceil(e.getBoundingClientRect().width)),e.setAttribute(this._elementHasCalculatedWidthAttribute,"")}));e.style.width=o}else{const e=this.getList(),t=window.innerWidth,s=this.getScrollViewerContent(),i=this.popupDOMElement&&s?this.popupDOMElement.getBoundingClientRect().width-s.getBoundingClientRect().width:0,n=Math.ceil(e.getBoundingClientRect().width+i);this.dropDownElement.desiredWidth=y.toPx(Math.min(n,t))}}needAdjustNarrowContent(e,t,i){if(i){return this.dropDownWidthMode===s.ContentOrEditorWidth&&e+t<i}return!1}resizeDropDownElement(){this.dropDownElement&&this.isDropDownWidthDependsOnContent&&this.resizeDropDownElementCore()}releaseRenderContainer(){this.renderContainer&&(this.renderContainer.removeEventListener(l.eventName,this.boundOnScrollViewerUpdateHandler),this.renderContainer.removeEventListener(o.eventName,this.boundOnVisibleElementChangedHandler),b.Browser.Firefox&&this.isTableRender&&this.renderContainer.removeEventListener("mousedown",this.boundOnMouseDownHandler),this._pointerEventsHelper.dispose()),this.isDropDownWidthDependsOnContent&&this.dropDownElement&&(this.dropDownElement.desiredWidth=null)}onSizeChanged(e,t){if(!this.dropDownElement)return;const s=this.getScrollViewer();s&&s.refreshUI&&s.refreshUI(),super.onSizeChanged(e,t)}onRenderContainerMouseDown(e){e.ctrlKey&&e.preventDefault()}onScrollViewerUpdate(e){const t=this.getScrollViewerContent(),s=this.getTable();t&&s&&(s.offsetHeight<t.clientHeight?I.DomUtils.addClassName(s,h.TableNoScrollClassName):I.DomUtils.removeClassName(s,h.TableNoScrollClassName)),e.stopPropagation()}get isDropDownWidthDependsOnContent(){return this.dropDownWidthMode===s.ContentWidth||this.dropDownWidthMode===s.ContentOrEditorWidth}get handlePointerEventsMode(){return N.None}get handlePointerEventsTarget(){var e;return null!==(e=this.getTable())&&void 0!==e?e:this}get handlePointerEventsDelay(){return T}get hoverTitleElementsSelector(){return this._hoverTitleElementsSelector}get bypassNonInlineHoverTitleElementChildSelector(){return null}}e([H({type:U,attribute:"search-mode"})],B.prototype,"searchMode",void 0),e([H({type:Boolean,attribute:"use-custom-input"})],B.prototype,"useCustomInput",void 0);export{B as D,U as L};