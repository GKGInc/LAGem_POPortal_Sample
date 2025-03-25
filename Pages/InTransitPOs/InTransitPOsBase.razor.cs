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
        public GridEditMode CurrentEditMode { get { return usePopupEditForm ? GridEditMode.PopupEditForm : GridEditMode.EditForm; } } // GridEditMode.EditRow
        public string mainGridEditFormHeaderText { get; set; } = "In Transit POs";

        public IReadOnlyList<object> SelectedDataItems { get; set; } // Items Selected in Edit Form
        public IReadOnlyList<object> SelectedDataPOListGridItems { get; set; }
        public IEnumerable<GridSelectAllCheckboxMode> SelectAllCheckboxModes { get; } = Enum.GetValues<GridSelectAllCheckboxMode>();
        public GridSelectAllCheckboxMode CurrentSelectAllCheckboxMode { get; set; }

        // ------------------------------------------------------------ \\

        public bool PopupVisible { get; set; } = false;
        public const string LocalStorageKey = "DialogsAndWindows-Popup-Dragging";
        public int? positionX, positionY;
        public bool allowDragByHeaderOnly = true;

        public string popupOkButtonText { get; set; } = "Ok";
        public string popupCancelButtonText { get; set; } = "Cancel";
        public bool isPopupCancelButtonVisible { get; set; } = false;

        public string popupTitleText { get; set; } = "Notification";
        public string popupBodyText { get; set; } = "Sample Popup Message";

        public string callbackProcessName { get; set; } = null;

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
            SqlData sqlData = new SqlData();
            GridData = await sqlData.GetShippingDetailData();
            POOpenVendorData = await sqlData.GetPOOpenVendorData();
            //POOpenDetailData = await sqlData.GetPOOpenDetailData();
            OpenPOShipmentData = await sqlData.GetOpenPOShipmentData();

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

            var dim = await js.InvokeAsync<BrowserDimension>("getDimensions");
            Height = dim.Height;
            Width = dim.Width;
        }

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
                await InsertShippingDataAsync(savingShippingDataObject);

                await Grid.CancelEditAsync();
            }
            if (callbackProcessName == "SaveShippingData-UDPATE")
            {
                await UpdateShippingDataAsync(savingShippingDataObject);

                await Grid.CancelEditAsync();
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

            var polist = OpenPOShipmentData.Where(x => x.VendorId == vendorId).Select(x => x.PONumber).Distinct().ToList();
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

        public void AddToPOListGridData(ShippingData poData)
        {
            if (poData != null)
            {
                bool isAddon = false;
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

        public async Task LoadItemGridData(string poNo)
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
        public async void RefreshData_Click()
        {
            SqlData sqlData = new SqlData();
            GridData = await sqlData.GetShippingDetailData();

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

        public void Grid_CustomizeEditModel(GridCustomizeEditModelEventArgs e)
        {
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
                    LoadItemGridData(((ShippingData)e.EditModel).PONumber);
                    POHeaderIdSelected = ((ShippingData)e.EditModel).POHeaderId;
                    //POListGridData = new List<POOpenDetail>();
                    POListGridData = new List<ShippingData>();
                    POShipmentListGridData = new List<ShippingData>();

                    ShippingData poItem = OpenPOShipmentData.Where(c => c.PONumber == ((ShippingData)e.EditModel).PONumber).FirstOrDefault();
                    AddToPOListGridData(poItem);
                    isVendorEditable = false;
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

        public async Task InsertShippingDataAsync(ShippingData item)
        {
            bool alwaysInsert = true;

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
            if (SelectedDataItems.Count > 0)
                productlist = SelectedDataItems.Select(x => (x as ShippingData).ProductNo).Distinct().ToList();
            List<int> soDetailIds = new List<int>();
            if (SelectedDataItems.Count > 0)
                soDetailIds = SelectedDataItems.Select(x => (x as ShippingData).SODetailId).Distinct().ToList();

            // Save POShipmentListGrid data
            foreach (ShippingData ship in POShipmentListGridData)
            {
                if (soDetailIds.Contains(ship.SODetailId))
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

--UPDATE [PIMS].[dbo].[PODetail] 
--SET [ReceivedQty] = CASE WHEN [ReceivedQty] IS NULL THEN (SELECT SUM([ShipmentQty]) FROM [PIMS].[dbo].[ShipmentDetails] WHERE [PODetailId] = {1}) + {2} ELSE [ReceivedQty] + {2} END 
--WHERE [PODetailId] = {1}

UPDATE [PIMS].[dbo].[PODetail]
SET [ReceivedQty] = (SELECT SUM([ShipmentQty]) FROM [PIMS].[dbo].[ShipmentDetails] WHERE [PODetailId] = {1})
    ,[LastModifiedOn] = GETDATE()
WHERE [PODetailId] = {1}

SELECT @@IDENTITY AS 'pk'";
                        string detailInsertFullQuery = string.Format(detailInsertQuery, shipmentHeaderId, poDetailId, shipmentQty);

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
                        if (ship.OrderQty != ship.ShipmentQty && ship.ShipmentQty != ship.LastShipmentQty)
                        {
                            //                            // UPDATE
                            //                            string detailUpdateQuery = @"
                            //UPDATE [PIMS].[dbo].[ShipmentDetails] 
                            //SET [ShipmentQty] = {1}
                            //--, [LastModifiedOn] = GETDATE() 
                            //WHERE [PODetailId] = {0}";
                            //string detailUpdateFullQuery = string.Format(detailUpdateQuery, item.PODetailId, item.OrderQty);
                            string detailUpdateQuery = @"
--UPDATE [PIMS].[dbo].[PODetail] 
--SET [ReceivedQty] = CASE WHEN [ReceivedQty] IS NULL THEN (SELECT SUM([ShipmentQty]) FROM [PIMS].[dbo].[ShipmentDetails] WHERE [PODetailId] = {1}) + {2} ELSE [ReceivedQty] - (SELECT [ShipmentQty] FROM [PIMS].[dbo].[ShipmentDetails] WHERE [ShipmentDetailId] = {0}) + {2} END 
--WHERE [PODetailId] = {1}

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

                            using (var uow = new UnitOfWork())
                            {
                                await uow.ExecuteNonQueryAsync(detailUpdateFullQuery);
                            }
                            ship.LastShipmentQty = ship.ShipmentQty;
                            //UpdateOpenPOShipmentData(ship);
                        }
                    }
                }
            }

            SqlData sqlData = new SqlData();
            GridData = await sqlData.GetShippingDetailData();

            await InvokeAsync(StateHasChanged); // <-- refreshes
        }
        public async Task UpdateShippingDataAsync(ShippingData item)
        {
            // Don't save if empty detail list
            if (POShipmentListGridData.Count() == 0)
                return;

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

            using (var uow = new UnitOfWork())
            {
                await uow.ExecuteNonQueryAsync(fullQuery);
            }

            // Get list of Rows Selected. See SelectedDataItems list
            List<string> productlist = new List<string>();
            if (SelectedDataItems != null && SelectedDataItems.Count > 0)
                productlist = SelectedDataItems.Select(x => (x as ShippingData).ProductNo).Distinct().ToList();
            List<int> soDetailIds = new List<int>();
            if (SelectedDataItems != null && SelectedDataItems.Count > 0)
                soDetailIds = SelectedDataItems.Select(x => (x as ShippingData).SODetailId).Distinct().ToList();

            // Save POShipmentListGrid data
            foreach (ShippingData ship in POShipmentListGridData)
            {
                if (soDetailIds.Contains(ship.SODetailId))
                {
                    if (ship.ShipmentDetailId == 0 && ship.ShipmentQty != ship.LastShipmentQty)
                    {
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
                        //if (ship.OrderQty != ship.ShipmentQty && ship.ShipmentQty != ship.LastShipmentQty)
                        if (ship.ShipmentQty != ship.LastShipmentQty)
                        {
                            //// UPDATE
                            //string detailUpdateQuery = @"
                            //UPDATE [PIMS].[dbo].[ShipmentDetails] 
                            //SET [ShipmentQty] = {1}
                            //--, [LastModifiedOn] = GETDATE() 
                            //WHERE [PODetailId] = {0}";
                            //string detailUpdateFullQuery = string.Format(detailUpdateQuery, item.PODetailId, item.OrderQty);
                            string detailUpdateQuery = @"
UPDATE [PIMS].[dbo].[ShipmentDetails] 
SET [ShipmentQty] = {2}
    , [LastModifiedOn] = GETDATE() 
WHERE [ShipmentDetailId] = {0}

UPDATE [PIMS].[dbo].[PODetail] 
SET [LastModifiedOn] = GETDATE() 
    --, [ReceivedQty] = CASE WHEN [ReceivedQty] IS NULL THEN (SELECT SUM([ShipmentQty]) FROM [PIMS].[dbo].[ShipmentDetails] WHERE [PODetailId] = {1}) + {2} ELSE [ReceivedQty] - (SELECT [ShipmentQty] FROM [PIMS].[dbo].[ShipmentDetails] WHERE [ShipmentDetailId] = {0}) + {2} END 
    , [ReceivedQty] = (SELECT SUM([ShipmentQty]) FROM [PIMS].[dbo].[ShipmentDetails] WHERE [PODetailId] = {1})
WHERE [PODetailId] = {1}
";
                            string detailUpdateFullQuery = string.Format(detailUpdateQuery, ship.ShipmentDetailId, ship.PODetailId, ship.ShipmentQty);

                            using (var uow = new UnitOfWork())
                            {
                                await uow.ExecuteNonQueryAsync(detailUpdateFullQuery);
                            }
                            ship.LastShipmentQty = ship.ShipmentQty;
                            //UpdateOpenPOShipmentData(ship);
                        }
                    }
                }
            }

            SqlData sqlData = new SqlData();
            GridData = await sqlData.GetShippingDetailData();

            await InvokeAsync(StateHasChanged); // <-- refreshes
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
                    POHeaderIdSelected = (POListGrid.GetDataItem(e.VisibleIndex) as ShippingData).POHeaderId;
                await LoadItemGridData((POListGrid.GetDataItem(e.VisibleIndex) as ShippingData).PONumber);

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
            if (e.ElementType == GridElementType.DataCell && (e.Column as DevExpress.Blazor.DxGridDataColumn).FieldName == "ShipmentQty")
            {
                //decimal qty = (int)e.Grid.GetRowValue(e.VisibleIndex, "OrderQty");
                e.Style = "font-weight: 800";
                e.Style = "color: red";
            }
            if (e.ElementType == GridElementType.EditCell && (e.Column as DevExpress.Blazor.DxGridDataColumn).FieldName == "ShipmentQty")
            {
                //decimal qty = (int)e.Grid.GetRowValue(e.VisibleIndex, "OrderQty");
                e.Style = "font-weight: 800";
                e.Style = "color: red";
            }

            if (e.ElementType != GridElementType.DataCell) return;
            GridCell cell = clickedCells.Find(x => x.colName == (e.Column as DxGridDataColumn).FieldName && x.rowIndex == e.VisibleIndex);
            if (cell != null)
            {
                //e.CssClass = "highlighted-item";
                e.Style = "border: 2px solid #4864a9";
            }
            else
                e.CssClass = string.Empty;
        }

        public void ItemGrid_OnRowClick(GridRowClickEventArgs e)
        {
            //var args = e.Grid.Data.GetType().GetGenericArguments();
            //if ((args.Count() == 1 && args.Single() == typeof(ShippingData)) ||
            //        (args.Count() > 1 && args[0] == typeof(ShippingData)))
            //{ }

            GridCell cell = clickedCells.Find(x => x.colName == (e.Column as DxGridDataColumn).FieldName && x.rowIndex == e.VisibleIndex);
            if (cell == null)
            {
                if ((e.Column as DxGridDataColumn).FieldName == "ShipmentQty")
                {
                    clickedCells.Add(new GridCell((e.Column as DxGridDataColumn).FieldName, e.VisibleIndex));
                }
                else
                    clickedCells.Remove(cell);
            }
            else
                clickedCells.Remove(cell);
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
                    ship.ShipmentQty = ship.OrderQty;
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
                if (editObject.ShipmentDetailId == 0)
                    editObject.isNew = true;

                if (e.IsNew)
                {
                    editObject.isNew = true;
                }
                else
                {
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

                POShipmentListGridData.Where(c => c.Id == savingObject.Id).FirstOrDefault().ShipmentQty = savingObject.ShipmentQty;

                int incomingUnitsSum = (POShipmentListGridData.Count() == 0) ? 0 : POShipmentListGridData.Select(x => x.ShipmentQty).Sum();
                ShippingData shipingData = POListGridData.Where(x => x.POHeaderId == POHeaderIdSelected).FirstOrDefault();
                if (shipingData != null)
                {
                    shipingData.TotalSum = incomingUnitsSum;
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