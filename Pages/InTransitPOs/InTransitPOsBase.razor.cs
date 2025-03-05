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
        private Task<AuthenticationState> authenticationState { get; set; }
        string currentUser { get; set; } = "";

        [CascadingParameter]
        protected Task<AuthenticationState> AuthStat { get; set; }

        IGrid Grid { get; set; }
        IGrid POListGrid { get; set; }
        IGrid POShipmentListGrid { get; set; }
        IGrid TestGrid { get; set; }

        IEnumerable<ShippingData> GridData { get; set; }
        IEnumerable<POOpenVendor> POOpenVendorData { get; set; }
        IEnumerable<ShippingData> OpenPOShipmentData { get; set; }
        IEnumerable<ShippingData> POListGridData { get; set; }
        IEnumerable<ShippingData> POShipmentListGridData { get; set; }
        IEnumerable<ShippingData> TestGridData { get; set; }
        List<Lookup> openPOList { get; set; }

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

        int PageCount { get; set; } = 0;
        int TotalRecords { get; set; } = 0;
        int PageSize { get; set; } = 20;
        int ActivePageIndex { get; set; } = 0;

        Dictionary<string, GridSearchTextParseMode> SearchTextParseModes { get; } = new Dictionary<string, GridSearchTextParseMode>{
        { "Group Words By And", GridSearchTextParseMode.GroupWordsByAnd },
        { "Group Words By Or", GridSearchTextParseMode.GroupWordsByOr },
        { "Exact Match", GridSearchTextParseMode.ExactMatch }
    };
        void ChangeSearchMode(string key)
        {
            CurrentSearchTextParseModeDisplayText = key;
            CurrentSearchTextParseMode = SearchTextParseModes[key];
        }
        string CurrentSearchTextParseModeDisplayText { get; set; } = "Group Words By And";
        GridSearchTextParseMode CurrentSearchTextParseMode { get; set; } = GridSearchTextParseMode.GroupWordsByAnd;

        GridColumnResizeMode CurrentColumnResizeMode { get; set; } = GridColumnResizeMode.ColumnsContainer; // GridColumnResizeMode.NextColumn;
        string CurrentColumnResizeModeDisplayText { get; set; } = "Next Column";
        Dictionary<string, GridColumnResizeMode> GridColumnResizeModes { get; } = new Dictionary<string,
        GridColumnResizeMode>{
        { "Disabled", GridColumnResizeMode.Disabled },                  //A user cannot resize columns.
        { "Next Column", GridColumnResizeMode.NextColumn },             //When a user resizes a column, the width of the column to the right changes, but the Grid's total width does not change.
        { "Columns Container", GridColumnResizeMode.ColumnsContainer }  //When a user resizes a column, all other columns retain width settings, but the width of the entire column container changes proportionally.
    };
        void ChangeResizeMode(string key)
        {
            CurrentColumnResizeModeDisplayText = key;
            CurrentColumnResizeMode = GridColumnResizeModes[key];
        }

        bool AutoCollapseDetailRow { get; set; }
        TaskCompletionSource<bool> DataLoadedTcs { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        bool TextWrapEnabled = true;
        bool WordWrapEnabled = false;

        bool usePopupEditForm { get; set; } = true;
        GridEditMode CurrentEditMode { get { return usePopupEditForm ? GridEditMode.PopupEditForm : GridEditMode.EditForm; } } // GridEditMode.EditRow
        string mainGridEditFormHeaderText { get; set; } = "In Transit POs";

        IReadOnlyList<object> SelectedDataItems { get; set; } // Items Selected in Edit Form
        IReadOnlyList<object> SelectedDataPOListGridItems { get; set; }
        IEnumerable<GridSelectAllCheckboxMode> SelectAllCheckboxModes { get; } = Enum.GetValues<GridSelectAllCheckboxMode>();
        GridSelectAllCheckboxMode CurrentSelectAllCheckboxMode { get; set; }

        // ------------------------------------------------------------ \\

        bool PopupVisible { get; set; } = false;
        const string LocalStorageKey = "DialogsAndWindows-Popup-Dragging";
        int? positionX, positionY;
        bool allowDragByHeaderOnly = true;

        string popupOkButtonText { get; set; } = "Ok";
        string popupCancelButtonText { get; set; } = "Cancel";
        bool isPopupCancelButtonVisible { get; set; } = false;

        string popupTitleText { get; set; } = "Notification";
        string popupBodyText { get; set; } = "Sample Popup Message";

        string callbackProcessName { get; set; } = null;

        // ------------------------------------------------------------ \\

        string headerMessage { get; set; } = "Loading Data...";
        bool hiddenGrid { get; set; } = true;
        bool autoFitColWidths { get; set; } = true;
        bool isAutoFitPending { get; set; } = true;
        bool isVendorEditable { get; set; } = true;
        bool selectCheckboxToEdit { get; set; } = false;
        string FocusedColumn { get; set; } = "OrderQty";
        int POHeaderIdSelected { get; set; }
        public int editPOShipmentListGridHeight { get; set; } = 200;

        CriteriaOperator shippingGridFilterCriteria { get; set; }

        // ------------------------------------------------------------ \\

        public class BrowserDimension
        {
            public int Width { get; set; }
            public int Height { get; set; }
        }
        public int Height { get; set; } = 0;
        public int Width { get; set; } = 0;

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

            var user = (await AuthStat).User;
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
                await js.InvokeVoidAsync("window.registerViewportChangeCallback", DotNetObjectReference.Create(this));

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

            int offset = 75;
            scrollHeight = height - 195 - offset;
            mainGridSectionHeight = (height - offset).ToString() + "px";
        }


        #endregion

        // ============================================================ \\

        #region Popup Functions

        async void OkPopupClick()
        {
            PopupVisible = false;

            //if (callbackProcessName == "SaveLinks")
            //{
            //    //@PopupLinkingFormContext.CloseCallback
            //    windowRefLinking.CloseAsync();
            //
            //    await SaveCustomerPOLinking();
            //}
            //if (callbackProcessName == "UnLinkSave")
            //{
            //    await OnUnLinkPOItemSave(unLinkingRow);
            //}
        }

        void CancelPopupClick()
        {
            PopupVisible = false;
            //linkingEdiPOtoSO = false;
        }

        void DisplayPopupMessage(string message, string title = "Notification", string callbackName = null)
        {
            isPopupCancelButtonVisible = false;
            popupOkButtonText = "Ok";
            popupCancelButtonText = "Cancel";
            popupTitleText = title;
            popupBodyText = message;
            PopupVisible = true;

            callbackProcessName = callbackName;
        }

        void DisplayPopupQuestion(string message, string title = "Alert", string callbackName = null)
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

        void Grid_FitWidths()
        {
            Grid.AutoFitColumnWidths();
        }
        void ColumnChooserButton_Click()
        {
            Grid.ShowColumnChooser();
        }
        async void RefreshData_Click()
        {
            SqlData sqlData = new SqlData();
            GridData = await sqlData.GetShippingDetailData();
        }
        async Task UsePopupEditForm_CheckedChanged(bool value)
        {
            usePopupEditForm = value;
            await Grid.CancelEditAsync();
        }

        #endregion

        // ============================================================ \\

        #region Main Grid Functions

        void Grid_CustomizeElement(GridCustomizeElementEventArgs e)
        {
            //bool isShipping = false;
            //if (Grid.GetDataItem(e.VisibleIndex) is ShippingData)
            //{
            //    isShipping = true;
            //}

            // if (e.ElementType == GridElementType.DataRow && (System.Decimal)e.Grid.GetRowValue(e.VisibleIndex, "Total") > 1000)
            // {
            //     e.CssClass = "highlighted-item";
            // }

            if (e.ElementType == GridElementType.DataCell && (e.Column as DevExpress.Blazor.DxGridDataColumn).FieldName == "CustomerName")
            {
                //string customerName = (string)e.Grid.GetRowValue(e.VisibleIndex, "CustomerName");
                //if (customerName == "MACYS")
                //    e.Style = "background: green";
            }
            if (e.ElementType == GridElementType.DataCell && (e.Column as DevExpress.Blazor.DxGridDataColumn).FieldName == "FactoryCancel")
            {
                //e.Style = "font-weight: 800";
            }
            if (e.ElementType == GridElementType.DataCell && (e.Column as DevExpress.Blazor.DxGridDataColumn).FieldName == "PONumber")
            {
                //e.Style = "font-weight: 800";
            }
            if (e.ElementType == GridElementType.DataCell && (e.Column as DevExpress.Blazor.DxGridDataColumn).FieldName == "Units")
            {
                //decimal units = (decimal)e.Grid.GetRowValue(e.VisibleIndex, "Units");
                //if (units > 100)
                //    e.Style = "color: red";
            }
            if (e.ElementType == GridElementType.DataCell && (e.Column as DevExpress.Blazor.DxGridDataColumn).FieldName == "SOQty")
            {
                //decimal units = (decimal)e.Grid.GetRowValue(e.VisibleIndex, "SOQty");
                //if (units > 100)
                //    e.Style = "color: red";
            }
        }
        void Grid_CustomizeCellDisplayText(GridCustomizeCellDisplayTextEventArgs e)
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

        void Grid_CustomizeEditModel(GridCustomizeEditModelEventArgs e)
        {
            if (e.IsNew)
            {
                if (e.EditModel.GetType() == typeof(ShippingData))
                {
                    var newObject = (ShippingData)e.EditModel;
                    newObject.Id = GridData.Count() + 1;
                    newObject.ShipmentDate = DateTime.Now;

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
        async Task Grid_EditModelSaving(GridEditModelSavingEventArgs e)
        {
            e.Cancel = true;

            if (e.EditModel.GetType() == typeof(ShippingData))
            {
                var savingObject = (ShippingData)e.EditModel;

                if (e.IsNew)
                    await InsertShippingDataAsync(savingObject);
                else
                    await UpdateShippingDataAsync(savingObject);

                await Grid.CancelEditAsync();
            }
        }
        async Task Grid_DataItemDeleting(GridDataItemDeletingEventArgs e)
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

        async Task InsertShippingDataAsync(ShippingData item)
        {
            // Don't save if empty detail list
            if (POShipmentListGridData.Count() == 0)
                return;

            if (item.TrackingNumber == null) item.TrackingNumber = "";
            if (item.InvoiceNo == null) item.InvoiceNo = "";

            string query = @"INSERT INTO [PIMS].[dbo].[ShipmentHeader] ([ShipmentDate],[TrackingNumber],[InvoiceNo],[CreatedOn]) 
        SELECT '{0}','{1}','{2}',GETDATE()
        SELECT @@IDENTITY AS 'pk'";
            string fullQuery = string.Format(query, item.ShipmentDate, item.TrackingNumber, item.InvoiceNo);

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

                    if (ship.ShipmentDetailId == 0)
                    {
                        //int shipmentHeaderId = item.ShipmentHeaderId;
                        int poDetailId = ship.PODetailId;
                        int shipmentQty = ship.ShipmentQty;

                        // NEW insert
                        string detailInsertQuery = @"INSERT INTO [PIMS].[dbo].[ShipmentDetails] ([ShipmentHeaderId],[PODetailId],[ShipmentQty],[CreatedOn])
                SELECT {0},{1},{2}, GETDATE()
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
                            // UPDATE
                            string detailUpdateQuery = @"UPDATE [PIMS].[dbo].[ShipmentDetails] 
SET [ShipmentQty] = {1}
--, [LastModifiedOn] = GETDATE() 
WHERE [PODetailId] = {0}";
                            string detailUpdateFullQuery = string.Format(detailUpdateQuery, item.PODetailId, item.OrderQty);

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

        }
        async Task UpdateShippingDataAsync(ShippingData item)
        {
            // Don't save if empty detail list
            if (POShipmentListGridData.Count() == 0)
                return;

            string query = @"UPDATE [PIMS].[dbo].[ShipmentHeader] SET
       [ShipmentDate] = '{1}'
      ,[TrackingNumber] = '{2}'
      ,[ShipToETA] = '{3}'
      ,[InvoiceNo] = '{4}'
      --,[LastModifiedOn] = GETDATE()
      WHERE [ShipmentHeaderId] = {0}";
            string fullQuery = string.Format(query, item.ShipmentHeaderId, item.ShipmentDate, item.TrackingNumber, item.ShipToETA, item.InvoiceNo);

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
                        string detailInsertQuery = @"INSERT INTO [PIMS].[dbo].[ShipmentDetails] ([ShipmentHeaderId],[PODetailId],[ShipmentQty],[CreatedOn])
                SELECT {0},{1},{2}, GETDATE()
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
                        //if (ship.OrderQty != ship.ShipmentQty && ship.ShipmentQty != ship.LastShipmentQty)
                        if (ship.ShipmentQty != ship.LastShipmentQty)
                        {
                            // UPDATE
                            string detailUpdateQuery = @"UPDATE [PIMS].[dbo].[ShipmentDetails] 
SET [ShipmentQty] = {1}
--, [LastModifiedOn] = GETDATE() 
WHERE [PODetailId] = {0}";
                            string detailUpdateFullQuery = string.Format(detailUpdateQuery, ship.PODetailId, ship.ShipmentQty);

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
        }

        public void CancelAddItem(MouseEventArgs args)
        {
            //...
        }

        public void UpdateAddItem(MouseEventArgs args)
        {
            //...
        }

        void UpdateOpenPOShipmentData(ShippingData poShipmentListGridDataItem)
        {
            //OpenPOShipmentData = await sqlData.GetOpenPOShipmentData();

            //ShippingData openPOShipmentDataItem = OpenPOShipmentData.Where(c => c.Id == poShipmentListGridDataItem.Id).FirstOrDefault();
            //if (openPOShipmentDataItem != null)
            //{
            //    openPOShipmentDataItem.ShipmentHeaderId = poShipmentListGridDataItem.ShipmentHeaderId;
            //    openPOShipmentDataItem.ShipmentDate = poShipmentListGridDataItem.ShipmentDate;
            //    openPOShipmentDataItem.InvoiceNo = poShipmentListGridDataItem.InvoiceNo;
            //    openPOShipmentDataItem.TrackingNumber = poShipmentListGridDataItem.TrackingNumber;
            //    openPOShipmentDataItem.ShipToETA = poShipmentListGridDataItem.ShipToETA;
            //    openPOShipmentDataItem.PONumber = poShipmentListGridDataItem.PONumber;
            //    openPOShipmentDataItem.ProductNo = poShipmentListGridDataItem.ProductNo;
            //    openPOShipmentDataItem.ProductName = poShipmentListGridDataItem.ProductName;
            //    openPOShipmentDataItem.OrderQty = poShipmentListGridDataItem.OrderQty;
            //    openPOShipmentDataItem.ShipmentQty = poShipmentListGridDataItem.ShipmentQty;
            //    openPOShipmentDataItem.ShipmentDetailId = poShipmentListGridDataItem.ShipmentDetailId;
            //    openPOShipmentDataItem.PODetailId = poShipmentListGridDataItem.PODetailId;
            //    openPOShipmentDataItem.ProductId = poShipmentListGridDataItem.ProductId;
            //
            //    openPOShipmentDataItem.POHeaderId = poShipmentListGridDataItem.POHeaderId;
            //    openPOShipmentDataItem.SOHeaderId = poShipmentListGridDataItem.SOHeaderId;
            //    openPOShipmentDataItem.SODetailId = poShipmentListGridDataItem.SODetailId;
            //
            //    openPOShipmentDataItem.VendorId = poShipmentListGridDataItem.VendorId;
            //    openPOShipmentDataItem.VendorName = poShipmentListGridDataItem.VendorName;
            //    openPOShipmentDataItem.CustomerId = poShipmentListGridDataItem.CustomerId;
            //    openPOShipmentDataItem.CustomerName = poShipmentListGridDataItem.CustomerName;
            //
            //    openPOShipmentDataItem.SODate = poShipmentListGridDataItem.SODate;
            //    openPOShipmentDataItem.PODate = poShipmentListGridDataItem.PODate;
            //    openPOShipmentDataItem.EndDate = poShipmentListGridDataItem.EndDate;
            //    openPOShipmentDataItem.StartDate = poShipmentListGridDataItem.StartDate;
            //    openPOShipmentDataItem.SONumber = poShipmentListGridDataItem.SONumber;
            //
            //    openPOShipmentDataItem.ForProductNo = poShipmentListGridDataItem.ForProductNo;
            //}
        }

        #endregion

        // ============================================================ \\

        #region Template POList Grid Functions

        async void POListGrid_OnRowClick(GridRowClickEventArgs e)
        {
            //if (ItemGrid != null)
            //{
            //    ItemGrid.StartEditRowAsync(e.VisibleIndex);
            //}

            if (POListGrid != null)
            {
                //if (POListGrid.GetDataItem(e.VisibleIndex) is ShippingData)
                //    POHeaderIdSelected = (POListGrid.GetDataItem(e.VisibleIndex) as POOpenDetail).POHeaderId;
                //LoadItemGridData((POListGrid.GetDataItem(e.VisibleIndex) as POOpenDetail).PONumber); 

                if (POListGrid.GetDataItem(e.VisibleIndex) is ShippingData)
                    POHeaderIdSelected = (POListGrid.GetDataItem(e.VisibleIndex) as ShippingData).POHeaderId;
                await LoadItemGridData((POListGrid.GetDataItem(e.VisibleIndex) as ShippingData).PONumber);

                int incomingUnitsSum = (POShipmentListGridData.Count() == 0) ? 0 : POShipmentListGridData.Select(x => x.LastShipmentQty).Sum();
                ShippingData shipingData = POListGridData.Where(x => x.POHeaderId == POHeaderIdSelected).FirstOrDefault();
                if (shipingData != null)
                {
                    shipingData.TotalSum = incomingUnitsSum;
                    //(POListGridData.Where(x => x.POHeaderId == POHeaderIdSelected).FirstOrDefault()).LastShipmentQty = incomingUnitsSum;
                }
            }
        }

        async Task POListGrid_OnRowDoubleClick(GridRowClickEventArgs e)
        {
            //await e.Grid.SaveChangesAsync();
            //await e.Grid.StartEditRowAsync(e.VisibleIndex);
        }

        void POListGrid_CustomizeElement(GridCustomizeElementEventArgs e)
        {
            if (e.ElementType == GridElementType.DataCell && (e.Column as DevExpress.Blazor.DxGridDataColumn).FieldName == "ShipmentQty")
            {
                e.Style = "font-weight: 800";
                e.Style = "color: red";
            }
        }

        async Task POListGrid_DataItemDeleting(GridDataItemDeletingEventArgs e)
        {
            if (e.DataItem.GetType() == typeof(ShippingData))
            {
                var deletingObject = (ShippingData)e.DataItem;

                //// Delete from POListGridData
                //POListGridData.Remove(POListGridData.First(c => c.POHeaderId == deletingObject.POHeaderId));
                POListGridData = POListGridData.Where(c => c.POHeaderId != deletingObject.POHeaderId).ToList();

                // Delete from POShipmentListGridData
                POShipmentListGridData = POShipmentListGridData.Where(c => c.POHeaderId != deletingObject.POHeaderId).ToList();
            }
        }

        #endregion

        // ============================================================ \\

        #region Template PO Items Grid Functions

        void ItemGrid_OnRowClick(GridRowClickEventArgs e)
        {
            if (POShipmentListGrid != null)
            {
                //if (POShipmentListGrid.GetDataItem(e.VisibleIndex) is ShippingData)
                //{
                //    ShippingData ship = POShipmentListGrid.GetDataItem(e.VisibleIndex) as ShippingData;
                //    if (ship.ShipmentQty == 0)
                //        ship.ShipmentQty = ship.OrderQty;
                //}
                //
                //POShipmentListGrid.StartEditRowAsync(e.VisibleIndex); 
            }
        }

        async Task ItemGrid_OnRowDoubleClick(GridRowClickEventArgs e)
        {
            ////await e.Grid.SaveChangesAsync();
            ////FocusedColumn = (e.Column as DxGridDataColumn).FieldName;
            //await e.Grid.StartEditRowAsync(e.VisibleIndex); 
        }

        void OnSelectedDataItemsChanged(IReadOnlyList<object> selectedDataItems)
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

        void ItemGrid_CustomizeElement(GridCustomizeElementEventArgs e)
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
        }

        void ItemGrid_FilterCriteriaChanged(GridFilterCriteriaChangedEventArgs args)
        {
            //CriteriaOperator newCriteria = args.FilterCriteria;
            // ...
        }

        void ItemGrid_CustomizeEditModel(GridCustomizeEditModelEventArgs e)
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

        async Task ItemGrid_OnEditStart(GridEditStartEventArgs e)
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

        async Task ItemGrid_EditModelSaving(GridEditModelSavingEventArgs e)
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

        private async Task OnButtonClick()
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

        async Task OnPopupDragCompleted(PopupDragCompletedEventArgs args)
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