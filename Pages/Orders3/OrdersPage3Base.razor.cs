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

namespace LAGem_POPortal.Orders3
{
    public class OrdersPage3Base : ComponentBase
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

        [CascadingParameter]
        public Task<AuthenticationState> authenticationState { get; set; }
        string currentUser { get; set; } = "";

        public IGrid OrdersHeaderGrid { get; set; }
        public IGrid OrdersDetailEDIGrid { get; set; }
        public IGrid OrdersDetailJewelryGrid { get; set; }
        public IGrid OrdersDetailPackagingGrid { get; set; }
        public IEnumerable<SoEdiData> OrdersHeaderGridData { get; set; }           // Main grid data (SO/EDI)
        public IEnumerable<SoEdiData> EDIOrdersDetailGridData { get; set; }        // EDI Detail grid data

        public IEnumerable<CustomerSoPoData> SOOrdersDetailGridData { get; set; }  // SO Orders Detail grid data
        public IEnumerable<CustomerSoPoData> EdiOrderDetailGridJewelryData { get; set; }
        public IEnumerable<CustomerSoPoData> EdiOrderDetailGridPackagingData { get; set; }

        public DxGrid LinkEdiOrderDetailGrid { get; set; }
        public DxGrid LinkSOOrderDetailGrid { get; set; }
        //IEnumerable<EdiOrderDetailData> LinkEdiOrderDetailGridData { get; set; }
        public IEnumerable<SoEdiData> LinkEdiOrderDetailGridData { get; set; }
        public IEnumerable<CustomerSoPoData> LinkSOOrderDetailGridData { get; set; }

        public DxGrid EdiOrderHeaderGrid { get; set; } // No longer used
        public DxGrid EdiOrderDetailGrid { get; set; } // No longer used
        public IEnumerable<EdiOrderDetailData> EdiOrderHeaderGridData { get; set; } // No longer used
        public IEnumerable<EdiOrderDetailData> EdiOrderDetailGridData { get; set; } // No longer used

        public DxGrid TasksMainGrid { get; set; }
        public DxGrid TasksDetailGrid { get; set; }
        public IEnumerable<TaskQueue> TasksMainGridData { get; set; }
        public IEnumerable<TaskQueue> TasksDetailGridData { get; set; }

        public DxGrid TestsMainGrid { get; set; }
        public IEnumerable<ProductTest> TestsMainGridData { get; set; }

        // ------------------------------------------------------------ \\

        public int PageSize { get; set; } = 100;
        public int ActivePageIndex { get; set; } = 0;
        public int PageCount { get; set; } = 0;
        public int TotalRecords { get; set; } = 0;
        public bool ShowAllRows { get; set; } = true;

        public bool ShowSearchBox { get; set; } = true;
        public bool AutoCollapseDetailRow { get; set; } = true;
        public bool AutoExpandAllGroupRows { get; set; } = true;

        public bool TextWrapEnabled = true;
        public bool WordWrapEnabled = false;

        public TaskCompletionSource<bool> DataLoadedTcs { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

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
        public bool usePopupEditForm { get; set; } = true;
        public GridEditMode CurrentEditMode { get { return usePopupEditForm ? GridEditMode.PopupEditForm : GridEditMode.EditForm; } } // GridEditMode.EditRow
        public string mainGridEditFormHeaderText { get; set; } = "EDI Edit";

        public bool ShowGroupPanel { get; set; } = true;
        public Dictionary<string, string[]> GroupInfo { get; } = new Dictionary<string, string[]> {
        { "ShipYear", new string[] { "ShipYear" } },
        { "ShipYear, MondayOfTheWeek", new string[] { "ShipYear", "MondayOfTheWeek" } }
    };
        public string CurrentGroupInfoKey { get; set; } = "SOShipYear";

        public bool showToolbar { get; set; } = false;
        //string gridCss => !showToolbar ? "hide-toolbar my-partnertasks-grid" : "my-partnertasks-grid";
        public string gridCss => "hide-toolbar my-partnertasks-grid";
        public DateTime blankDate { get; set; } = new DateTime(1800, 1, 1);
        public int ActiveTabIndex { get; set; } = 1;

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

        public DxWindow windowRefPOSearch;
        public bool windowRefPOSearchVisible { get; set; } = false;

        public DxWindow windowRefLinking;
        public bool windowRefLinkingVisible { get; set; } = false;

        public ElementReference popupTarget;
        public string searchTitleText { get; set; } = "Search";
        public string GetButtonText() => !windowRefLinkingVisible ? "SHOW A WINDOW" : "CLOSE A WINDOW";
        public bool displayPopupGrid { get; set; } = true;
        public string linkingItemText { get; set; } = "";

        // ------------------------------------------------------------ \\

        public CriteriaOperator criteriaOrderOperator { get; set; }
        public CriteriaOperator criteriaPackagingOperator { get; set; }

        public IEnumerable<BusinessPartner> BusinessPartnerListData { get; set; }
        public List<Lookup> partnerList { get; set; }
        public int businessPartnerIdSelected { get; set; } = 0;

        public string headerNote { get; set; } = "";
        public SoEdiData selectedRow { get; set; } // main row selected
        public SoEdiData linkingRow { get; set; } // detail row
        public SoEdiData unLinkingRow { get; set; } // detail row
        public EdiOrderDetailData selectedEdiRow { get; set; }

        public SoEdiData linkingSoEdiDataItem { get; set; }
        public CustomerSoPoData linkingCustomerSoPoDataItem { get; set; }

        public bool isBlankOrder { get; set; } = false;

        public string selectedSono { get; set; } = "";
        public string selectedEdiPO { get; set; } = "";

        public DxTextBox searchBoxRef;
        public string searchText { get; set; } = "";
        public bool linkingEdiPOtoSO { get; set; } = false;
        public bool useSearchButton { get; set; } = true;

        #endregion

        // ============================================================ \\

        #region Constructors/Page Functions

        protected async override Task OnInitializedAsync()
        {
            base.OnInitialized();

            // var authstate = await authStateProvider.GetAuthenticationStateAsync();
            // var userClaimsPrincipal = authstate.User; // ClaimsPrincipal
            // var userClaimsPrincipalName = userClaimsPrincipal.Identity.Name;
            // currentUser = userClaimsPrincipal.Identity.Name;

            // // var user = (await authenticationState).User;
            // // if (!user.Identity.IsAuthenticated)
            // // {
            // //     //NavigationManager.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(NavigationManager.Uri)}");
            // //     //navManager.NavigateTo("/Login", true);
            // //     //navManager.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(navManager.Uri)}");
            // // }
            // // else { }

            // // var authstate = await authStateProvider.GetAuthenticationStateAsync();
            // // var userClaimsPrincipal = authstate.User; // ClaimsPrincipal
            // // var userClaimsPrincipalName = userClaimsPrincipal.Identity.Name;

            // // if (userClaimsPrincipalName != null)
            // // {
            // //     var userData = userAccountService.GetUserAccountFromClaims(userClaimsPrincipal);

            // //     if (userData != null)
            // //     {
            // //         if (userData.Role == "Administrator") { }
            // //     }
            // //     else { }
            // // }

            await LoadGridHeaderData();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await AfterRenderAsync(firstRender);

            // if (firstRender)
            // {
            //     if (OrdersHeaderGrid != null)
            //     {
            //         OrdersHeaderGrid.BeginUpdate();
            //         OrdersHeaderGrid.ClearSort();
            //         OrdersHeaderGrid.GroupBy("ShipYear");
            //         //OrdersHeaderGrid.GroupBy("ShipWindow");
            //         OrdersHeaderGrid.GroupBy("MondayOfTheWeek");
            //         OrdersHeaderGrid.SortBy("ShipYear", GridColumnSortOrder.Descending);
            //         //OrdersHeaderGrid.SortBy("ShipWindow", GridColumnSortOrder.Descending);
            //         OrdersHeaderGrid.SortBy("MondayOfTheWeek", GridColumnSortOrder.Descending);
            //         OrdersHeaderGrid.EndUpdate();

            //         //OrdersHeaderGrid.ShowAllRows = true;

            //         var criteria = new InOperator("Archived", new Boolean[] { false });
            //         criteriaOrderOperator = criteria;
            //         if (OrdersHeaderGrid != null)
            //             OrdersHeaderGrid.SetFilterCriteria(criteriaOrderOperator);
            //     }

            //     await DataLoadedTcs.Task;
            //     // Waits for grid data to load
            //     // Grid.ExpandDetailRow(0);
            // }

            // if (firstRender)
            // {
            //     ////https://demos.devexpress.com/blazor/Popup
            //     var position = await LoadPositionFromLocalStorageAsync();
            //     (positionX, positionY) = (position?.X ?? null, position?.Y ?? null);
            //     StateHasChanged();
            // }
        }

        public void Dispose()
        {
            // Northwind?.Dispose();
        }


        public async Task AfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (OrdersHeaderGrid != null)
                {
                    OrdersHeaderGrid.BeginUpdate();
                    OrdersHeaderGrid.ClearSort();
                    OrdersHeaderGrid.GroupBy("ShipYear");
                    //OrdersHeaderGrid.GroupBy("ShipWindow");
                    OrdersHeaderGrid.GroupBy("MondayOfTheWeek");
                    OrdersHeaderGrid.SortBy("ShipYear", GridColumnSortOrder.Descending);
                    //OrdersHeaderGrid.SortBy("ShipWindow", GridColumnSortOrder.Descending);
                    OrdersHeaderGrid.SortBy("MondayOfTheWeek", GridColumnSortOrder.Descending);
                    OrdersHeaderGrid.EndUpdate();

                    //OrdersHeaderGrid.ShowAllRows = true;

                    var criteria = new InOperator("Archived", new Boolean[] { false });
                    criteriaOrderOperator = criteria;
                    if (OrdersHeaderGrid != null)
                        OrdersHeaderGrid.SetFilterCriteria(criteriaOrderOperator);
                }

                await DataLoadedTcs.Task;
                // Waits for grid data to load
                // Grid.ExpandDetailRow(0);
            }

