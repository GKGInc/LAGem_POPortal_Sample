using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using DevExpress.Blazor;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Newtonsoft.Json;
using System.Drawing;
using LAGem_POPortal.Authentication;
using LAGem_POPortal.Data;
using LAGem_POPortal.Models;
using Microsoft.JSInterop;
using DevExpress.CodeParser;
using static DevExpress.Drawing.Printing.Internal.DXPageSizeInfo;
using DevExpress.DashboardCommon;
using DevExpress.XtraRichEdit.Services;
using Microsoft.AspNetCore.Components.Web;
using DevExpress.Blazor.Popup.Internal;
using DevExpress.Utils.About;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DevExpress.XtraCharts;
using DevExpress.XtraReports.Design.ParameterEditor;
using Azure;
using System.Security.Cryptography.Xml;
using System.Linq;
using DevExpress.DataAccess.Sql;
using System;

namespace LAGem_POPortal.Pages.InTransitPOs
{
    public class InTransitPOsBase : ComponentBase
    {
        // ============================================================ \\

        #region Variables

        // ------------------------------------------------------------ \\

        [Inject]
        public IJSRuntime js { get; set; }

        [Inject]
        public AuthenticationStateProvider authStateProvider { get; set; }

        [Inject]
        public NavigationManager navManager { get; set; }

        [Inject]
        public UserAccountService userAccountService { get; set; }

        // ------------------------------------------------------------ \\

        [CascadingParameter]
        public Task<AuthenticationState> authenticationState { get; set; }
        public string currentUser { get; set; } = "";

        //[CascadingParameter]
        //public protected Task<AuthenticationState> AuthStat { get; set; }

        public IGrid Grid { get; set; }
        public IGrid POListGrid { get; set; }
        public IGrid POShipmentListGrid { get; set; }
        public IGrid TestGrid { get; set; }

        public IEnumerable<ShippingData> GridData { get; set; }
        public IEnumerable<POOpenVendor> POOpenVendorData { get; set; }
        public IEnumerable<ShippingData> OpenPOShipmentData { get; set; }
        public IEnumerable<ShippingData> POListGridData { get; set; }
        public IEnumerable<ShippingData> POShipmentListGridData { get; set; }
        public IEnumerable<ShippingData> TestGridData { get; set; }
        public List<Lookup> openPOList { get; set; }

        public IGrid ProductFactoryPOsGrid { get; set; }
        public IEnumerable<SoEdiData> ProductFactoryPOsGridData { get; set; }

        // ------------------------------------------------------------ \\

        public string title;
        public int pageSize, scrollHeight;
        public string mainGridSectionHeight;
        public int windowHeight, windowWidth;
        public class WindowDimensions
        {
            public int Width { get; set; }
            public int Height { get; set; }
        }

        public int PageSize { get; set; } = 20;
        public int PageCount { get; set; } = 0;
        public int TotalRecords { get; set; } = 0;
        public int ActivePageIndex { get; set; } = 0;
        public bool ShowAllRows { get; set; } = true;

        public Dictionary<string, GridSearchTextParseMode> SearchTextParseModes { get; } = new Dictionary<string, GridSearchTextParseMode>{
            { "Group Words By And", GridSearchTextParseMode.GroupWordsByAnd },
            { "Group Words By Or", GridSearchTextParseMode.GroupWordsByOr },
            { "Exact Match", GridSearchTextParseMode.ExactMatch }
        };

        public void ChangeSearchMode(string key)
        {
            CurrentSearchTextParseModeDisplayText = key;
            CurrentSearchTextParseMode = SearchTextParseModes[key];
        }
        public string CurrentSearchTextParseModeDisplayText { get; set; } = "Group Words By And";
        public GridSearchTextParseMode CurrentSearchTextParseMode { get; set; } = GridSearchTextParseMode.GroupWordsByAnd;

        public GridColumnResizeMode CurrentColumnResizeMode { get; set; } = GridColumnResizeMode.ColumnsContainer; // GridColumnResizeMode.NextColumn;
        public string CurrentColumnResizeModeDisplayText { get; set; } = "Next Column";
        public Dictionary<string, GridColumnResizeMode> GridColumnResizeModes { get; } = new Dictionary<string,
        GridColumnResizeMode>{
            { "Disabled", GridColumnResizeMode.Disabled },                  //A user cannot resize columns.
            { "Next Column", GridColumnResizeMode.NextColumn },             //When a user resizes a column, the width of the column to the right changes, but the Grid's total width does not change.
            { "Columns Container", GridColumnResizeMode.ColumnsContainer }  //When a user resizes a column, all other columns retain width settings, but the width of the entire column container changes proportionally.
        };

        public void ChangeResizeMode(string key)
        {
            CurrentColumnResizeModeDisplayText = key;
            CurrentColumnResizeMode = GridColumnResizeModes[key];
        }

        public bool AutoCollapseDetailRow { get; set; }
        public TaskCompletionSource<bool> DataLoadedTcs { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public bool TextWrapEnabled = true;
        public bool WordWrapEnabled = false;

        public bool usePopupEditForm { get; set; } = true;
        public GridEditMode CurrentEditMode { get { return usePopupEditForm ? GridEditMode.PopupEditForm : GridEditMode.EditForm; } } // GridEditMode.EditRow // GridEditMode.EditCell
        public string mainGridEditFormHeaderText { get; set; } = "In Transit POs";
        public string gridCss => "hide-toolbar my-partnertasks-grid";

        public object SelectedDataItem { get; set; }
        public IReadOnlyList<object> SelectedDataItems { get; set; } // Items Selected in Edit Form
        public IReadOnlyList<object> SelectedDataPOListGridItems { get; set; }
        public IEnumerable<GridSelectAllCheckboxMode> SelectAllCheckboxModes { get; } = Enum.GetValues<GridSelectAllCheckboxMode>();
        public GridSelectAllCheckboxMode CurrentSelectAllCheckboxMode { get; set; }

        public ShippingData ShippingProductSelectedDataItem { get; set; }

        public bool isLoadingVisible { get; set; } = false;
        public string isLoadingMessage { get; set; } = "Loading data...";

        // ------------------------------------------------------------ \\

        public bool PopupVisible { get; set; } = false;
        public const string LocalStorageKey = "DialogsAndWindows-Popup-Dragging";
        public int? positionX, positionY;
        public bool allowDragByHeaderOnly = true;

        public string popupOkButtonText { get; set; } = "Ok";
        public string popupCancelButtonText { get; set; } = "Cancel";
        public bool isPopupCancelButtonVisible { get; set; } = false;
        public bool isPopupOkButtonEnabled { get; set; } = true;

        public string popupTitleText { get; set; } = "Notification";
        public string popupBodyText { get; set; } = "Sample Popup Message";

        public string callbackProcessName { get; set; } = null;
        public bool isDataSaving { get; set; } = false;

        // ------------------------------------------------------------ \\

        public string productFactoryPOsGridPopupTitleText { get; set; } = "Notification";
        public string productFactoryPOsGridPopupBodyText { get; set; } = "Sample Popup Message";

        public bool productFactoryPOsGridDataPopupVisible { get; set; } = false;
        public bool productFactoryPOsOkButtonEnabled { get; set; } = false;

        // ------------------------------------------------------------ \\

        public DateTime blankDate { get; set; } = new DateTime(1900, 1, 1);
        public string headerMessage { get; set; } = "Loading Data...";
        public bool hiddenGrid { get; set; } = true;
        public bool autoFitColWidths { get; set; } = true;
        public bool isAutoFitPending { get; set; } = true;
        public bool isVendorEditable { get; set; } = true;
        public bool selectCheckboxToEdit { get; set; } = false;
        string FocusedColumn { get; set; } = "OrderQty";
        public int POHeaderIdSelected { get; set; }
        public int ShipmentHeaderIdSelected { get; set; }
        public int editPOShipmentListGridHeight { get; set; } = 200;

        public ShippingData savingShippingDataObject { get; set; }

        public CriteriaOperator shippingGridFilterCriteria { get; set; }

        public string modeOfTransportation { get; set; } = "";
        public IEnumerable<string> modesOfTransportations = new[] { "Air", "Boat" };

        // ------------------------------------------------------------ \\

        public class BrowserDimension
        {
            public int Width { get; set; }
            public int Height { get; set; }
        }
        public int Height { get; set; } = 0;
        public int Width { get; set; } = 0;

        // ------------------------------------------------------------ \\

        public List<GridCell> clickedCells = new List<GridCell>();

        public class GridCell
        {
            public string colName { get; set; }
            public int rowIndex { get; set; }
            public GridCell(string _colName, int _rowIndex)
            {
                colName = _colName;
                rowIndex = _rowIndex;
            }
        }

        // ------------------------------------------------------------ \\

        #endregion

        // ============================================================ \\

        #region Constructors/Page Functions

        protected async override Task OnInitializedAsync()
        {
            base.OnInitialized();

            InitializeAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await AfterRenderAsync(firstRender);
        }

        public void Dispose()
        {
            headerMessage = "";
            hiddenGrid = true;
            // Northwind?.Dispose();
        }

        public async Task InitializeAsync()
        {
            isLoadingMessage = "Loading data...";
            isLoadingVisible = true;

            SqlData sqlData = new SqlData();
            GridData = await sqlData.GetShippingDetailData();           // Main grid data
            POOpenVendorData = await sqlData.GetPOOpenVendorData();     // All POs data
            //POOpenDetailData = await sqlData.GetPOOpenDetailData();
            OpenPOShipmentData = await sqlData.GetOpenPOShipmentData(); // All shipping data

            isLoadingMessage = "";
            isLoadingVisible = false;

            // ------------------------------------------------------------ \\

            isAutoFitPending = true;

            var user = (await authenticationState).User;
            if (!user.Identity.IsAuthenticated)
            {
            //NavigationManager.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(NavigationManager.Uri)}");
            //navManager.NavigateTo("/Login", true);
            hiddenGrid = true;
            }
            else
            {
                hiddenGrid = false;
                headerMessage = "";
            }

            var authstate = await authStateProvider.GetAuthenticationStateAsync();
            var userClaimsPrincipal = authstate.User; // ClaimsPrincipal
            var userClaimsPrincipalName = userClaimsPrincipal.Identity.Name;
            currentUser = userClaimsPrincipal.Identity.Name;

            if (userClaimsPrincipalName != null)
            {
                var userData = userAccountService.GetUserAccountFromClaims(userClaimsPrincipal);

                if (userData != null)
                {
                    if (userData.Role == "Administrator")
                    {
                        //workingMode = WorkingMode.Designer;
                        //showWorkingModeToggleButton = true;
                    }

                    //headers = new Dictionary<string, string>() {
                    //    { "Oid", userData.Oid.ToString() },
                    //    { "User", userData.UserName },
                    //    { "PromoCode", userData.PromoCode }
                    //};
                }
                else
                {

                }
            }
            else
            {
                //navManager.NavigateTo("/Login", true);

                userClaimsPrincipal = authenticationState.Result.User;
                var userData = userAccountService.GetUserAccountFromClaims(userClaimsPrincipal);

                if (userData != null)
                {
                    if (userData.Role == "Administrator")
                    {
                        //workingMode = WorkingMode.Designer;
                        //showWorkingModeToggleButton = true;
                    }

                    //headers = new Dictionary<string, string>() {
                    //    { "Oid", userData.Oid.ToString() },
                    //    { "User", userData.UserName },
                    //    { "PromoCode", userData.PromoCode }
                    //};
                }
                else
                {

                }
            }

            // ------------------------------------------------------------ \\

            var dim = await js.InvokeAsync<BrowserDimension>("getDimensions");
            Height = dim.Height;
            Width = dim.Width;

            try
            {
                if (Grid != null)
                {
                    Grid.BeginUpdate();
                    Grid.SortBy(CurrentSortInfoKey);
                    Grid.EndUpdate();
                }
            }
            catch
            {

            }
        }

        string CurrentSortInfoKey { get; set; } = "LastModifiedOn";

        public async Task AfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<string>("resizeListener", DotNetObjectReference.Create(this));
                var dimension = await js.InvokeAsync<WindowDimensions>("getWindowSize");
                windowHeight = dimension.Height;
                windowWidth = dimension.Width;
                SetPageSize(windowHeight);

                StateHasChanged();

                //await js.InvokeVoidAsync("window.registerViewportChangeCallback", DotNetObjectReference.Create(this));

                await DataLoadedTcs.Task; // Waits for grid data to load
                                          // Grid.ExpandDetailRow(0);
            }
            //CurrentColumnResizeMode = GridColumnResizeMode.ColumnsContainer;
            //    Grid.AutoFitColumnWidths(); 

            ////https://supportcenter.devexpress.com/ticket/details/t1207460/dxgrid-autofitcolumnwidths-true-does-not-set-widths-as-i-expected
            // if (AutoFitColWidths && IsAutoFitPending)
            //    {
            //        IsAutoFitPending = false;
            //        await Grid.WaitForDataLoadAsync();
            //        Grid.AutoFitColumnWidths();
            //    }

            if (Grid != null && GridData != null && isAutoFitPending)
            {
                isAutoFitPending = false;
                await Grid.WaitForDataLoadAsync();
                Grid.AutoFitColumnWidths();
            }

            if (POShipmentListGrid != null && POShipmentListGridData != null)
            {
                try
                {
                    POShipmentListGrid.SetFilterCriteria(shippingGridFilterCriteria);
                }
                catch (Exception ex)
                {

                }
            }
        }

        [JSInvokable]
        public void OnResize(int width, int height)
        {
            if (Width == width && Height == height) return;
            Width = width;
            Height = height;
            StateHasChanged();
        }

        [JSInvokable]
        public async void UpdatePage()
        {
            var dimension = await js.InvokeAsync<WindowDimensions>("getWindowSize");
            windowHeight = dimension.Height;
            windowWidth = dimension.Width;
            SetPageSize(windowHeight);
            StateHasChanged();
        }

        public void SetPageSize(int height)
        {
            pageSize = ((height - 195) / 30);   // Not used
            title = height.ToString() + "/" + pageSize.ToString(); // Not used

            int offset = 80;
            scrollHeight = height - 195 - offset;
            mainGridSectionHeight = (height - offset).ToString() + "px";
        }

        #endregion

        // ============================================================ \\

        #region Popup Functions

        public async void OkPopupClick()
        {
            isPopupOkButtonEnabled = false;
            PopupVisible = false;

            //if (callbackProcessName == "SaveLinks")
            //{
            //    //@PopupLinkingFormContext.CloseCallback
            //    windowRefLinking.CloseAsync();
            //
            //    await SaveCustomerPOLinking();
            //}
            if (callbackProcessName == "SaveShippingData-NEW")
            {
                if (!isDataSaving)
                {
                    headerMessage = "Saving Data...";
                    isDataSaving = true;

                    await Grid.CancelEditAsync();
                    await InvokeAsync(StateHasChanged);

                    try
                    {
                        await InsertShippingDataAsync(savingShippingDataObject);
                    }
                    catch (Exception ex)
                    {
                        DisplayPopupMessage("Error Saving Shipping Data (Insert):" + ex.Message);
                    }
                    isDataSaving = false;
                    headerMessage = "";

                    await RefreshData_Click();
                }
                else
                {
                    try
                    {
                        await Grid.CancelEditAsync();
                        await InvokeAsync(StateHasChanged);
                    }
                    catch
                    {

                    }
                }
            }
            if (callbackProcessName == "SaveShippingData-UDPATE")
            {
                if (!isDataSaving)
                {
                    headerMessage = "Saving Data...";
                    isDataSaving = true;

                    await Grid.CancelEditAsync();
                    await InvokeAsync(StateHasChanged);

                    try
                    {
                        await UpdateShippingDataAsync(savingShippingDataObject);
                    }
                    catch (Exception ex)
                    {
                        DisplayPopupMessage("Error Saving Shipping Data (Update):" + ex.Message);
                    }
                    isDataSaving = false;
                    headerMessage = "";

                    await RefreshData_Click();
                }
                else
                {
                    try
                    {
                        await Grid.CancelEditAsync();
                        await InvokeAsync(StateHasChanged);
                    }
                    catch
                    {

                    }
                }
            }
        }

        public void CancelPopupClick()
        {
            PopupVisible = false;
            //linkingEdiPOtoSO = false;

            if (callbackProcessName == "SaveShippingData-NEW")
            {
                //savingShippingDataObject = null;
            }
            if (callbackProcessName == "SaveShippingData-UDPATE")
            {
                //savingShippingDataObject = null;
            }
        }

        public void DisplayPopupMessage(string message, string title = "Notification", string callbackName = null)
        {
            isPopupOkButtonEnabled = true;
            isPopupCancelButtonVisible = false;
            popupOkButtonText = "Ok";
            popupCancelButtonText = "Cancel";
            popupTitleText = title;
            popupBodyText = message;
            PopupVisible = true;

            callbackProcessName = callbackName;
        }

        public void DisplayPopupQuestion(string message, string title = "Alert", string callbackName = null)
        {
            isPopupOkButtonEnabled = true;
            isPopupCancelButtonVisible = true;
            popupOkButtonText = "Yes";
            popupCancelButtonText = "No";
            popupTitleText = title;
            popupBodyText = message;
            PopupVisible = true;

            callbackProcessName = callbackName;
        }

        #endregion

        // ============================================================ \\

        #region Load/Refresh Functions

        public void AutoCollapseDetailRow_Changed(bool newValue)
        {
            AutoCollapseDetailRow = newValue;
            if (newValue)
            {
                Grid.BeginUpdate();
                Grid.CollapseAllDetailRows();
                Grid.ExpandDetailRow(0);
                Grid.EndUpdate();
            }
        }

        public List<Lookup> GetPOListData(int vendorId)
        {
            List<Lookup> list = new List<Lookup>();

            var polist = (vendorId > 0) ?
                    OpenPOShipmentData.Where(x => x.VendorId == vendorId).Select(x => x.PONumber).Distinct().ToList()
                    : OpenPOShipmentData.Select(x => x.PONumber).Distinct().ToList();
            foreach (string po in polist)
            {
                Lookup l = new Lookup()
                {
                    LookupText = po,
                    LookupValue = OpenPOShipmentData.Where(c => c.PONumber == po).FirstOrDefault().POHeaderId
                };
                list.Add(l);
            }
            return list;
        }

        public async Task AddToPOListGridDataByPO(ShippingData poData, bool isNew = false)
        {
            if (poData != null)
            {
                bool isAddon = false;
                //bool isNew = (poData.ShipmentHeaderId == 0) ? true : false;
                int incomingUnitsSum = 0;

                // Append to POShipmentListGridData
                IEnumerable<ShippingData> poShipmentList = OpenPOShipmentData.Where(c => c.PONumber == poData.PONumber);
                //incomingUnitsSum = (poShipmentList.Count() == 0) ? 0 : poShipmentList.Select(x => x.LastShipmentQty).Sum();
                incomingUnitsSum = (poShipmentList.Count() == 0) ? 0 : poShipmentList.Select(x => x.ShipmentQty).Sum();
                poData.TotalSum = incomingUnitsSum;

                if (POListGridData == null)
                {
                    isAddon = true;
                    //POListGridData = POListGridData.Concat(new[] { poData });
                    POListGridData = new List<ShippingData>()
                    {
                        poData
                    };
                }
                else
                {
                    ShippingData poOListGridDataItem = POListGridData.Where(c => c.POHeaderId == poData.POHeaderId).FirstOrDefault();
                    if (poOListGridDataItem == null) // if NOT found
                    {
                        isAddon = true;
                        POListGridData = POListGridData.Concat(new[] { poData });
                    }
                }

                SelectedDataPOListGridItems = new List<object>();
                SelectedDataPOListGridItems.Append(poData);

                if (POShipmentListGridData == null)
                {
                    POShipmentListGridData = poShipmentList;
                }
                else
                {
                    if (isNew)
                    {
                        SqlData sqlData = new SqlData();
                        IEnumerable<ShippingData> poShipmentData = await sqlData.GetOpenPOShipmentData(poData.PONumber, true);
                        foreach(ShippingData shipment in poShipmentData)
                        {
                            shipment.ShipmentQty = 0;
                        }
                        POShipmentListGridData = poShipmentData;
                        POShipmentListGrid.ClearFilter();
                    }
                    else
                    if (isAddon)
                    {
                        POShipmentListGridData = POShipmentListGridData.Concat(poShipmentList);
                    }
                }
            }
            else
            {

            }
        }
        public void AddToPOListGridDataByShipmentHeaderId(ShippingData poData)
        {
            if (poData != null)
            {
                bool isAddon = false;
                int incomingUnitsSum = 0;

                // Append to POShipmentListGridData
                //IEnumerable<ShippingData> poShipmentList = OpenPOShipmentData.Where(c => c.PONumber == poData.PONumber);
                IEnumerable<ShippingData> poShipmentList = OpenPOShipmentData.Where(c => c.ShipmentHeaderId == poData.ShipmentHeaderId);
                //incomingUnitsSum = (poShipmentList.Count() == 0) ? 0 : poShipmentList.Select(x => x.LastShipmentQty).Sum();
                incomingUnitsSum = (poShipmentList.Count() == 0) ? 0 : poShipmentList.Select(x => x.ShipmentQty).Sum();
                poData.TotalSum = incomingUnitsSum;

                if (POListGridData == null)
                {
                    isAddon = true;
                    //POListGridData = POListGridData.Concat(new[] { poData });
                    POListGridData = new List<ShippingData>()
                    {
                        poData
                    };
                }
                else
                {
                    ShippingData poOListGridDataItem = POListGridData.Where(c => c.POHeaderId == poData.POHeaderId).FirstOrDefault();
                    if (poOListGridDataItem == null) // if NOT found
                    {
                        isAddon = true;
                        POListGridData = POListGridData.Concat(new[] { poData });
                    }
                }

                SelectedDataPOListGridItems = new List<object>();
                SelectedDataPOListGridItems.Append(poData);

                if (POShipmentListGridData == null)
                {
                    POShipmentListGridData = poShipmentList;
                }
                else
                {
                    if (isAddon)
                    {
                        POShipmentListGridData = POShipmentListGridData.Concat(poShipmentList);
                    }
                }
            }
            else
            {

            }
        }