            if (firstRender)
            {
                ////https://demos.devexpress.com/blazor/Popup
                var position = await LoadPositionFromLocalStorageAsync();
                (positionX, positionY) = (position?.X ?? null, position?.Y ?? null);
                StateHasChanged();
            }
        }

        #endregion

        // ============================================================ \\

        #region Popup Functions

        public async void OkPopupClick()
        {
            PopupVisible = false;

            if (callbackProcessName == "SaveLinks")
            {
                //@PopupLinkingFormContext.CloseCallback
                windowRefLinking.CloseAsync();

                await SaveCustomerPOLinking();
            }
            if (callbackProcessName == "UnLinkSave")
            {
                await OnUnLinkPOItemSave(unLinkingRow);
            }
            if (callbackProcessName == "SaveBlankOrderLinks")
            {
                await ToggleSearchPopupVisibilityAsync();
                await ToggleLinkingPopupVisibilityAsync();

                await SaveCustomerPOLinking();
            }
            if (callbackProcessName == "Archive")
            {
                foreach (SoEdiData item in SelectedDataItems.Cast<SoEdiData>())
                {
                    if (!item.Archived)
                    {
                        item.Archived = true;

                        // Update SQL table
                        string query = @"UPDATE [PIMS].[dbo].[SOHeader] SET [Archived] = 1 WHERE [SOHeaderId] = {0}";
                        string fullQuery = string.Format(query, item.SOHeaderId);

                        using (var uow = new UnitOfWork())
                        {
                            await uow.ExecuteNonQueryAsync(fullQuery);
                        }

                        OrdersHeaderGrid.BeginUpdate();
                        OrdersHeaderGrid.ClearFilter();
                        OrdersHeaderGrid.SetFilterCriteria(criteriaOrderOperator);
                        OrdersHeaderGrid.EndUpdate();

                        // Clear list
                        SelectedDataItems = new List<object>();

                        await InvokeAsync(StateHasChanged); // <-- refreshes
                    }
                }
            }
        }

        public void CancelPopupClick()
        {
            PopupVisible = false;
            linkingEdiPOtoSO = false;

            if (callbackProcessName == "Archive")
            {
                // Clear list
                SelectedDataItems = new List<object>();
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

        #region Grid Functions

        public void AutoCollapseDetailRow_Changed(bool newValue)
        {
            AutoCollapseDetailRow = newValue;
            if (newValue)
            {
                OrdersHeaderGrid.BeginUpdate();
                OrdersHeaderGrid.CollapseAllDetailRows();
                OrdersHeaderGrid.ExpandDetailRow(0);
                OrdersHeaderGrid.EndUpdate();
            }
        }

        public void Grid_CustomizeElement(GridCustomizeElementEventArgs e)
        {
            if (e.Grid.Data != null)
            {
                if (e.Grid.Data.GetType().GetGenericArguments().Single() == typeof(SoEdiData))
                {
                    //if (e.ElementType == GridElementType.DataCell && (e.Column as DevExpress.Blazor.DxGridDataColumn).FieldName == "Description")
                    if (e.ElementType == GridElementType.DataCell)
                    {
                        string fieldName = (e.Column as DevExpress.Blazor.DxGridDataColumn).FieldName;

                        if (fieldName == "Description")
                        {
                            string desc = e.Grid.GetRowValue(e.VisibleIndex, "Description").ToString();
                            if (desc.ToLower().Contains("auto forecast"))
                            {
                                e.Style = "color: red";
                            }
                        }

                        if (fieldName == "IntransitUnits")
                        {
                            int intransitUnits = (int)e.Grid.GetRowValue(e.VisibleIndex, "IntransitUnits");
                            int orderQty = (int)e.Grid.GetRowValue(e.VisibleIndex, "OrderQty");
                            if (intransitUnits > 0 && intransitUnits >= orderQty)
                            {
                                //e.Style = "color: green";
                                e.Style = "background: green";
                            }
                        }

                        if (fieldName == "ShipYear")
                        {
                            string shipYear = e.Grid.GetRowValue(e.VisibleIndex, "ShipYear").ToString();
                            if (shipYear.ToLower().Contains("1900") || shipYear.ToLower().Contains("auto"))
                            {
                                e.Style = "color: red";
                            }
                        }

                        if (fieldName == "MondayOfTheWeek")
                        {
                            string mondayOfTheWeek = e.Grid.GetRowValue(e.VisibleIndex, "MondayOfTheWeek").ToString();
                            if (mondayOfTheWeek.ToLower().Contains("1900") || mondayOfTheWeek.ToLower().Contains("auto"))
                            {
                                e.Style = "color: red";
                            }
                        }
                    }
                }

                if (e.ElementType == GridElementType.GroupRow)
                {
                    if ((e.Column as DevExpress.Blazor.DxGridDataColumn).FieldName == "ShipYear")
                    {
                        string shipYear = e.Grid.GetRowValue(e.VisibleIndex, "ShipYear").ToString();
                        if (shipYear.ToLower().Contains("1900") || shipYear.ToLower().Contains("auto"))
                        {
                            e.Style = "color: red";
                        }
                    }
                    if ((e.Column as DevExpress.Blazor.DxGridDataColumn).FieldName == "MondayOfTheWeek")
                    {
                        string mondayOfTheWeek = e.Grid.GetRowValue(e.VisibleIndex, "MondayOfTheWeek").ToString();
                        if (mondayOfTheWeek.ToLower().Contains("1900") || mondayOfTheWeek.ToLower().Contains("auto"))
                        {
                            e.Style = "color: red";
                        }
                    }
                }
            }
        }
        public void Grid_CustomizeCellDisplayText(GridCustomizeCellDisplayTextEventArgs e)
        {
            // string[] dateList = new string[] { "SODate", "StartDate", "EndDate", "ShipmentDate", "ShipToETA" };
            // if (dateList.Contains(e.FieldName))
            if (e.Value.GetType() == typeof(DateTime))
            {
                if (DateTime.Parse(e.Value.ToString()) <= new DateTime(1900, 1, 1))
                    e.DisplayText = "";
            }
        }

        public async Task MainGrid_CustomizeEditModel(GridCustomizeEditModelEventArgs e)
        {
            if (e.IsNew)
            {
                if (e.EditModel.GetType() == typeof(SoEdiData))
                {
                    var newObject = (SoEdiData)e.EditModel;
                    newObject.Description = "Auto Forecast";
                }
            }
        }
        public async Task MainGrid_EditModelSaving(GridEditModelSavingEventArgs e)
        {
            e.Cancel = true;

            if (e.EditModel.GetType() == typeof(SoEdiData))
            {
                var savingObject = (SoEdiData)e.EditModel;

                if (e.IsNew)
                    await InsertOrderDataAsync(savingObject);
                else
                {
                    if (savingObject.Id == 0)
                        await InsertOrderDataAsync(savingObject);
                    else
                        await UpdateOrderDataAsync(savingObject);
                }

                await OrdersHeaderGrid.CancelEditAsync();
            }
        }
        public async Task MainGridDataItemDeleting(GridDataItemDeletingEventArgs e)
        {
            var deletingData = (SoEdiData)e.DataItem;

            // await NwindDataService.RemoveEmployeeAsync((EditableEmployee)e.DataItem);
            //await UpdateDataAsync();
        }
        public async Task UpdateDataAsync()
        {
            // SOGridData = await sqlData.GetCustomerSoData();
        }

        public async Task DetailGrid_EditModelSaving(GridEditModelSavingEventArgs e)
        {
            e.Cancel = true;

            if (e.EditModel.GetType() == typeof(SoEdiData))
            {
                var savingObject = (SoEdiData)e.EditModel;

                if (e.IsNew)
                    await InsertOrderDataAsync(savingObject);
                else
                {
                    if (savingObject.Id == 0)
                        await InsertOrderDataAsync(savingObject);
                    else
                        await UpdateOrderDataAsync(savingObject);
                }

                await OrdersHeaderGrid.CancelEditAsync();
            }
        }
        public async Task DetailGrid_DataItemDeleting(GridDataItemDeletingEventArgs e)
        {
            if (e.DataItem.GetType() == typeof(SoEdiData))
            {
                var deletingObject = (SoEdiData)e.DataItem;

                //await DeleteBusinessPartnerTaskDataAsync(deletingObject);

                //await Grid.CancelEditAsync();
                //await GridDetail.CancelEditAsync();
            }
        }
        public async Task DetailGrid_EditCanceling(GridEditCancelingEventArgs e)
        {

        }
        public async Task DetailGrid_CustomizeEditModel(GridCustomizeEditModelEventArgs e)
        {
            if (e.IsNew)
            {
                if (e.EditModel.GetType() == typeof(SoEdiData))
                {
                    var newObject = (SoEdiData)e.EditModel;

                }
            }
        }

        public async void EDIItemGrid_OnRowClick(GridRowClickEventArgs e)
        {
            if (LinkEdiOrderDetailGrid != null && e.Column.GetType() != typeof(DevExpress.Blazor.DxGridCommandColumn))
            {
                if (LinkEdiOrderDetailGrid.GetDataItem(e.VisibleIndex) is SoEdiData)
                {
                    SoEdiData item = LinkEdiOrderDetailGrid.GetDataItem(e.VisibleIndex) as SoEdiData;
                    await OnLinkPOItemClick(item, "link");
                }

                //LinkEdiOrderDetailGrid.StartEditRowAsync(e.VisibleIndex);
            }
        }
        public async Task EDIItemGrid_OnRowDoubleClick(GridRowClickEventArgs e)
        {
            ////await e.Grid.SaveChangesAsync();
            ////FocusedColumn = (e.Column as DxGridDataColumn).FieldName;
            //await e.Grid.StartEditRowAsync(e.VisibleIndex);
        }

        public async void SOItemGrid_OnRowClick(GridRowClickEventArgs e)
        {
            if (LinkSOOrderDetailGrid != null)
            {
                if (LinkSOOrderDetailGrid.GetDataItem(e.VisibleIndex) is CustomerSoPoData)
                {
                    CustomerSoPoData item = LinkSOOrderDetailGrid.GetDataItem(e.VisibleIndex) as CustomerSoPoData;
                    await OnLinkPOItemClick(item, "link");
                }

                //LinkSOOrderDetailGrid.StartEditRowAsync(e.VisibleIndex);
            }
        }
        public async Task SOItemGrid_OnRowDoubleClick(GridRowClickEventArgs e)
        {
            ////await e.Grid.SaveChangesAsync();
            ////FocusedColumn = (e.Column as DxGridDataColumn).FieldName;
            //await e.Grid.StartEditRowAsync(e.VisibleIndex);
        }

        public async Task InsertOrderDataAsync(SoEdiData item) // Save Auto Forecast Order
        {
            string query = @"INSERT INTO [PIMS].[dbo].[SOHeader]
        ([SOHeaderId]
      ,[SONumber]
      ,[SODate]
      ,[BusinessPartnerId]
      ,[StartDate]
      ,[EndDate]
      ,[TotalCost]
      ,[TotalPrice]
      ,[PONumber]
      ,[PostedToERP]
      ,[PostedDate]
      ,[SOStatusId]
      ,[SOStatusDate]
      ,[LegacySystemId]
      ,[Description]
      ,[QBSO]
      ,[Comments]
      ,[CreatedOn]
      ,[ShipWindow])
      SELECT 1000000 + (SELECT COUNT(*) FROM [PIMS].[dbo].[SOHeader] WHERE [SOHeaderId] >= 1000000) + 1 AS [NewSOHeaderId]
      ,''           AS [SONumber]
      ,GETDATE()    AS [SODate]
      ,{0}          AS [BusinessPartnerId]
      ,'{1}'        AS [StartDate]
      ,'{2}'        AS [EndDate]
      ,{3}          AS [TotalCost]
      ,{4}          AS [TotalPrice]
      ,''           AS [PONumber]
      ,0            AS [PostedToERP]
      ,'1900-01-01 00:00:00.000' AS [PostedDate]
      ,0            AS [SOStatusId]
      ,'1900-01-01 00:00:00.000' AS [SOStatusDate]
      ,0 AS [LegacySystemId]
      ,'{5}'        AS [Description]
      ,'{6}'        AS [QBSO]
      ,'{7}'        AS [Comments]
      ,GETDATE() AS [CreatedOn]
      , '{8}'       AS [ShipWindow]
        ";
            string fullQuery = string.Format(query, item.CustomerId, item.StartDate, item.EndDate, item.OrderQty, item.Price, item.Description, item.QBSO, item.Comments, item.StartDate); // item.ShipWindow = item.StartDate on New entry

            using (var uow = new UnitOfWork())
            {
                await uow.ExecuteNonQueryAsync(fullQuery);
            }

            SqlData sqlData = new SqlData();
            OrdersHeaderGridData = await sqlData.GetSoEdiData();
        }

        public async Task UpdateOrderDataAsync(SoEdiData item)
        {
            // Note: if and unlinked order,save to [PIMS].[dbo].[SOHeader]
            string query = "";
            string fullQuery = "";
            if (item.IsLinked == 1)
            {
                query = @"
UPDATE [PIMS].[edi].[EdiHdr] SET
    [Description] = '{1}'
    ,[QBSO] = '{2}'
    ,[Comments] = '{3}'
WHERE [Edihdrid] = {0}

UPDATE [PIMS].[edi].[EdiTrn] SET
    [ShipWindow] = '{4}'
WHERE [Edihdrid] = {0}
";
                fullQuery = string.Format(query, item.EdiHdrId, item.Description, item.QBSO, item.Comments, item.ShipWindow);
            }
            else
            {
                query = @"
UPDATE [PIMS].[dbo].[SOHeader] SET
    [Description] = '{1}'
    ,[QBSO] = '{2}'
    ,[Comments] = '{3}'
    ,[ShipWindow] = '{4}'
WHERE [SOHeaderId] = {0}";
                fullQuery = string.Format(query, item.SOHeaderId, item.Description, item.QBSO, item.Comments, item.ShipWindow);
            }
            using (var uow = new UnitOfWork())
            {
                await uow.ExecuteNonQueryAsync(fullQuery);
            }

            SqlData sqlData = new SqlData();
            OrdersHeaderGridData = await sqlData.GetSoEdiData();
        }

        public async void OnSelectedDataItemsChanged(IReadOnlyList<object> selectedDataItems)
        {
            SelectedDataItems = selectedDataItems;
            //onTicketSelectionChanged.InvokeAsync(selectedDataItems);

            foreach (SoEdiData item in selectedDataItems.Cast<SoEdiData>())
            {
                if (!item.Archived)
                {
                    string message = "Are you sure you want to " + Environment.NewLine + "archived i2 SO " + item.SONumber + "?";
                    DisplayPopupQuestion(message, "Confirmation", "Archive");
                    //item.Archived = true;  

                    //OrdersHeaderGrid.BeginUpdate();
                    //OrdersHeaderGrid.ClearFilter();
                    //OrdersHeaderGrid.SetFilterCriteria(criteriaOrderOperator);  
                    //OrdersHeaderGrid.EndUpdate();
                    //await InvokeAsync(StateHasChanged); // <-- refreshes
                }
            }
        }

        #endregion

        // ============================================================ \\

        #region Task/Test Tabs Functions

        public async Task LoadGridHeaderData()
        {
            SqlData sqlData = new SqlData();
            OrdersHeaderGridData = await sqlData.GetSoEdiData();

            BusinessPartnerListData = await sqlData.GetCustomerBusinessPartnerList();
            partnerList = GetPartnerData();
        }

        public async Task LoadGridDetailData(SoEdiData row)
        {
            //if (selectedRow != null && selectedRow.SOHeaderId == row.SOHeaderId)
            if (JsonConvert.SerializeObject(selectedRow) == JsonConvert.SerializeObject(row))
                return;

            SqlData sqlData = new SqlData();
            EDIOrdersDetailGridData = await sqlData.GetSoEdiDetailData(row.EdiHdrId, false, false);
            SOOrdersDetailGridData = await sqlData.GetCustomerSoPoDetailData(row.SOHeaderId, true);

            EdiOrderDetailGridJewelryData = (from o in SOOrdersDetailGridData where o.SoSubLineType == "Customer Order" select o).Cast<CustomerSoPoData>();
            EdiOrderDetailGridPackagingData = (from o in SOOrdersDetailGridData where o.SoSubLineType == "Packaging" select o).Cast<CustomerSoPoData>();

            if (ActiveTabIndex == 0 && EDIOrdersDetailGridData.Count() == 0)
                ActiveTabIndex = 1;
            else
            {
                if (EDIOrdersDetailGridData.Count() > 0 && ActiveTabIndex <= 1)
                    ActiveTabIndex = 0;
            }

            await InvokeAsync(StateHasChanged); // <-- refreshes
        }

        public async Task LoadTasksGridData(SoEdiData row)
        {
            if (JsonConvert.SerializeObject(selectedRow) == JsonConvert.SerializeObject(row))
                return;

            SqlData sqlData = new SqlData();
            TasksMainGridData = await sqlData.GetAllProductQueueTasks(row.SOHeaderId);

            await InvokeAsync(StateHasChanged); // <-- refreshes
        }

        public async Task LoadTasksDetailGridData(string gridName, int id, int headerId)
        {
            if (gridName == "SubGrid")
            {
                SqlData sqlData = new SqlData();
                if (id == 0)
                {
                    TasksDetailGridData = await sqlData.GetProductQueueTasks(-1, headerId);
                }
                else
                {
                    TasksDetailGridData = await sqlData.GetProductQueueTasks(id);
                }
            }
            if (gridName == "TasksGrid")
            {
                SqlData sqlData = new SqlData();
                TasksDetailGridData = await sqlData.GetProductQueueTasks(id, headerId);

                await InvokeAsync(StateHasChanged); // <-- refreshes
            }
        }

        public async Task LoadTestsGridData(SoEdiData row)
        {
            if (JsonConvert.SerializeObject(selectedRow) == JsonConvert.SerializeObject(row))
                return;

            List<int> productIds = new List<int>();

            SqlData sqlData = new SqlData();
            TestsMainGridData = await sqlData.GetProductTests(row.SOHeaderId, productIds);

            await InvokeAsync(StateHasChanged); // <-- refreshes
        }

        public void TestsMainGrid_CustomizeElement(GridCustomizeElementEventArgs e)
        {
            if (e.ElementType == GridElementType.DataRow)
            {
                string status = (System.String)e.Grid.GetRowValue(e.VisibleIndex, "TestStatus");

                if (status == "Passed")
                {
                    e.Style = "color: green";
                }
                if (status == "Failed")
                {
                    e.Style = "color: red";
                }
                if (status == "Pending")
                {
                    e.Style = "background: yellow";
                }
            }
        }

        #endregion

        // ============================================================ \\

        #region Linking Functions

        public async Task ToggleSearchPopupVisibilityAsync()
        {
            if (windowRefPOSearchVisible)
                await windowRefPOSearch.CloseAsync();
            else
                await windowRefPOSearch.ShowAtAsync(popupTarget);
        }
        public async Task ToggleLinkingPopupVisibilityAsync()
        {
            if (windowRefLinkingVisible)
                await windowRefLinking.CloseAsync();
            else
                await windowRefLinking.ShowAtAsync(popupTarget);
        }

        public async void LinkCustomerPOClick(SoEdiData row, string orderType = "SOOrder")
        {
            if (orderType == "SOOrder")
            {
                linkingRow = row;
                selectedSono = row.SONumber;
                searchTitleText = "Linking " + row.SONumber + " to...";
                linkingItemText = "";

                SqlData sqlData = new SqlData();
                //LinkSOOrderDetailGridData = await sqlData.GetCustomerSoPoDetailData(row.SOHeaderId, false);
                LinkSOOrderDetailGridData = await sqlData.GetSODetailSearchData(row.SOHeaderId, false);

                EdiOrderHeaderGridData = Enumerable.Empty<EdiOrderDetailData>();
                await ToggleSearchPopupVisibilityAsync();
            }
            if (orderType == "BlankOrder")
            {
                linkingRow = row;
                selectedSono = "BlankOrder";
                searchTitleText = "Linking Blank Order to...";
                linkingItemText = "";

                EdiOrderHeaderGridData = Enumerable.Empty<EdiOrderDetailData>();
                LinkSOOrderDetailGridData = Enumerable.Empty<CustomerSoPoData>();

                await ToggleSearchPopupVisibilityAsync();
            }
        }
        public async void OnSearchButtonClick(string poNo)
        {
            if (useSearchButton)
            {
                //if (string.IsNullOrEmpty(poNo))
                poNo = searchBoxRef.Text;

                SqlData sqlData = new SqlData();
                EdiOrderHeaderGridData = await sqlData.SearchEdiOrderSummaryViewData(poNo, true);

                if (!string.IsNullOrEmpty(poNo))
                {
                    if (EdiOrderHeaderGridData.Count() == 0)
                    {
                        DisplayPopupMessage("No EDI POs found for " + poNo);
                    }
                }
                await InvokeAsync(StateHasChanged); // <-- refreshes
            }
        }
        public async void OnSearchPOClick(EdiOrderDetailData row)
        {
            if (isBlankOrder)
            {
                selectedEdiRow = row;
                selectedEdiPO = row.PONumber;
                searchTitleText = "Linking " + selectedEdiRow.PONumber + " to " + selectedSono; //linkingRow.SONumber;

                SqlData sqlData = new SqlData();
                LinkEdiOrderDetailGridData = await sqlData.GetSoEdiDetailData(row.EdiHdrId, false, true);

                foreach (SoEdiData item in LinkEdiOrderDetailGridData)
                {
                    item.IsItemLinked = true;
                }

                string message = "Are you sure you want to " + Environment.NewLine + "link i2 SO " + selectedSono + " with EDI PO Number " + selectedEdiPO + "?";
                DisplayPopupQuestion(message, "Confirmation", "SaveBlankOrderLinks");
            }
            else
            {
                selectedEdiRow = row;
                selectedEdiPO = row.PONumber;
                searchTitleText = "Linking " + selectedEdiRow.PONumber + " to " + selectedSono; //linkingRow.SONumber;

                SqlData sqlData = new SqlData();
                LinkEdiOrderDetailGridData = await sqlData.GetSoEdiDetailData(row.EdiHdrId, false, true);

                await AutoSyncItemData();

                await ToggleSearchPopupVisibilityAsync();
                await ToggleLinkingPopupVisibilityAsync();

                await InvokeAsync(StateHasChanged); // <-- refreshes
            }
        }
        public async Task AutoSyncItemData()
        {
            if (LinkSOOrderDetailGridData != null &&
                LinkEdiOrderDetailGridData != null)
            {
                string soItemNo = "";
                string ediItemNo = "";
                int ediTrnId = 0;

                try
                {
                    foreach (CustomerSoPoData soItem in LinkSOOrderDetailGridData)
                    {
                        soItemNo = soItem.ProductNo;
                        ediItemNo = "";
                        ediTrnId = 0;

                        if (soItem.SOQty > 0)
                        {
                            foreach (SoEdiData ediItem in LinkEdiOrderDetailGridData)
                            {
                                if (ediItem.ItemNo == soItemNo)
                                {
                                    ediItemNo = ediItem.ItemNo;
                                    ediTrnId = ediItem.EdiTrnId;

                                    // Link
                                    ediItem.LinkedToId = soItem.SODetailId;
                                    ediItem.LinkedToName = soItem.ProductNo;
                                    ediItem.IsItemLinked = true;
                                    ediItem.LinkedStatus = "Auto-Linked to " + soItem.ProductNo;
                                    break;
                                }
                            }

                            if (ediTrnId > 0)
                            {
                                soItem.LinkedToId = ediTrnId;
                                soItem.LinkedToName = ediItemNo;
                                soItem.IsItemLinked = true;
                                soItem.LinkedStatus = "Auto-Linked to " + ediItemNo;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        public async Task OnLinkPOItemClick(object data, string process)
        {
            if (data.GetType() == typeof(SoEdiData))
            {
                if (process == "link")
                {
                    if ((data as SoEdiData).IsItemLinked)
                        return;

                    linkingSoEdiDataItem = (data as SoEdiData);

                    if (!isBlankOrder)
                    {
                        if (linkingCustomerSoPoDataItem == null)
                        {
                            linkingItemText = "Select SO Item";
                        }
                        else
                        {
                            linkingItemText = "Linking SO Item " + linkingCustomerSoPoDataItem.ProductNo + " to EDI Item " + linkingSoEdiDataItem.ItemNo;
                            await LinkPOItem(linkingSoEdiDataItem, linkingCustomerSoPoDataItem, process);
                        }
                    }
                    else
                    {
                        await AddLinkPOItem(linkingSoEdiDataItem, linkingCustomerSoPoDataItem, "add");
                    }
                }
                if (process == "unlink")
                {
                    if (!(data as SoEdiData).IsItemLinked)
                        return;

                    if (!isBlankOrder)
                    {
                        await LinkPOItem((data as SoEdiData), null, process);
                        linkingSoEdiDataItem = null;
                        linkingItemText = "";
                    }
                    else
                    {
                        await AddLinkPOItem((data as SoEdiData), null, "remove");
                    }
                }
            }
            if (data.GetType() == typeof(CustomerSoPoData))
            {
                if (process == "link")
                {
                    if ((data as CustomerSoPoData).IsItemLinked)
                        return;

                    linkingCustomerSoPoDataItem = (data as CustomerSoPoData);

                    if (linkingCustomerSoPoDataItem.SOQty == 0)
                    {
                        DisplayPopupMessage("Unable to Link. 0 SO Qty available.");
                        return;
                    }

                    if (linkingSoEdiDataItem == null)
                    {
                        linkingItemText = "Select EDI Item";
                    }
                    else
                    {
                        linkingItemText = "Linking EDI Item " + linkingSoEdiDataItem.ItemNo + " to SO Item " + linkingCustomerSoPoDataItem.ProductNo;
                        await LinkPOItem(linkingSoEdiDataItem, linkingCustomerSoPoDataItem, process);
                    }
                }
                if (process == "unlink")
                {
                    if (!(data as CustomerSoPoData).IsItemLinked)
                        return;

                    if (!isBlankOrder)
                    {
                        await LinkPOItem(null, (data as CustomerSoPoData), process);
                        linkingCustomerSoPoDataItem = null;
                        linkingItemText = "";
                    }
                    else
                    {
                        await AddLinkPOItem(null, (data as CustomerSoPoData), "remove");
                    }
                }
            }
        }

        public async Task LinkPOItem(SoEdiData ediData, CustomerSoPoData soPoData, string process)
        {
            if (process == "link")
            {
                foreach (SoEdiData item in LinkEdiOrderDetailGridData)
                {
                    if (item.EdiTrnId == ediData.EdiTrnId)
                    {
                        // Link
                        item.LinkedToId = soPoData.SODetailId;
                        item.LinkedToName = soPoData.ProductNo;
                        item.IsItemLinked = true;
                        item.LinkedStatus = "Linked to " + soPoData.ProductNo;
                        break;
                    }
                }
                foreach (CustomerSoPoData item in LinkSOOrderDetailGridData)
                {
                    if (item.SODetailId == soPoData.SODetailId)
                    {
                        // Link
                        item.LinkedToId = ediData.EdiTrnId;
                        item.LinkedToName = ediData.ItemNo;
                        item.IsItemLinked = true;
                        item.LinkedStatus = "Linked to " + ediData.ItemNo;
                        break;
                    }
                }

                linkingSoEdiDataItem = null;
                linkingCustomerSoPoDataItem = null;
                linkingItemText = "";
            }
            if (process == "unlink")
            {
                if (ediData != null)
                {
                    int linkedToId = 0;

                    foreach (SoEdiData item in LinkEdiOrderDetailGridData)
                    {
                        if (item.EdiTrnId == ediData.EdiTrnId)
                        {
                            linkedToId = item.LinkedToId;

                            // Un-Link
                            item.LinkedToId = 0;
                            item.LinkedToName = "";
                            item.IsItemLinked = false;
                            item.LinkedStatus = "";
                            break;
                        }
                    }
                    if (linkedToId > 0)
                    {
                        foreach (CustomerSoPoData item in LinkSOOrderDetailGridData)
                        {
                            if (item.SODetailId == linkedToId)
                            {
                                // Un-Link
                                item.LinkedToId = 0;
                                item.LinkedToName = "";
                                item.IsItemLinked = false;
                                item.LinkedStatus = "";
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (soPoData != null)
                    {
                        int linkedToId = 0;

                        foreach (CustomerSoPoData item in LinkSOOrderDetailGridData)
                        {
                            if (item.SODetailId == soPoData.SODetailId)
                            {
                                linkedToId = item.LinkedToId;

                                // Un-Link
                                item.LinkedToId = 0;
                                item.LinkedToName = "";
                                item.IsItemLinked = false;
                                item.LinkedStatus = "";
                                break;
                            }
                        }

                        if (linkedToId > 0)
                        {
                            foreach (SoEdiData item in LinkEdiOrderDetailGridData)
                            {
                                if (item.EdiTrnId == linkedToId)
                                {
                                    // Un-Link
                                    item.LinkedToId = 0;
                                    item.LinkedToName = "";
                                    item.IsItemLinked = false;
                                    item.LinkedStatus = "";
                                    break;
                                }
                            }
                        }
                    }
                }

                linkingSoEdiDataItem = null;
                linkingCustomerSoPoDataItem = null;
                linkingItemText = "";
            }
        }

        public async Task AddLinkPOItem(SoEdiData ediData, CustomerSoPoData soPoData, string process)
        {
            if (process == "add")
            {
                CustomerSoPoData poData = new CustomerSoPoData();
                poData.SODetailId = (LinkSOOrderDetailGridData.Count() == 0) ? 1 : LinkSOOrderDetailGridData.Where(x => x.SODetailId > 0).Select(x => x.SODetailId).Max() + 1;
                poData.ProductNo = ediData.ItemNo;
                poData.SOQty = 0;
                poData.VendorPO = "";
                poData.VendorName = "";
                poData.LinkedToId = ediData.EdiTrnId;
                poData.LinkedToName = ediData.ItemNo;
                poData.IsItemLinked = true;
                poData.LinkedStatus = "Linked to " + ediData.ItemNo;

                //LinkSOOrderDetailGridData.Append<CustomerSoPoData>(poData);
                //LinkSOOrderDetailGridData.Concat(new[] { poData });
                List<CustomerSoPoData> newList = LinkSOOrderDetailGridData.ToList();
                newList.Add(poData);
                LinkSOOrderDetailGridData = newList;

                foreach (SoEdiData item in LinkEdiOrderDetailGridData)
                {
                    if (item.EdiTrnId == ediData.EdiTrnId)
                    {
                        // Link
                        item.LinkedToId = poData.SODetailId;
                        item.LinkedToName = poData.ProductNo;
                        item.IsItemLinked = true;
                        item.LinkedStatus = "Linked to " + poData.ProductNo;
                        break;
                    }
                }

                linkingSoEdiDataItem = null;
                linkingItemText = "";
            }
            if (process == "remove")
            {
                if (ediData != null)
                {
                    int linkedToId = 0;

                    foreach (SoEdiData item in LinkEdiOrderDetailGridData)
                    {
                        if (item.EdiTrnId == ediData.EdiTrnId)
                        {
                            linkedToId = item.LinkedToId;

                            // Un-Link
                            item.LinkedToId = 0;
                            item.LinkedToName = "";
                            item.IsItemLinked = false;
                            item.LinkedStatus = "";
                            break;
                        }
                    }
                    if (linkedToId > 0)
                    {
                        // find and remove LinkSOOrderDetailGridData item
                        int index = 0;
                        foreach (CustomerSoPoData item in LinkSOOrderDetailGridData)
                        {
                            if (item.SODetailId == linkedToId)
                            {
                                // remove
                                //LinkSOOrderDetailGridData = LinkSOOrderDetailGridData.Skip(index);

                                List<CustomerSoPoData> newList = LinkSOOrderDetailGridData.ToList();
                                newList.Remove(item);
                                LinkSOOrderDetailGridData = newList;
                                break;
                            }
                            index++;
                        }
                    }
                }
                else
                {
                    if (soPoData != null)
                    {
                        int linkedToId = 0;
                        int index = 0;
                        foreach (CustomerSoPoData item in LinkSOOrderDetailGridData)
                        {
                            if (item.SODetailId == soPoData.SODetailId)
                            {
                                linkedToId = item.LinkedToId;

                                // remove
                                //if (LinkSOOrderDetailGridData.Count() == 1)
                                //    LinkSOOrderDetailGridData = Enumerable.Empty<CustomerSoPoData>();
                                //else
                                //    LinkSOOrderDetailGridData = LinkSOOrderDetailGridData.Skip(index);

                                List<CustomerSoPoData> newList = LinkSOOrderDetailGridData.ToList();
                                newList.Remove(item);
                                LinkSOOrderDetailGridData = newList;

                                break;
                            }
                            index++;
                        }

                        if (linkedToId > 0)
                        {
                            foreach (SoEdiData item in LinkEdiOrderDetailGridData)
                            {
                                if (item.EdiTrnId == linkedToId)
                                {
                                    // Un-Link
                                    item.LinkedToId = 0;
                                    item.LinkedToName = "";
                                    item.IsItemLinked = false;
                                    item.LinkedStatus = "";
                                    break;
                                }
                            }
                        }
                    }
                }

                linkingSoEdiDataItem = null;
                linkingItemText = "";
            }
        }

        public async Task OnLinkinkPopupOkClick()
        {
            string message = "Are you sure you want to " + Environment.NewLine + "link i2 SO " + selectedSono + " with EDI PO Number " + selectedEdiPO + "?";
            DisplayPopupQuestion(message, "Confirmation", "SaveLinks");
        }

        public async Task SaveCustomerPOLinking()
        {
            // Note: if 'Auto Forecast' order, transfer [Description] and [Comments] to [PIMS].[edi].[EdiHdr]

            if (!isBlankOrder)
            {
                int edihdrid = selectedEdiRow.EdiHdrId;
                string query = "";
                string fullQuery = "";

                query = @"
UPDATE [PIMS].[edi].[EdiHdr] SET [SoHeaderId] = {2} WHERE [Edihdrid] = {0}
UPDATE [PIMS].[edi].[EdiTrn] SET [SoDetailId] = {3}, [ProductId] = {4} WHERE [Editrnid] = {1} AND [Edihdrid] = {0}";
                foreach (CustomerSoPoData item in LinkSOOrderDetailGridData)
                {
                    if (item.IsItemLinked) //item.LinkedToId = ediData.EdiTrnId;
                    {
                        fullQuery += string.Format(query, edihdrid, item.LinkedToId, item.SOHeaderId, item.SODetailId, item.ProductId);
                    }
                }

                //DisplayPopupMessage("Linking");

                try
                {
                    using (var uow = new UnitOfWork())
                    {
                        await uow.ExecuteNonQueryAsync(fullQuery);
                    }
                }
                catch (Exception ex)
                {
                    DisplayPopupMessage("Error Saving:" + ex.Message);
                }

                //TogglePopupVisibilityAsync()
                await windowRefLinking.CloseAsync();

                SqlData sqlData = new SqlData();
                OrdersHeaderGridData = await sqlData.GetSoEdiData();

                await InvokeAsync(StateHasChanged); // <-- refreshes
            }
            else
            {
                //linkingRow(SoEdiData) row clicked on [main row]
                //selectedEdiRow(EdiOrderDetailData) search EDI
                //LinkEdiOrderDetailGridData(IEnumerable < CustomerSoPoData > LinkSOOrderDetailGridData) EDI detail data

                int edihdrid = selectedEdiRow.EdiHdrId;
                int soHeaderId = linkingRow.SOHeaderId; // > 1000000
                string soDesc = linkingRow.Description;
                string soComments = linkingRow.Comments;
                string soQBSO = linkingRow.QBSO;

                string query = "";
                string fullQuery = "";

                query = @"
UPDATE [PIMS].[edi].[EdiHdr]
SET [SoHeaderId] = {1}
    ,[Description] = '{2}'
    ,[Comments] = '{3}'
    ,[QBSO] = '{4}'
WHERE [Edihdrid] = {0}";
                fullQuery = string.Format(query, edihdrid, soHeaderId, soDesc, soComments, soQBSO);

                query = @"
UPDATE [PIMS].[edi].[EdiTrn]
SET [SoDetailId] = 0
    ,[ProductId] = (SELECT ISNULL([ProductId],0) AS [ProductId] FROM [PIMS].[dbo].[Product] WHERE [ProductNo] = '{2}')
WHERE [Editrnid] = {1} AND [Edihdrid] = {0}";
                foreach (SoEdiData item in LinkEdiOrderDetailGridData)
                {
                    if (item.IsItemLinked) //item.LinkedToId = ediData.EdiTrnId;
                    {
                        fullQuery += string.Format(query, edihdrid, item.EdiTrnId, item.ItemNo);
                    }
                }

                try
                {
                    using (var uow = new UnitOfWork())
                    {
                        await uow.ExecuteNonQueryAsync(fullQuery);
                    }
                }
                catch (Exception ex)
                {
                    DisplayPopupMessage("Error Saving:" + ex.Message);
                }

                //TogglePopupVisibilityAsync()
                await windowRefLinking.CloseAsync();

                SqlData sqlData = new SqlData();
                OrdersHeaderGridData = await sqlData.GetSoEdiData();

                await InvokeAsync(StateHasChanged); // <-- refreshes
            }
        }

        public async Task OnUnLinkPOItemClick(SoEdiData ediData)
        {
            if (string.IsNullOrWhiteSpace(ediData.CustomerPO))
                return;

            string selectedSono = ediData.SONumber;
            string selectedEdiPO = ediData.CustomerPO;

            unLinkingRow = ediData;

            //int ediHdrId = ediData.EdiHdrId;
            //int soHeaderId = ediData.SOHeaderId;
            //int ediTrnId = ediData.EdiTrnId;
            //int soDetailId = ediData.SoDetailId;
            //int productId = ediData.ProductId;

            string message = "Are you sure you want to " + Environment.NewLine + "UNLINK i2 SO " + selectedSono + " with EDI PO Number " + selectedEdiPO + "?";
            DisplayPopupQuestion(message, "Confirmation", "UnLinkSave");
        }

        public async Task OnUnLinkPOItemSave(SoEdiData ediData)
        {
            int ediHdrId = ediData.EdiHdrId;
            int soHeaderId = ediData.SOHeaderId;

            int ediTrnId = ediData.EdiTrnId;
            int soDetailId = ediData.SoDetailId;
            int productId = ediData.ProductId;

            string query = "";
            string fullQuery = "";

            query = @"UPDATE [PIMS].[edi].[EdiTrn] SET [SoDetailId] = NULL, [ProductId] = NULL WHERE [Editrnid] = {1} AND [Edihdrid] = {0}
IF (SELECT COUNT(*) FROM [PIMS].[edi].[EdiTrn] WHERE [Edihdrid] = {0}) = 0
BEGIN
	UPDATE [PIMS].[edi].[EdiHdr] SET [SoHeaderId] = NULL WHERE [Edihdrid] = {0}
END";

            fullQuery += string.Format(query, ediHdrId, ediTrnId);
            try
            {
                using (var uow = new UnitOfWork())
                {
                    await uow.ExecuteNonQueryAsync(fullQuery);
                }
            }
            catch (Exception ex)
            {
                DisplayPopupMessage("Error Saving:" + ex.Message);
            }

            SqlData sqlData = new SqlData();
            OrdersHeaderGridData = await sqlData.GetSoEdiData();
            await InvokeAsync(StateHasChanged); // <-- refreshes
        }

        #endregion

        // ============================================================ \\

        #region Non-Grid Functions

        public List<Lookup> GetPartnerData()
        {
            List<Lookup> list = new List<Lookup>();

            //var sublist = OpenPOShipmentData.Where(x => x.VendorId == vendorId).Select(x => x.PONumber).Distinct().ToList();
            foreach (BusinessPartner item in BusinessPartnerListData.AsEnumerable())
            {
                Lookup l = new Lookup()
                {
                    LookupText = item.BusinessPartnerName,
                    LookupValue = item.BusinessPartnerId
                };
                list.Add(l);
            }
            return list;
        }

        public async void LinkCustomerPOClick(CustomerSoPoData row)
        {
            //linkingRow = row;
            //searchTitleText = "Linking " + row.ProductNo + " to...";
            //await TogglePopupVisibilityAsync();
        }
        public async void LinkCustomerPOClick(EdiOrderDetailData row)
        {
            //selectedEdiDetailRow = row; // Line Items
            //
            //// Question User
            //linkingEdiPOtoSO = true;
            //
            //string message = "Are you sure you want to " + Environment.NewLine + "link i2 SO " + selectedRow.SONumber + ", Product " + selectedRow.ProductNo + Environment.NewLine + " with EDI PO Number " + row.PONumber + "m Product No " + row.ProductNo + "?";
            //DisplayPopupQuestion(message);
        }

        public async void OnSearchPOTextChanged(string poNo)
        {
            if (!useSearchButton)
            {
                SqlData sqlData = new SqlData();
                EdiOrderHeaderGridData = await sqlData.SearchEdiOrderSummaryViewData(poNo, true);
                // EdiOrderDetailGridData = await sqlData.GetEdiOrderDetailViewData(newVpoNoalue);

                if (!string.IsNullOrEmpty(poNo))
                {
                    if (EdiOrderHeaderGridData.Count() == 0)
                    {
                        DisplayPopupMessage("No EDI POs found for " + poNo);
                    }
                }
                await InvokeAsync(StateHasChanged); // <-- refreshes
            }
        }

        public async void LoadEdiOrderDetailGridData(EdiOrderDetailData row) //string poNo
        {
            if (useSearchButton)
            {
                //if (selectedEdiRow != null && selectedEdiRow.PONumber == row.PONumber)
                if (JsonConvert.SerializeObject(selectedEdiRow) == JsonConvert.SerializeObject(row))
                    return;

                SqlData sqlData = new SqlData();
                EdiOrderDetailGridData = await sqlData.GetEdiOrderDetailViewData(row.PONumber);

                await InvokeAsync(StateHasChanged); // <-- refreshes
            }
        }

        #endregion

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