        public async Task LoadItemGridDataByPO(string poNo)
        {
            //POShipmentListGridData = OpenPOShipmentData.Where(c => c.PONumber == poNo);
            TestGridData = OpenPOShipmentData.Where(c => c.PONumber == poNo);

            var criteria = new InOperator("PONumber", new string[] { poNo });
            shippingGridFilterCriteria = criteria;

            if (POShipmentListGrid != null)
            {
                try
                {
                    POShipmentListGrid.SetFilterCriteria(criteria); 
                }
                catch (Exception ex)
                {

                }
            }
            else
            {

            }
            clickedCells = new List<GridCell>();
        }

        public async Task LoadItemGridDataByShipmentHeaderId(int shipmentHeaderId)
        {
            TestGridData = OpenPOShipmentData.Where(c => c.ShipmentHeaderId == shipmentHeaderId);

            var criteria = new InOperator("ShipmentHeaderId", new int[] { shipmentHeaderId });
            shippingGridFilterCriteria = criteria;

            if (POShipmentListGrid != null)
            {
                try
                {
                    POShipmentListGrid.SetFilterCriteria(criteria);
                }
                catch (Exception ex)
                {

                }
            }
            else
            {

            }
            clickedCells = new List<GridCell>();
        }
        
        #endregion

        // ============================================================ \\

        #region Button Functions

        public void Grid_FitWidths()
        {
            Grid.AutoFitColumnWidths();
        }
        public void ColumnChooserButton_Click()
        {
            Grid.ShowColumnChooser();
        }
        public async Task RefreshData_Click()
        {
            isLoadingMessage = "Loading data...";
            isLoadingVisible = true;

            SqlData sqlData = new SqlData();
            GridData = await sqlData.GetShippingDetailData();           // Main grid data
            POOpenVendorData = await sqlData.GetPOOpenVendorData();     // All POs data
            OpenPOShipmentData = await sqlData.GetOpenPOShipmentData(); // All shipping data

            isLoadingMessage = "";
            isLoadingVisible = false;

            await InvokeAsync(StateHasChanged); // <-- refreshes
        }
        public async Task UsePopupEditForm_CheckedChanged(bool value)
        {
            usePopupEditForm = value;
            await Grid.CancelEditAsync();
        }

        #endregion

        // ============================================================ \\

        #region Main Grid Functions

        public void Grid_CustomizeElement(GridCustomizeElementEventArgs e)
        {
            //if (e.ElementType != GridElementType.DataCell) return;
            //GridCell cell = clickedCells.Find(x => x.colName == (e.Column as DxGridDataColumn).FieldName && x.rowIndex == e.VisibleIndex);
            //if (cell != null)
            //    e.CssClass = "highlighted-item";
            //else
            //    e.CssClass = string.Empty;
        }
        public void Grid_CustomizeCellDisplayText(GridCustomizeCellDisplayTextEventArgs e)
        {
            //string[] dateList = { "SODate", "StartDate", "EndDate", "PODate", "ShipmentDate", "ShipToETA", "FactoryCancel" };
            //if (dateList.Contains(e.FieldName)) 
            if (e.Value.GetType() == typeof(DateTime))
            {
                // e.DisplayText = CustomerList.Where(p => p.CustomerId == ((Customer)e.Value).CustomerId).FirstOrDefault().CompanyName;
                if (DateTime.Parse(e.Value.ToString()) <= new DateTime(1900, 1, 1))
                    e.DisplayText = "";
            }
        }

        public void Grid_CustomGroup(GridCustomGroupEventArgs e)
        {
            if (e.FieldName == "TrackingNumber")
            {
                //e.SameGroup = Grid_CompareColumnValues(e.Value1, e.Value2) == 0;
                e.SameGroup = Grid_CompareColumnValues(e.Value1, e.Value2) == 0;
                e.Handled = true;
            }
        }
        public int Grid_CompareColumnValues(object value1, object value2)
        {
            //double val1 = Math.Floor(Convert.ToDouble(value1) / 10);
            //double val2 = Math.Floor(Convert.ToDouble(value2) / 10);
            //var res = System.Collections.Comparer.Default.Compare(val1, val2);
            //if (res < 0)
            //    res = -1;
            //else if (res > 0)
            //    res = 1;
            //if (res == 0 || (val1 > 9 && val2 > 9))
            //    res = 0;

            string val1 = (string)value1;
            string val2 = (string)value2;
            var res = System.Collections.Comparer.Default.Compare(val1, val2);

            return res;
        }
        public void Grid_CustomizeGroupValueDisplayText(GridCustomizeGroupValueDisplayTextEventArgs e)
        {
            bool customizeTrackingNumber = false;

            if (e.FieldName == "TrackingNumber" && customizeTrackingNumber)
            {
                IEnumerable<ShippingData> gridList = e.Grid.Data as IEnumerable<ShippingData>;

                string displayText = "";
                DateTime blankDate = new DateTime(1900, 1, 1);
                string trackingNumber = e.Value.ToString();
                if (trackingNumber != null) //if (!string.IsNullOrWhiteSpace(trackingNumber))
                {
                    List<ShippingData> trackingNumberList = gridList.Where(c => c.TrackingNumber != null && c.TrackingNumber == trackingNumber).ToList();

                    string transportModes = string.Join(",", trackingNumberList.Select(x => x.TransportationMode).Distinct().ToList());
                    //string shipDate = trackingNumberList.Where(x => x.ShipmentDate > blankDate).Select(x => x.ShipmentDate).DefaultIfEmpty(blankDate).Max().ToString("MM/dd");
                    //string etaDate = trackingNumberList.Where(x => x.ShipToETA > blankDate).Select(x => x.ShipToETA).DefaultIfEmpty(blankDate).Max().ToString("MM/dd");
                    string shipDate = string.Join(",", trackingNumberList.Where(x => x.ShipmentDate > blankDate).Select(x => x.ShipmentDate).Distinct().ToList().Select(x => x.ToString("MM/dd")));
                    string etaDate = string.Join(",", trackingNumberList.Where(x => x.ShipToETA > blankDate).Select(x => x.ShipToETA).Distinct().ToList().Select(x => x.ToString("MM/dd")));
                    int totalUnits = trackingNumberList.Select(x=> x.OrderQty).Sum();
                    string invoiceNos = string.Join(",", trackingNumberList.Select(x => x.InvoiceNo).Distinct().ToList());

                    if (string.IsNullOrWhiteSpace(trackingNumber))  trackingNumber = "None";
                    if (string.IsNullOrWhiteSpace(transportModes))  transportModes = "None";
                    if (string.IsNullOrWhiteSpace(shipDate))        shipDate = "None";
                    if (string.IsNullOrWhiteSpace(etaDate))         etaDate = "None";
                    if (string.IsNullOrWhiteSpace(invoiceNos))      invoiceNos = "None";

                    displayText = string.Format("{0} [Transport Modes:{1}] [Ship:{2}] [ETA:{3}] [Total Units:{4}] [Invoices:{5}] ", trackingNumber, transportModes, shipDate, etaDate,  totalUnits, invoiceNos);
                }

                //double val = Math.Floor(Convert.ToDouble(e.Value) / 10);
                //displayText = string.Format("{0:c} - {1:c} ", val * 10, (val + 1) * 10);
                //if (val > 9)
                //    displayText = string.Format(">= {0:c} ", 100);
                e.DisplayText = displayText;
            }
        }

        public async Task Grid_CustomizeEditModel(GridCustomizeEditModelEventArgs e)
        {
            if (e.EditModel == null)
                return;

            if (e.IsNew)
            {
                if (e.EditModel.GetType() == typeof(ShippingData))
                {
                    var newObject = (ShippingData)e.EditModel;
                    newObject.Id = GridData.Count() + 1;
                    newObject.ShipmentDate = DateTime.Now;
                    newObject.ShipToETA = blankDate;

                    POHeaderIdSelected = 0;
                    isVendorEditable = true;

                    //ItemGridData = new List<POOpenDetail>();
                    //POListGridData = new List<POOpenDetail>();

                    POListGridData = new List<ShippingData>();
                    POShipmentListGridData = new List<ShippingData>();
                }
            }
            else
            {
                if (e.EditModel.GetType() == typeof(ShippingData))
                {
                    var editObject = ((ShippingData)e.EditModel);

                    POHeaderIdSelected = editObject.POHeaderId;
                    ShipmentHeaderIdSelected = editObject.ShipmentHeaderId;

                    if (ShipmentHeaderIdSelected == 0) // New
                    {
                        await LoadItemGridDataByPO(editObject.PONumber);

                        POListGridData = new List<ShippingData>();
                        POShipmentListGridData = new List<ShippingData>();

                        ShippingData poItem = OpenPOShipmentData.Where(c => c.PONumber == ((ShippingData)e.EditModel).PONumber).FirstOrDefault();
                        AddToPOListGridDataByPO(poItem);

                    }
                    else // Update
                    {
                        await LoadItemGridDataByShipmentHeaderId(editObject.ShipmentHeaderId);

                        POListGridData = new List<ShippingData>();
                        POShipmentListGridData = new List<ShippingData>();

                        ShippingData poItem = OpenPOShipmentData.Where(c => c.ShipmentHeaderId == ((ShippingData)e.EditModel).ShipmentHeaderId).FirstOrDefault();
                        AddToPOListGridDataByShipmentHeaderId(poItem);
                    }

                    isVendorEditable = false;

                    await InvokeAsync(StateHasChanged);
                }
            }
        }
        public async Task Grid_EditModelSaving(GridEditModelSavingEventArgs e)
        {
            e.Cancel = true;

            if (e.EditModel.GetType() == typeof(ShippingData))
            {
                var savingObject = (ShippingData)e.EditModel;

                savingShippingDataObject = savingObject;
                string savingDataType = (e.IsNew) ? "SaveShippingData-NEW": "SaveShippingData-UDPATE";

                string message = "Are you sure you want to " + Environment.NewLine + "Shipping Data?";
                DisplayPopupQuestion(message, "Confirmation", savingDataType);

                //if (e.IsNew)
                //    await InsertShippingDataAsync(savingObject);
                //else
                //    await UpdateShippingDataAsync(savingObject);

                //await Grid.CancelEditAsync();
            }
        }
        public async Task Grid_DataItemDeleting(GridDataItemDeletingEventArgs e)
        {
            if (e.DataItem.GetType() == typeof(BOMData))
            {
                var deletingObject = (BOMData)e.DataItem;

            }
            if (e.DataItem.GetType() == typeof(FreightData))
            {
                var deletingObject = (FreightData)e.DataItem;

            }

            if (e.DataItem.GetType() == typeof(ShippingData))
            {
                var deletingObject = (ShippingData)e.DataItem;

            }

            // await NwindDataService.RemoveEmployeeAsync((EditableEmployee)e.DataItem);
            //await UpdateDataAsync();
        }

        //Saving...
        public async Task InsertShippingDataAsync(ShippingData item)
        {            
            bool onePushProcess = true;
            string completeQuery = "";

            bool alwaysInsert = true; // true changed 040925
            int detailSaveCount = 0;
            int detailUpdateCount = 0;

            // Don't save if empty detail list
            if (POShipmentListGridData.Count() == 0)
                return;

            if (item.TrackingNumber == null) item.TrackingNumber = "";
            if (item.InvoiceNo == null) item.InvoiceNo = "";

            string query = @"
INSERT INTO [PIMS].[dbo].[ShipmentHeader] ([ShipmentDate],[TrackingNumber],[InvoiceNo],[CreatedOn],[ShipToETA],[TransportationMode]) 
SELECT '{0}','{1}','{2}',GETDATE(),'{3}', '{4}'
SELECT @@IDENTITY AS 'pk'";
            string fullQuery = string.Format(query, item.ShipmentDate, item.TrackingNumber, item.InvoiceNo, item.ShipToETA, item.TransportationMode);

            int shipmentHeaderId = 0;
            using (var uow = new UnitOfWork())
            {
                DevExpress.Xpo.DB.SelectedData selectedData = uow.ExecuteQuery(fullQuery);

                if (selectedData.ResultSet.Length > 0)
                {
                    DevExpress.Xpo.DB.SelectStatementResult result = selectedData.ResultSet[0];

                    if (result.Rows.Count() > 0)
                    {
                        string rValue = result.Rows[0].Values[0].ToString();
                        int.TryParse(rValue, out shipmentHeaderId);
                    }
                }
            }
            if (shipmentHeaderId > 0)
            {
                //POShipmentListGridData.Where(c => c.Id == ship.Id).FirstOrDefault().ShipmentDetailId = shipmentDetailId;
                //ship.ShipmentDetailId = shipmentDetailId;
            }

            // Get list of Rows Selected. See SelectedDataItems list
            List<string> productlist = new List<string>();
            List<int> soDetailIds = new List<int>();
            if (SelectedDataItems != null && SelectedDataItems.Count > 0)
            {
                productlist = SelectedDataItems.Select(x => (x as ShippingData).ProductNo).Distinct().ToList();
                soDetailIds = SelectedDataItems.Select(x => (x as ShippingData).SODetailId).Distinct().ToList();
            }

            // Save POShipmentListGrid data
            foreach (ShippingData ship in POShipmentListGridData)
            {
                if (soDetailIds.Contains(ship.SODetailId) || alwaysInsert)
                {
                    if (ship.ShipmentDetailId == 0 || alwaysInsert)
                    {
                        //int shipmentHeaderId = item.ShipmentHeaderId;
                        int poDetailId = ship.PODetailId;
                        int shipmentQty = ship.ShipmentQty;

                        // NEW insert
                        string detailInsertQuery = @"
INSERT INTO [PIMS].[dbo].[ShipmentDetails] ([ShipmentHeaderId],[PODetailId],[ShipmentQty],[CreatedOn])
SELECT {0},{1},{2}, GETDATE()

UPDATE [PIMS].[dbo].[PODetail]
SET [ReceivedQty] = (SELECT SUM([ShipmentQty]) FROM [PIMS].[dbo].[ShipmentDetails] WHERE [PODetailId] = {1})
    ,[LastModifiedOn] = GETDATE()
WHERE [PODetailId] = {1}

SELECT @@IDENTITY AS 'pk'";
                        string detailInsertFullQuery = string.Format(detailInsertQuery, shipmentHeaderId, poDetailId, shipmentQty);

                        int shipmentDetailId = 0;
                        if (shipmentQty > 0)
                        {
                            detailSaveCount++;

                            if (!onePushProcess)
                            {
                                using (var uow = new UnitOfWork())
                                {
                                    DevExpress.Xpo.DB.SelectedData selectedData = uow.ExecuteQuery(detailInsertFullQuery);

                                    if (selectedData.ResultSet.Length > 0)
                                    {
                                        DevExpress.Xpo.DB.SelectStatementResult result = selectedData.ResultSet[0];

                                        if (result.Rows.Count() > 0)
                                        {
                                            string rValue = result.Rows[0].Values[0].ToString();
                                            int.TryParse(rValue, out shipmentDetailId);
                                        }
                                    }
                                }
                                if (shipmentDetailId > 0)
                                {
                                    //POShipmentListGridData.Where(c => c.Id == ship.Id).FirstOrDefault().ShipmentDetailId = shipmentDetailId;
                                    ship.ShipmentDetailId = shipmentDetailId;
                                }
                                ship.LastShipmentQty = ship.ShipmentQty;
                                //UpdateOpenPOShipmentData(ship);
                            }
                            else
                            {
                                completeQuery += detailInsertFullQuery + @"
";
                            }

                            // EDI QTY Allocation
                            if (ship.AllocatedQuantities != null)
                            {
                                string transactionType = "";

                                string allocQtyUpdateQuery_replace = @"
UPDATE [PIMS].[edi].[EdiTrn]
SET [AllocatedQty] = {0}
    ,[LastModifiedOn] = GETDATE()
WHERE [Editrnid] IN (SELECT trn.[Editrnid]
FROM [PIMS].[edi].[EdiTrn] trn
LEFT JOIN [PIMS].[edi].[EdiHdr] hdr
	ON trn.[Edihdrid] = hdr.[Edihdrid]
WHERE hdr.[Edihdrid] = {1} AND trn.[EdiTrnId] = {2})

UPDATE [PIMS].[dbo].[SODetail]
SET [AllocatedQty] = {0}
    ,[LastModifiedOn] = GETDATE()
WHERE [SODetailId] = {3}

";
                                string allocQtyUpdateQuery_adjust = @"
UPDATE [PIMS].[edi].[EdiTrn]
SET [AllocatedQty] = ISNULL([AllocatedQty], 0) + {0}
    ,[LastModifiedOn] = GETDATE()
WHERE [Editrnid] IN (SELECT trn.[Editrnid]
FROM [PIMS].[edi].[EdiTrn] trn
LEFT JOIN [PIMS].[edi].[EdiHdr] hdr
	ON trn.[Edihdrid] = hdr.[Edihdrid]
WHERE hdr.[Edihdrid] = {1} AND trn.[EdiTrnId] = {2})

UPDATE [PIMS].[dbo].[SODetail]
SET [AllocatedQty] = (SELECT [AllocatedQty] FROM [PIMS].[edi].[EdiTrn] WHERE [Edihdrid] = {1} AND [EdiTrnId] = {2})
    ,[LastModifiedOn] = GETDATE()
WHERE [SODetailId] = {3}

";
                                string allocQtyUpdateFullQuery = "";

                                foreach (SoEdiData alloc in ship.AllocatedQuantities)
                                {
                                    transactionType = alloc.TransactionType;
                                    // "Multi": On "New" or "Edit": Replace
                                    // "Single": On "New": Add To Existing. On "Edit" Adjust with difference

                                    int soHeaderId = alloc.SOHeaderId;
                                    int soDetailId = alloc.SoDetailId;
                                    int ediHdrId = alloc.EdiHdrId;
                                    int ediTrnId = alloc.EdiTrnId;
                                    int inTransitUnits = alloc.IntransitUnits;  // Original Qty
                                    int allocQty = alloc.AllocatedQty;          // Adjusted Qty
                                    int adjustQty = allocQty - inTransitUnits;
                                    //int productId = alloc.ProductId;
                                    //string itemNo = alloc.ItemNo;
                                    //allocQtyUpdateFullQuery += string.Format(allocQtyUpdateQuery_replace, allocQty, ediHdrId, ediTrnId); 
                                    if (transactionType == "Multi")
                                        allocQtyUpdateFullQuery += string.Format(allocQtyUpdateQuery_replace, allocQty, ediHdrId, ediTrnId, soDetailId);
                                    else
                                        allocQtyUpdateFullQuery += string.Format(allocQtyUpdateQuery_adjust, allocQty, ediHdrId, ediTrnId, soDetailId);
                                }
                                try
                                {
                                    if (!onePushProcess)
                                    {
                                        using (var uow = new UnitOfWork())
                                        {
                                            await uow.ExecuteNonQueryAsync(allocQtyUpdateFullQuery);
                                        }
                                    }
                                    else
                                    {
                                        completeQuery += allocQtyUpdateFullQuery + @"
";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    DisplayPopupMessage("Error (Insert) AllocatedQuatities:" + ex.Message + ". AllocQtyQuery:" + allocQtyUpdateFullQuery);
                                }
                            }
                        }
                    }
                    else // update...
                    {
                        if (ship.OrderQty != ship.ShipmentQty && ship.ShipmentQty != ship.LastShipmentQty)
                        {
                            detailUpdateCount++;

                            string detailUpdateQuery = @"
UPDATE [PIMS].[dbo].[ShipmentDetails] 
SET [ShipmentQty] = {2}
    , [LastModifiedOn] = GETDATE() 
WHERE [ShipmentDetailId] = {0}

UPDATE [PIMS].[dbo].[PODetail] 
SET [ReceivedQty] = (SELECT SUM([ShipmentQty]) FROM [PIMS].[dbo].[ShipmentDetails] WHERE [PODetailId] = {1})
    ,[LastModifiedOn] = GETDATE()
WHERE [PODetailId] = {1}
";
                            string detailUpdateFullQuery = string.Format(detailUpdateQuery, ship.ShipmentDetailId, ship.PODetailId, ship.ShipmentQty);

                            if (!onePushProcess)
                            {
                                try
                                {
                                    using (var uow = new UnitOfWork())
                                    {
                                        await uow.ExecuteNonQueryAsync(detailUpdateFullQuery);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    DisplayPopupMessage("Error (Update) PODetail/ShipmentDetails:" + ex.Message + ". DetailUpdateQuery:" + detailUpdateFullQuery);
                                }
                                ship.LastShipmentQty = ship.ShipmentQty;
                                //UpdateOpenPOShipmentData(ship);
                            }
                            else
                            {
                                completeQuery += detailUpdateFullQuery + @"
";
                            }

                            // EDI QTY Allocation
                            if (ship.AllocatedQuantities != null)
                            {
                                string transactionType = "";

                                string allocQtyUpdateFullQuery = "";

                                string allocQtyUpdateQuery_replace = @"
UPDATE [PIMS].[edi].[EdiTrn]
SET [AllocatedQty] = {0}
    ,[LastModifiedOn] = GETDATE()
WHERE [Editrnid] IN (SELECT trn.[Editrnid]
FROM [PIMS].[edi].[EdiTrn] trn
LEFT JOIN [PIMS].[edi].[EdiHdr] hdr
	ON trn.[Edihdrid] = hdr.[Edihdrid]
WHERE hdr.[Edihdrid] = {1} AND trn.[EdiTrnId] = {2})

UPDATE [PIMS].[dbo].[SODetail]
SET [AllocatedQty] = {0}
    ,[LastModifiedOn] = GETDATE()
WHERE [SODetailId] = {3}
";
                                string allocQtyUpdateQuery_adjust = @"
UPDATE [PIMS].[edi].[EdiTrn]
SET [AllocatedQty] = ISNULL([AllocatedQty], 0) + {0}
    ,[LastModifiedOn] = GETDATE()
WHERE [Editrnid] IN (SELECT trn.[Editrnid]
FROM [PIMS].[edi].[EdiTrn] trn
LEFT JOIN [PIMS].[edi].[EdiHdr] hdr
	ON trn.[Edihdrid] = hdr.[Edihdrid]
WHERE hdr.[Edihdrid] = {1} AND trn.[EdiTrnId] = {2})

UPDATE [PIMS].[dbo].[SODetail]
SET [AllocatedQty] = (SELECT [AllocatedQty] FROM [PIMS].[edi].[EdiTrn] WHERE [Edihdrid] = {1} AND [EdiTrnId] = {2})
    ,[LastModifiedOn] = GETDATE()
WHERE [SODetailId] = {3}
";

                                foreach (SoEdiData alloc in ship.AllocatedQuantities)
                                {
                                    transactionType = alloc.TransactionType;
                                    // "Multi": On "New" or "Edit": Replace
                                    // "Single": On "New": Add To Existing. On "Edit" Adjust with difference

                                    int soHeaderId = alloc.SOHeaderId;
                                    int soDetailId = alloc.SoDetailId;
                                    int ediHdrId = alloc.EdiHdrId;
                                    int ediTrnId = alloc.EdiTrnId;
                                    int inTransitUnits = alloc.IntransitUnits;  // Original Qty
                                    int allocQty = alloc.AllocatedQty;          // Adjusted Qty
                                    int adjustQty = allocQty - inTransitUnits;
                                    //int productId = alloc.ProductId;
                                    //string itemNo = alloc.ItemNo;
                                    if (transactionType == "Multi")
                                        allocQtyUpdateFullQuery += string.Format(allocQtyUpdateQuery_replace, allocQty, ediHdrId, ediTrnId, soDetailId);
                                    else
                                        allocQtyUpdateFullQuery += string.Format(allocQtyUpdateQuery_adjust, allocQty, ediHdrId, ediTrnId, soDetailId);
                                }

                                try
                                {
                                    if (!onePushProcess)
                                    {
                                        using (var uow = new UnitOfWork())
                                        {
                                            await uow.ExecuteNonQueryAsync(allocQtyUpdateFullQuery);
                                        }
                                    }
                                    else
                                    {
                                        completeQuery += allocQtyUpdateFullQuery + @"
";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    DisplayPopupMessage("Error (Insert) AllocatedQuatities:" + ex.Message + ". AllocQtyDetailUpdateQuery:" + allocQtyUpdateFullQuery);
                                }
                            }
                        }
                    }
                }
            }

            if (onePushProcess && !string.IsNullOrWhiteSpace(completeQuery))
            {
                try
                {
                    using (var uow = new UnitOfWork())
                    {
                        await uow.ExecuteNonQueryAsync(completeQuery);
                    }
                }
                catch (Exception ex)
                {
                    DisplayPopupMessage("Error (OnePush Insert) AllocatedQuatities:" + ex.Message + ". AllocQtyDetailUpdateQuery:" + completeQuery);
                }
            }

            if (detailSaveCount == 0 && detailUpdateCount == 0)
            {
                string message = "No data saved." + Environment.NewLine + "Only 0 quantities.";
                DisplayPopupMessage(message);
            }

            headerMessage = "Refreshing Data...";
            SqlData sqlData = new SqlData();
            GridData = await sqlData.GetShippingDetailData();
            OpenPOShipmentData = await sqlData.GetOpenPOShipmentData();

            await InvokeAsync(StateHasChanged); // <-- refreshes
            headerMessage = "";
        }

        //Saving...
        public async Task UpdateShippingDataAsync(ShippingData item)
        {
            bool onePushProcess = true;
            string completeQuery = "";
            bool alwaysUpdate = true;

            // Don't save if empty detail list
            if (POShipmentListGridData.Count() == 0)
                return;

            int detailSaveCount = 0;
            int detailUpdateCount = 0;

            string query = @"
UPDATE [PIMS].[dbo].[ShipmentHeader] SET
    [ShipmentDate] = '{1}'
    ,[TrackingNumber] = '{2}'
    ,[ShipToETA] = '{3}'
    ,[InvoiceNo] = '{4}'
    ,[LastModifiedOn] = GETDATE()
    ,[TransportationMode] = '{5}'
WHERE [ShipmentHeaderId] = {0}";
            string fullQuery = string.Format(query, item.ShipmentHeaderId, item.ShipmentDate, item.TrackingNumber, item.ShipToETA, item.InvoiceNo, item.TransportationMode);

            if (!onePushProcess)
            {
                using (var uow = new UnitOfWork())
                {
                    await uow.ExecuteNonQueryAsync(fullQuery);
                }
            }
            else
            {
                completeQuery += fullQuery + @"
";
            }

            // Get list of Rows Selected. See SelectedDataItems list
            List<string> productlist = new List<string>();
            List<int> soDetailIds = new List<int>();
            if (SelectedDataItems != null && SelectedDataItems.Count > 0)
            {
                productlist = SelectedDataItems.Select(x => (x as ShippingData).ProductNo).Distinct().ToList();
                soDetailIds = SelectedDataItems.Select(x => (x as ShippingData).SODetailId).Distinct().ToList();
            }

            // Save POShipmentListGrid data
            foreach (ShippingData ship in POShipmentListGridData)
            {
                if (soDetailIds.Contains(ship.SODetailId) || alwaysUpdate)
                {
                    if (ship.ShipmentDetailId == 0 && ship.ShipmentQty != ship.LastShipmentQty)
                    {
                        detailSaveCount++;

                        int shipmentHeaderId = item.ShipmentHeaderId;
                        int poDetailId = ship.PODetailId;
                        int shipmentQty = ship.ShipmentQty;

                        // NEW insert
                        string detailInsertQuery = @"
INSERT INTO [PIMS].[dbo].[ShipmentDetails] ([ShipmentHeaderId],[PODetailId],[ShipmentQty],[CreatedOn])
SELECT {0},{1},{2}, GETDATE()

UPDATE [PIMS].[dbo].[PODetail] 
SET [ReceivedQty] = (SELECT SUM([ShipmentQty]) FROM [PIMS].[dbo].[ShipmentDetails] WHERE [PODetailId] = {1})
    ,[LastModifiedOn] = GETDATE()
WHERE [PODetailId] = {1}

SELECT @@IDENTITY AS 'pk'";
                        string detailInsertFullQuery = string.Format(detailInsertQuery, shipmentHeaderId, poDetailId, shipmentQty, ship.ShipmentDetailId);

                        if (!onePushProcess)
                        {
                            int shipmentDetailId = 0;
                            using (var uow = new UnitOfWork())
                            {
                                DevExpress.Xpo.DB.SelectedData selectedData = uow.ExecuteQuery(detailInsertFullQuery);

                                if (selectedData.ResultSet.Length > 0)
                                {
                                    DevExpress.Xpo.DB.SelectStatementResult result = selectedData.ResultSet[0];

                                    if (result.Rows.Count() > 0)
                                    {
                                        string rValue = result.Rows[0].Values[0].ToString();
                                        int.TryParse(rValue, out shipmentDetailId);
                                    }
                                }
                            }
                            if (shipmentDetailId > 0)
                            {
                                //POShipmentListGridData.Where(c => c.Id == ship.Id).FirstOrDefault().ShipmentDetailId = shipmentDetailId;
                                ship.ShipmentDetailId = shipmentDetailId;
                            }
                            ship.LastShipmentQty = ship.ShipmentQty;
                            //UpdateOpenPOShipmentData(ship);
                        }
                        else
                        {
                            completeQuery += detailInsertFullQuery + @"
";
                        }

                        // EDI QTY Allocation
                        if (ship.AllocatedQuantities != null)
                        {
                            string transactionType = "";

                            string allocQtyUpdateQuery_replace = @"
UPDATE [PIMS].[edi].[EdiTrn]
SET [AllocatedQty] = {0}
    ,[LastModifiedOn] = GETDATE()
WHERE [Editrnid] IN (SELECT trn.[Editrnid]
FROM [PIMS].[edi].[EdiTrn] trn
LEFT JOIN [PIMS].[edi].[EdiHdr] hdr
	ON trn.[Edihdrid] = hdr.[Edihdrid]
WHERE hdr.[Edihdrid] = {1} AND trn.[EdiTrnId] = {2})

UPDATE [PIMS].[dbo].[SODetail]
SET [AllocatedQty] = {0}
    ,[LastModifiedOn] = GETDATE()
WHERE [SODetailId] = {3}
";
                            string allocQtyUpdateQuery_adjust = @"
UPDATE [PIMS].[edi].[EdiTrn]
SET [AllocatedQty] = ISNULL([AllocatedQty], 0) + {0}
    ,[LastModifiedOn] = GETDATE()
WHERE [Editrnid] IN (SELECT trn.[Editrnid]
FROM [PIMS].[edi].[EdiTrn] trn
LEFT JOIN [PIMS].[edi].[EdiHdr] hdr
	ON trn.[Edihdrid] = hdr.[Edihdrid]
WHERE hdr.[Edihdrid] = {1} AND trn.[EdiTrnId] = {2})

UPDATE [PIMS].[dbo].[SODetail]
SET [AllocatedQty] = (SELECT [AllocatedQty] FROM [PIMS].[edi].[EdiTrn] WHERE [Edihdrid] = {1} AND [EdiTrnId] = {2})
    ,[LastModifiedOn] = GETDATE()
WHERE [SODetailId] = {3}
";
                            string allocQtyUpdateFullQuery = "";

                            foreach (SoEdiData alloc in ship.AllocatedQuantities)
                            {
                                transactionType = alloc.TransactionType;
                                // "Multi": On "New" or "Edit": Replace
                                // "Single": On "New": Add To Existing. On "Edit" Adjust with difference

                                int soHeaderId = alloc.SOHeaderId;
                                int soDetailId = alloc.SoDetailId;
                                int ediHdrId = alloc.EdiHdrId;
                                int ediTrnId = alloc.EdiTrnId;
                                int inTransitUnits = alloc.IntransitUnits;  // Original Qty
                                int allocQty = alloc.AllocatedQty;          // Adjusted Qty
                                int adjustQty = allocQty - inTransitUnits;
                                //int productId = alloc.ProductId;
                                //string itemNo = alloc.ItemNo;
                                //allocQtyUpdateFullQuery += string.Format(allocQtyUpdateQuery_replace, allocQty, ediHdrId, ediTrnId);   
                                if (transactionType == "Multi")
                                    allocQtyUpdateFullQuery += string.Format(allocQtyUpdateQuery_replace, allocQty, ediHdrId, ediTrnId, soDetailId);
                                else
                                    allocQtyUpdateFullQuery += string.Format(allocQtyUpdateQuery_adjust, adjustQty, ediHdrId, ediTrnId, soDetailId); 
                            }

                            try
                            {
                                if (!onePushProcess)
                                {
                                    using (var uow = new UnitOfWork())
                                    {
                                        await uow.ExecuteNonQueryAsync(allocQtyUpdateFullQuery);
                                    }
                                }
                                else
                                {
                                    completeQuery += allocQtyUpdateFullQuery + @"
";
                                }
                            }
                            catch (Exception ex)
                            {
                                DisplayPopupMessage("Error (Update) AllocatedQuatities:" + ex.Message + ". AllocQtyDetailUpdateQuery:" + allocQtyUpdateFullQuery);
                            }
                        }
                    }
                    else // adjust existing
                    {
                        if (ship.ShipmentQty != ship.LastShipmentQty || alwaysUpdate)
                        {
                            detailUpdateCount++;

                            string detailUpdateQuery = @"
UPDATE [PIMS].[dbo].[ShipmentDetails] 
SET [ShipmentQty] = {2}
    , [LastModifiedOn] = GETDATE() 
WHERE [ShipmentDetailId] = {0}

UPDATE [PIMS].[dbo].[PODetail] 
SET [LastModifiedOn] = GETDATE() 
    , [ReceivedQty] = (SELECT SUM([ShipmentQty]) FROM [PIMS].[dbo].[ShipmentDetails] WHERE [PODetailId] = {1})
WHERE [PODetailId] = {1}
";
                            string detailUpdateFullQuery = string.Format(detailUpdateQuery, ship.ShipmentDetailId, ship.PODetailId, ship.ShipmentQty);

                            if (!onePushProcess)
                            {
                                using (var uow = new UnitOfWork())
                                {
                                    await uow.ExecuteNonQueryAsync(detailUpdateFullQuery);
                                }
                                ship.LastShipmentQty = ship.ShipmentQty;
                                //UpdateOpenPOShipmentData(ship);
                            }
                            else
                            {
                                completeQuery += detailUpdateFullQuery + @"
";
                            }

                            // EDI QTY Allocation Allocated Qty
                            if (ship.AllocatedQuantities != null)
                            {
                                string transactionType = "";

                                string allocQtyUpdateQuery_replace = @"
UPDATE [PIMS].[edi].[EdiTrn]
SET [AllocatedQty] = {0}
    ,[LastModifiedOn] = GETDATE()
WHERE [Editrnid] IN (SELECT trn.[Editrnid]
FROM [PIMS].[edi].[EdiTrn] trn
LEFT JOIN [PIMS].[edi].[EdiHdr] hdr
	ON trn.[Edihdrid] = hdr.[Edihdrid]
WHERE hdr.[Edihdrid] = {1} AND trn.[EdiTrnId] = {2})

UPDATE [PIMS].[dbo].[SODetail]
SET [AllocatedQty] = {0}
    ,[LastModifiedOn] = GETDATE()
WHERE [SODetailId] = {3}
";
                                string allocQtyUpdateQuery_adjust = @"
UPDATE [PIMS].[edi].[EdiTrn]
SET [AllocatedQty] = ISNULL([AllocatedQty], 0) + {0}
    ,[LastModifiedOn] = GETDATE()
WHERE [Editrnid] IN (SELECT trn.[Editrnid]
FROM [PIMS].[edi].[EdiTrn] trn
LEFT JOIN [PIMS].[edi].[EdiHdr] hdr
	ON trn.[Edihdrid] = hdr.[Edihdrid]
WHERE hdr.[Edihdrid] = {1} AND trn.[EdiTrnId] = {2})

UPDATE [PIMS].[dbo].[SODetail]
SET [AllocatedQty] = (SELECT [AllocatedQty] FROM [PIMS].[edi].[EdiTrn] WHERE [Edihdrid] = {1} AND [EdiTrnId] = {2})
    ,[LastModifiedOn] = GETDATE()
WHERE [SODetailId] = {3}
";
                                string allocQtyUpdateFullQuery = "";

                                foreach (SoEdiData alloc in ship.AllocatedQuantities)
                                {
                                    transactionType = alloc.TransactionType;
                                    // "Multi": On "New" or "Edit": Replace
                                    // "Single": On "New": Add To Existing. On "Edit" Adjust with difference

                                    int soHeaderId = alloc.SOHeaderId;
                                    int soDetailId = alloc.SoDetailId;
                                    int ediHdrId = alloc.EdiHdrId;
                                    int ediTrnId = alloc.EdiTrnId;
                                    int inTransitUnits = alloc.IntransitUnits;  // Original Qty
                                    int allocQty = alloc.AllocatedQty;          // Adjusted Qty
                                    int adjustQty = allocQty - inTransitUnits;
                                    //int productId = alloc.ProductId;
                                    //string itemNo = alloc.ItemNo;
                                    if (transactionType == "Multi")
                                        allocQtyUpdateFullQuery += string.Format(allocQtyUpdateQuery_replace, allocQty, ediHdrId, ediTrnId, soDetailId);
                                    else
                                        allocQtyUpdateFullQuery += string.Format(allocQtyUpdateQuery_adjust, adjustQty, ediHdrId, ediTrnId, soDetailId); 
                                }

                                try
                                {
                                    if (!onePushProcess)
                                    {
                                        using (var uow = new UnitOfWork())
                                        {
                                            await uow.ExecuteNonQueryAsync(allocQtyUpdateFullQuery);
                                        }
                                    }
                                    else
                                    {
                                        completeQuery += allocQtyUpdateFullQuery + @"
";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    DisplayPopupMessage("Error (Update) AllocatedQuantities:" + ex.Message + ". AllocQtyDetailUpdateQuery:" + allocQtyUpdateFullQuery);
                                }
                            }
                        }
                    }
                }
            }

            if (onePushProcess && !string.IsNullOrWhiteSpace(completeQuery))
            {
                try
                {
                    using (var uow = new UnitOfWork())
                    {
                        await uow.ExecuteNonQueryAsync(completeQuery);
                    }
                }
                catch (Exception ex)
                {
                    DisplayPopupMessage("Error (OnePush Update) AllocatedQuantities:" + ex.Message + ". AllocQtyDetailUpdateQuery:" + completeQuery);
                }
            }

            if (detailSaveCount == 0 && detailUpdateCount == 0)
            {
                string message = "No data saved." + Environment.NewLine + "Only 0 quantities.";
                DisplayPopupMessage(message);
            }

            headerMessage = "Refreshing Data...";
            SqlData sqlData = new SqlData();
            GridData = await sqlData.GetShippingDetailData();
            OpenPOShipmentData = await sqlData.GetOpenPOShipmentData();

            await InvokeAsync(StateHasChanged); // <-- refreshes
            headerMessage = "";
        }

        public void Grid_OnRowClick(GridRowClickEventArgs e)
        {
            //GridCell cell = clickedCells.Find(x => x.colName == (e.Column as DxGridDataColumn).FieldName && x.rowIndex == e.VisibleIndex);
            //if (cell == null)
            //    clickedCells.Add(new GridCell((e.Column as DxGridDataColumn).FieldName, e.VisibleIndex));
            //else
            //    clickedCells.Remove(cell);
        }

        #endregion

        // ============================================================ \\

        #region Template POList Grid Functions

        public void POListGrid_CustomizeElement(GridCustomizeElementEventArgs e)
        {
            if (e.ElementType == GridElementType.DataCell && (e.Column as DevExpress.Blazor.DxGridDataColumn).FieldName == "ShipmentQty")
            {
                e.Style = "font-weight: 800";
                e.Style = "color: red";
            }
        }

        public async void POListGrid_OnRowClick(GridRowClickEventArgs e)
        {
            if (POListGrid != null)
            {
                if (POListGrid.GetDataItem(e.VisibleIndex) is ShippingData)
                {
                    ShippingData shippingData = (POListGrid.GetDataItem(e.VisibleIndex) as ShippingData);

                    POHeaderIdSelected = shippingData.POHeaderId;
                    ShipmentHeaderIdSelected = shippingData.ShipmentHeaderId;

                    if (ShipmentHeaderIdSelected == 0)
                        await LoadItemGridDataByPO(shippingData.PONumber);
                    else
                        await LoadItemGridDataByShipmentHeaderId(ShipmentHeaderIdSelected);
                }

                int incomingUnitsSum = (POShipmentListGridData.Count() == 0) ? 0 : POShipmentListGridData.Select(x => x.LastShipmentQty).Sum();
                ShippingData shipingData = POListGridData.Where(x => x.POHeaderId == POHeaderIdSelected).FirstOrDefault();
                if (shipingData != null)
                {
                    shipingData.TotalSum = incomingUnitsSum;
                }
            }
        }

        public async Task POListGrid_OnRowDoubleClick(GridRowClickEventArgs e)
        {
            //await e.Grid.SaveChangesAsync();
            //await e.Grid.StartEditRowAsync(e.VisibleIndex);
        }

        public async Task POListGrid_DataItemDeleting(GridDataItemDeletingEventArgs e)
        {
            if (e.DataItem.GetType() == typeof(ShippingData))
            {
                var deletingObject = (ShippingData)e.DataItem;

                //// Delete from POListGridData
                POListGridData = POListGridData.Where(c => c.POHeaderId != deletingObject.POHeaderId).ToList();

                // Delete from POShipmentListGridData
                POShipmentListGridData = POShipmentListGridData.Where(c => c.POHeaderId != deletingObject.POHeaderId).ToList();
            }
        }

        #endregion

        // ============================================================ \\

        #region Template PO Items Grid Functions

        public void ItemGrid_CustomizeElement(GridCustomizeElementEventArgs e)
        {
            //if (e.ElementType == GridElementType.DataCell && (e.Column as DevExpress.Blazor.DxGridDataColumn).FieldName == "ShipmentQty")
            //{
            //    //int qty = (int)e.Grid.GetRowValue(e.VisibleIndex, "ShipmentQty");
            //    e.Style = "font-weight: 800; color: red";
            //}
            //if (e.ElementType == GridElementType.EditCell && (e.Column as DevExpress.Blazor.DxGridDataColumn).FieldName == "ShipmentQty")
            //{
            //    //int qty = (int)e.Grid.GetRowValue(e.VisibleIndex, "ShipmentQty");
            //    e.Style = "font-weight: 800; color: red";
            //}

            if (e.ElementType == GridElementType.DataCell && (e.Column as DevExpress.Blazor.DxGridDataColumn).FieldName == "ShipmentQty")
            {
                //GridCell cell = clickedCells.Find(x => x.colName == (e.Column as DxGridDataColumn).FieldName && x.rowIndex == e.VisibleIndex);
                //if (cell != null)
                //{
                //    //e.CssClass = "highlighted-item";
                //    e.Style = "border: 2px solid #4864a9; color: red";
                //}
                //else
                //{
                //    e.Style = "color: red";
                //}
                e.Style = "border: 2px solid #a5bdfa; color: red";
            }
            if (e.ElementType == GridElementType.EditCell && (e.Column as DevExpress.Blazor.DxGridDataColumn).FieldName == "ShipmentQty")
            {
                GridCell cell = clickedCells.Find(x => x.colName == (e.Column as DxGridDataColumn).FieldName && x.rowIndex == e.VisibleIndex);
                if (cell != null)
                {
                    //e.CssClass = "highlighted-item";
                    e.Style = "border: 2px solid #a5bdfa; color: red";
                }
                else
                {
                    e.Style = "color: red";
                }
            }

            //if (e.ElementType != GridElementType.DataCell) return;
            //GridCell cell = clickedCells.Find(x => x.colName == (e.Column as DxGridDataColumn).FieldName && x.rowIndex == e.VisibleIndex);
            //if (cell != null)
            //{
            //    //e.CssClass = "highlighted-item";
            //    e.Style = "border: 2px solid #4864a9; font-weight: 800; color: red;";
            //}
            ////else
            ////    e.CssClass = string.Empty;
        }

        public void ItemGrid_OnRowClick(GridRowClickEventArgs e)
        {
            ////var args = e.Grid.Data.GetType().GetGenericArguments();
            ////if ((args.Count() == 1 && args.Single() == typeof(ShippingData)) ||
            ////        (args.Count() > 1 && args[0] == typeof(ShippingData)))
            ////{ }

            //GridCell cell = clickedCells.Find(x => x.colName == (e.Column as DxGridDataColumn).FieldName && x.rowIndex == e.VisibleIndex);
            //if (cell == null)
            //{
            //    if ((e.Column as DxGridDataColumn).FieldName == "ShipmentQty")
            //    {
            //        clickedCells.Add(new GridCell((e.Column as DxGridDataColumn).FieldName, e.VisibleIndex));
            //    }
            //    else
            //        clickedCells.Remove(cell);
            //}
            //else
            //    clickedCells.Remove(cell);

            GridCell cell = clickedCells.Find(x => x.colName == (e.Column as DxGridDataColumn).FieldName && x.rowIndex == e.VisibleIndex);
            if (cell != null)
            {
                if ((e.Column as DxGridDataColumn).FieldName == "ShipmentQty")
                {
                    clickedCells = new List<GridCell>();
                    clickedCells.Add(new GridCell((e.Column as DxGridDataColumn).FieldName, e.VisibleIndex));
                }              
            }            
        }

        public async Task ItemGrid_OnRowDoubleClick(GridRowClickEventArgs e)
        {
            ////await e.Grid.SaveChangesAsync();
            ////FocusedColumn = (e.Column as DxGridDataColumn).FieldName;
            //await e.Grid.StartEditRowAsync(e.VisibleIndex); 
        }

        public void OnSelectedDataItemsChanged(IReadOnlyList<object> selectedDataItems)
        {
            SelectedDataItems = selectedDataItems;
            //onTicketSelectionChanged.InvokeAsync(selectedDataItems);

            foreach (ShippingData ship in selectedDataItems.Cast<ShippingData>())
            {
                if (ship.ShipmentQty == 0)
                {
                    //ship.ShipmentQty = ship.OrderQty;
                    ship.ShipmentQty = ship.RemQty;
                }
            }

            int incomingSelectedUnitsSum = (selectedDataItems.Count() == 0) ? 0 : selectedDataItems.Cast<ShippingData>().Select(x => x.ShipmentQty).Sum();
            int incomingTotalUnitsSum = (POShipmentListGridData.Count() == 0) ? 0 : POShipmentListGridData.Select(x => x.ShipmentQty).Sum();
            ShippingData shipingData = POListGridData.Where(x => x.POHeaderId == POHeaderIdSelected).FirstOrDefault();
            if (shipingData != null)
            {
                shipingData.TotalSum = incomingTotalUnitsSum;
            }
        }

        public void ItemGrid_FilterCriteriaChanged(GridFilterCriteriaChangedEventArgs args)
        {
            //CriteriaOperator newCriteria = args.FilterCriteria;
            // ...
        }

        public void ItemGrid_CustomizeEditModel(GridCustomizeEditModelEventArgs e)
        {
            if (e.EditModel.GetType() == typeof(ShippingData))
            {
                var editObject = (ShippingData)e.EditModel;

                if (e.IsNew)
                {
                    editObject.isNew = true;
                }
                else
                {
                    if (editObject.ShipmentDetailId == 0)
                        editObject.isNew = true;
                    else
                        editObject.isUpdate = true;
                }
            }
        }

        public async Task ItemGrid_OnEditStart(GridEditStartEventArgs e)
        {
            //await e.Grid.SaveChangesAsync();
            if (selectCheckboxToEdit)
            {
                if (!e.IsNew && !e.Grid.IsDataItemSelected(e.DataItem))
                    e.Cancel = true;
            }
            else
            {
            }
        }

        // End edit
        public async Task ItemGrid_OnEditEnd(ShippingData savingObject) 
        {
            ShippingProductSelectedDataItem = savingObject;
            bool isNew = savingObject.isNew;

            if (savingObject.isUpdate || savingObject.ShipmentDetailId > 0)
            {
                if (savingObject.ShipmentQty > savingObject.LastShipmentQty)
                    savingObject.ShipmentQty = savingObject.LastShipmentQty;
            }
            else
            {
                if (savingObject.ShipmentQty > savingObject.RemQty)
                    savingObject.ShipmentQty = savingObject.RemQty;
            }

            // check for EDI PO distribution (Only if more than on factory PO)
            string poNumber = savingObject.PONumber;
            int productId = savingObject.ProductId;
            string productNo = savingObject.ProductNo;
            //string productName = savingObject.ProductName; // Product Description
            //string forProductNo = savingObject.ForProductNo;

            if (!string.IsNullOrEmpty(poNumber) && 
                !string.IsNullOrEmpty(productNo) && 
                savingObject.ShipmentQty > 0)
            {
                SqlData sqlData = new SqlData();
                List<SoEdiData> productFactoryPOsData = await sqlData.GetProductFactoryPOs(poNumber,productNo);

                int shipmentQty = savingObject.ShipmentQty;
                int inTransitTotal = 0;

                foreach (SoEdiData item in productFactoryPOsData)
                {
                    item.ShipmentQty = shipmentQty;
                    inTransitTotal = item.InTransitTotal;

                    if (productFactoryPOsData.Count == 1)
                    {
                        item.AllocatedQty = shipmentQty;
                        item.TransactionType = "Single"; // On "New": Add To Existing. On "Edit" Adjust with difference
                    }
                    else
                    {
                        item.TransactionType = "Multi"; // On "New" or "Edit": Replace
                    }
                }
                savingObject.AllocatedQuantities = productFactoryPOsData;

                savingObject.POCount = productFactoryPOsData.Count;

                //if (productFactoryPOsData.Count > 0) // 0 for testing. 1 for normal
                if (productFactoryPOsData.Count > 1) // 0 for testing. 1 for normal
                {
                    //productFactoryPOsGridPopupTitleText = "Product No " + productNo + " [Incoming Units = " + shipmentQty + "]";
                    //productFactoryPOsGridPopupBodyText = shipmentQty.ToString(); //"Available Qty: " + shipmentQty;
                    if (isNew)
                    {
                        productFactoryPOsGridPopupTitleText = "Product No " + productNo + " [Incoming Units = " + inTransitTotal + " (+" + shipmentQty + ") " + "]";
                        productFactoryPOsGridPopupBodyText = (inTransitTotal + shipmentQty).ToString(); 
                    }
                    else
                    {
                        productFactoryPOsGridPopupTitleText = "Product No " + productNo + " [Incoming Units = " + inTransitTotal + "]";
                        productFactoryPOsGridPopupBodyText = inTransitTotal.ToString();
                    }

                    ProductFactoryPOsGridData = productFactoryPOsData;
                    productFactoryPOsGridDataPopupVisible = true;
                    productFactoryPOsOkButtonEnabled = false;

                    await InvokeAsync(StateHasChanged); // <-- refreshes
                }
                else
                {
                    int index = POShipmentListGrid.GetFocusedRowIndex();

                    if (index >= 0)
                    {
                        (POShipmentListGrid.GetDataItem(index) as ShippingData).AllocatedQuantities = productFactoryPOsData.ToList();
                    }
                }
            }
        }

        public async Task ItemGrid_EditModelSaving(GridEditModelSavingEventArgs e)
        {
            if (e.EditModel.GetType() == typeof(ShippingData))
            {
                var savingObject = (ShippingData)e.EditModel;

                savingObject.isDirty = true;
                if (savingObject.ShipmentDetailId == 0)
                    savingObject.isNew = true;

                if (e.IsNew)
                {
                    savingObject.isNew = true;
                }
                else
                {
                    savingObject.isUpdate = true;
                }

                //if (savingObject.ShipmentQty > savingObject.RemQty)
                //    savingObject.ShipmentQty = savingObject.RemQty;
                //
                //// check for EDI PO distribution (Only if more than on factory PO)
                //int productId = savingObject.ProductId;
                //string productNo = savingObject.ProductNo;
                ////string productName = savingObject.ProductName; // Product Description
                ////string forProductNo = savingObject.ForProductNo;
                //
                //if (!string.IsNullOrEmpty(productNo) && savingObject.ShipmentQty > 0)
                //{
                //    SqlData sqlData = new SqlData();
                //    List<SoEdiData> productFactoryPOsData = await sqlData.GetProductFactoryPOs(productNo);
                //
                //    int shipmentQty = savingObject.ShipmentQty;
                //
                //    foreach (SoEdiData item in productFactoryPOsData)
                //    {
                //        item.ShipmentQty = shipmentQty;
                //    }
                //    savingObject.AllocatedQuatities = productFactoryPOsData;
                //
                //    if (productFactoryPOsData.Count > 0) // 0 for testing. 1 for normal
                //    {
                //        productFactoryPOsGridPopupTitleText = "Product No " + productNo + " [" + shipmentQty + "]";
                //        productFactoryPOsGridPopupBodyText = "Available Qty: " + shipmentQty;
                //
                //        ProductFactoryPOsGridData = productFactoryPOsData;
                //        productFactoryPOsGridDataPopupVisible = true;
                //    }
                //}

                POShipmentListGridData.Where(c => c.Id == savingObject.Id).FirstOrDefault().ShipmentQty = savingObject.ShipmentQty;

                int incomingUnitsSum = (POShipmentListGridData.Count() == 0) ? 0 : POShipmentListGridData.Select(x => x.ShipmentQty).Sum();
                ShippingData shipingData = POListGridData.Where(x => x.POHeaderId == POHeaderIdSelected).FirstOrDefault();
                if (shipingData != null)
                {
                    shipingData.TotalSum = incomingUnitsSum;
                }
            }
        }

        public void ItemGrid_OnFocusedRowChanged(GridFocusedRowChangedEventArgs e)
        {
            var shippingData = e.DataItem as ShippingData;

        }
        
        public void ItemGrid_OnKeyDownGrid(KeyboardEventArgs e) // <DxGrid @ref="POShipmentListGrid" ... @onkeydown="ItemGrid_OnKeyDownGrid"
        {
            List<string> keyList = new List<string>() { "ArrowRight", "Enter", "Tab" }; //"ArrowLeft"

            if (keyList.Contains(e.Key))
            {
                //// Get next FocusRow
                //int index = POShipmentListGrid.GetFocusedRowIndex();
                //if (index > POShipmentListGrid.GetVisibleRowCount())
                //    index = 1;
                //else
                //    index++;

                //POShipmentListGrid.SetFocusedRowIndex(index);

                ////POShipmentListGrid.CancelEditAsync();

                //js.InvokeVoidAsync("focusEditor");
            }

            //if (e.Key != "Enter")
            //    return;

            //if (ProductFactoryPOsGrid.GetVisibleRowCount() != 1)
            //    return;
            //var item = ProductFactoryPOsGrid.GetDataItem(0) as ShippingData;
            //if (item != null)
            //{
            //    //AddSelected(item);
            //}
        }

        public void HandleKeyDown(KeyboardEventArgs e) // <div tabindex="0" @onkeydown="HandleKeyDown">
        {
            List<string> keyList = new List<string>() { "ArrowRight", "Enter", "Tab" }; //"ArrowLeft"

            if (keyList.Contains(e.Key))
            {
                //// Get next FocusRow
                //int index = POShipmentListGrid.GetFocusedRowIndex();
                //if (index > POShipmentListGrid.GetVisibleRowCount())
                //    index = 1;
                //else
                //    index++;

                //POShipmentListGrid.SetFocusedRowIndex(index);
            }
        }

        public void GridCell_OnKeyDownGrid(KeyboardEventArgs e)
        {
            List<string> keyList = new List<string>() { "ArrowRight", "Enter", "Tab" };

            if (keyList.Contains(e.Key))
            {
                POShipmentListGrid.CancelEditAsync();

                //// Get next FocusRow
                //int index = POShipmentListGrid.GetFocusedRowIndex();
                //if (index > POShipmentListGrid.GetVisibleRowCount())
                //    index = 1;
                //else
                //    index++;

                //POShipmentListGrid.SetFocusedRowIndex(index);
            }
        }

        // ============================================================ \\

        public void ProductFactoryPOsGrid_CustomizeElement(GridCustomizeElementEventArgs e)
        {
            if (e.ElementType == GridElementType.DataCell && (e.Column as DevExpress.Blazor.DxGridDataColumn).FieldName == "AllocatedQty")
            {
                e.Style = "border: 2px solid #4864a9; color: red";
            }
            if (e.ElementType == GridElementType.EditCell && (e.Column as DevExpress.Blazor.DxGridDataColumn).FieldName == "AllocatedQty")
            {
                GridCell cell = clickedCells.Find(x => x.colName == (e.Column as DxGridDataColumn).FieldName && x.rowIndex == e.VisibleIndex);
                if (cell != null)
                {
                    e.Style = "border: 2px solid #4864a9; color: red";
                }
                else
                {
                    e.Style = "color: red";
                }
            }
        }

        public async Task ProductFactoryPOsGrid_OnEditEnd_old(SoEdiData productFactoryPOsGridData) // End edit
        {
            if (productFactoryPOsGridData != null)
            {
                // Test for qty 
                string editItemNo = productFactoryPOsGridData.ItemNo;
                string poNumber = productFactoryPOsGridData.CustomerPO; //productFactoryPOsGridData.PONumber;

                int allocQty = productFactoryPOsGridData.AllocatedQty;
                int totalShipmentQty = ProductFactoryPOsGridData.ToList()[0].ShipmentQty;
                int subTotal = 0;
                int remQty = 0;

                // Qty cannot be greater than OrderQty (+ any previous received qty)
                if (productFactoryPOsGridData.AllocatedQty > (productFactoryPOsGridData.OrderQty - productFactoryPOsGridData.IntransitUnits))
                {
                    productFactoryPOsGridData.AllocatedQty = (productFactoryPOsGridData.OrderQty - productFactoryPOsGridData.IntransitUnits);
                }

                foreach (SoEdiData item in ProductFactoryPOsGridData)
                {
                    if (item.CustomerPO != poNumber)
                        subTotal += item.AllocatedQty;
                }

                if ((subTotal + allocQty) > totalShipmentQty)
                {
                    productFactoryPOsGridData.AllocatedQty = (totalShipmentQty - subTotal);
                }

                remQty = (totalShipmentQty - (subTotal + productFactoryPOsGridData.AllocatedQty));
                productFactoryPOsGridPopupBodyText = remQty.ToString();
                productFactoryPOsOkButtonEnabled = (remQty == 0) ? true : false;

                foreach (SoEdiData item in ProductFactoryPOsGridData)
                {
                    if (item.CustomerPO == poNumber)
                    {
                        item.AllocatedQty = productFactoryPOsGridData.AllocatedQty;
                        break;
                    }
                }
            }
        }
        public async Task ProductFactoryPOsGrid_OnEditEnd(SoEdiData productFactoryPOsGridData) // End edit
        {
            if (productFactoryPOsGridData != null)
            {
                bool isNew = ShippingProductSelectedDataItem.isNew;

                // Test for qty 
                string editItemNo = productFactoryPOsGridData.ItemNo;
                string poNumber = productFactoryPOsGridData.CustomerPO; //productFactoryPOsGridData.PONumber;
                int edihdrid = productFactoryPOsGridData.EdiHdrId;
                int editrnid = productFactoryPOsGridData.EdiTrnId;

                int allocQty = productFactoryPOsGridData.AllocatedQty;
                int totalShipmentQty = ProductFactoryPOsGridData.ToList()[0].ShipmentQty;
                int inTransitTotalQty = ProductFactoryPOsGridData.ToList()[0].InTransitTotal;
                int totalAllocatableQty = 0;
                int subTotal = 0;
                int remQty = 0;

                if (isNew)
                    totalAllocatableQty = inTransitTotalQty + totalShipmentQty;
                else
                    totalAllocatableQty = inTransitTotalQty;

                // Qty cannot be greater than OrderQty (+ any previous received qty)
                if (productFactoryPOsGridData.AllocatedQty > productFactoryPOsGridData.OrderQty)
                {
                    productFactoryPOsGridData.AllocatedQty = productFactoryPOsGridData.OrderQty;
                    allocQty = productFactoryPOsGridData.AllocatedQty;
                }

                foreach (SoEdiData item in ProductFactoryPOsGridData)
                {
                    if (item.CustomerPO == poNumber && item.EdiHdrId == edihdrid && item.EdiTrnId == editrnid)
                    {
                    }
                    else
                        subTotal += item.AllocatedQty;
                }

                if ((subTotal + allocQty) > totalAllocatableQty)
                {
                    productFactoryPOsGridData.AllocatedQty = (totalAllocatableQty - subTotal);
                }

                remQty = totalAllocatableQty - productFactoryPOsGridData.AllocatedQty - subTotal;
                productFactoryPOsGridPopupBodyText = remQty.ToString();
                productFactoryPOsOkButtonEnabled = (remQty == 0) ? true : false;

                foreach (SoEdiData item in ProductFactoryPOsGridData)
                {
                    if (item.CustomerPO == poNumber && item.EdiHdrId == edihdrid && item.EdiTrnId == editrnid)
                    {
                        item.AllocatedQty = productFactoryPOsGridData.AllocatedQty;
                        break;
                    }
                }
            }
        }

        public async Task POShipmentListGridRowFocus(int index) // End edit
        {
            //POShipmentListGrid.SetFocusedRowIndex(index);

            ShippingData fi = POShipmentListGrid.GetDataItem(index) as ShippingData;
            //await POShipmentListGrid.SetFocusedDataItemAsync(fi);

            //SelectedDataItem = fi;
            //await InvokeAsync(StateHasChanged);
        }

        public async void OkProductFactoryPOsPopupClick()
        {
            productFactoryPOsGridDataPopupVisible = false;

            int index = POShipmentListGrid.GetFocusedRowIndex();

            if (index >= 0)
            {
                (POShipmentListGrid.GetDataItem(index) as ShippingData).AllocatedQuantities = ProductFactoryPOsGridData.ToList();

                (POShipmentListGrid.GetDataItem(index) as ShippingData).POCount = (POShipmentListGrid.GetDataItem(index) as ShippingData).AllocatedQuantities.Count;
            }

            await ProductFactoryPOsGrid.CancelEditAsync();

            // Use ProductFactoryPOsGridData to update EdiTrn data

        }

        public void CancelProductFactoryPOsPopupClick()
        {
            productFactoryPOsGridDataPopupVisible = false;

            int index = POShipmentListGrid.GetFocusedRowIndex();

            if (index >= 0)
            {
                try
                {
                    (POShipmentListGrid.GetDataItem(index) as ShippingData).ShipmentQty = (POShipmentListGrid.GetDataItem(index) as ShippingData).LastShipmentQty;
                    (POShipmentListGrid.GetDataItem(index) as ShippingData).AllocatedQuantities = null;
                }
                catch
                {

                }
            }
        }

        #endregion

        // ============================================================ \\

        public async Task OnButtonClick()
        {
            try
            {
                var dim = await js.InvokeAsync<BrowserDimension>("getDimensions");
                Height = dim.Height;
                Width = dim.Width;

                DisplayPopupMessage("Screen H" + dim.Height + " W" + dim.Width);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task<BrowserDimension> GetDimensions()
        {
            try
            {
                return await js.InvokeAsync<BrowserDimension>("getDimensions");
            }
            catch (Exception ex)
            {
                return new BrowserDimension();
            }
        }

        // ============================================================ \\

        #region Popup Functions

        public async Task OnPopupDragCompleted(PopupDragCompletedEventArgs args)
        {
            (positionX, positionY) = (args.End.X, args.End.Y);
            await SavePositionToLocalStorageAsync(args.End);
        }
        // Refer to https://docs.microsoft.com/en-us/aspnet/core/blazor/state-management
        // to learn more about Blazor state management
        // In Blazor Server apps, prefer ASP.NET Core Protected Browser Storage
        async Task<Point?> LoadPositionFromLocalStorageAsync()
        {
            //var json = await JSRuntime.InvokeAsync<string>("localStorage.getItem", LocalStorageKey);
            //return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<Point>(json);
            return null;
        }
        async Task SavePositionToLocalStorageAsync(Point position)
        {
            //await JSRuntime.InvokeVoidAsync("localStorage.setItem", LocalStorageKey, JsonSerializer.Serialize(position));
        }
        async Task RemovePositionFromLocalStorageAsync()
        {
            //await JSRuntime.InvokeVoidAsync("localStorage.removeItem", LocalStorageKey);
        }
        async Task ReloadPageButton_ClickAsync()
        {
            //await JSRuntime.InvokeVoidAsync("location.reload");
        }
        async Task ResetPositionButton_ClickAsync()
        {
            //await RemovePositionFromLocalStorageAsync();
            //await JSRuntime.InvokeVoidAsync("location.reload");
        }

        #endregion

        // ============================================================ \\
    }
}