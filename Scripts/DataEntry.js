

$(function () {
    //$('.date-withicon').datepicker({
    //    format: 'mm-dd-yyyy'
    //});
    $('#PurchaseDate').datepicker({
        dateFormat: "mm-dd-yy",
        changeMonth: true,
        changeYear: true,
        pickerClass: 'form-control '
    });
 


    //*************************** Grid  creation ***************************//
    var dataEntryGrid = { location: "local" };
    var colEntryGrid = [
        { title: "", dataIndx: "Purchase_Id", dataType: "integer", hidden: true },
        { title: "", dataIndx: "Partner_Id", dataType: "integer", hidden: true },

        { title: "Purchase Date", dataIndx: "StrPurchase_Date", editable: false, filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] } },
        { title: "Partner Name", dataIndx: "Partner_Name", editable: false, filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] } },
        { title: "Amount", dataIndx: "Amount", dataType: "float", filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] } },
        {
            title: "", editable: false, minWidth: 90, maxWidth: 100, sortable: false, align: "center",
            render: function (ui) {
                let PurchaseId = ui.rowData.Purchase_Id;
                var renderButton = '<button type="button" class="btn btn-primary" onclick="UpdateEntries(' + PurchaseId + ');" title="Update"></i> Update</button>';
                return renderButton;
            }

        },
        {
            title: "", editable: false, minWidth: 90, maxWidth: 100, sortable: false, align: "center",
            render: function (ui) {
                let PurchaseId = ui.rowData.Purchase_Id
                var renderButton = '<button type="button" class="btn btn-primary" onclick="DeleteEntries(' + PurchaseId + ');" title="Delete"></i> Delete</button>';
                return renderButton;

            }
        }
    ];
    var gridObject = {
        sortable: false,
        numberCell: { show: true },
        hoverMode: 'cell',
        showTop: true,
        showBottom: true,
        resizable: true,
        filterModel: { on: true, mode: "AND", header: true },
        selectionModel: { type: 'row' },
        scrollModel: { autoFit: true, flexContent: true },
        pageModel: { type: "local", rPP: 10 },
        title: 'Purchase Entries',
        width: '100%',
        height: 500,
        colModel: colEntryGrid,
        dataModel: dataEntryGrid,
        editable: true
    }
    $("#FinancialEntryGrid").pqGrid(gridObject);

    var dataComGrid = { location: "local" };
    var colComGrid = [
        { title: "", dataIndx: "Partner_Id", dataType: "integer", hidden: true },
        { title: "Partner Name", dataIndx: "Partner_Name", editable: false, filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] } },
        { title: "Team Amount", dataIndx: "TeamShoppingAmount", dataType: "float", filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] } },
        { title: "Total Amount", dataIndx: "TotalShoppingAmount", dataType: "float", filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] } },
        { title: "Comission", dataIndx: "TotalCommissionAmount", dataType: "float", filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] } },

    ];
    var gridComObject = {
        sortable: false,
        numberCell: { show: true },
        hoverMode: 'cell',
        showTop: true,
        showBottom: true,
        resizable: true,
        filterModel: { on: true, mode: "AND", header: true },
        selectionModel: { type: 'row' },
        scrollModel: { autoFit: true, flexContent: true },
        pageModel: { type: "local", rPP: 10 },
        title: 'Purchase Entries',
        width: '100%',
        height: 500,
        colModel: colComGrid,
        dataModel: dataComGrid,
        editable: true
    }
    $("#ComissionGrid").pqGrid(gridComObject);


    LoadPartner();
    LoadALLEntries();
});

//*************************** Load and CRUD Functions ***************************//
function LoadPartner() {
    $.ajax({
        type: "GET",
        traditional: true,
        url: "../DataEntry/GetAllPartners",
        success: function (response) {
            $('#ddlPartner').val("");
            $('#ddlPartner').html("");
            $('#ddlPartner').append('<option value="' + 0 + '">' + "Select" + '</option>');
            $.each(response, function (index, value) {
                $('#ddlPartner').append('<option value="' + value.Partner_Id + '">' + value.Partner_Name + '</option>');
            });
        }
    });
}

function LoadALLEntries() {
    $("#FinancialEntryGrid").pqGrid("showLoading");
    $.ajax({
        type: "GET",
        traditional: true,
        url: "../DataEntry/GetAllPurchaseEntries",
        success: function (response) {
            $("#FinancialEntryGrid").pqGrid("option", "dataModel.data", response);
            $("#FinancialEntryGrid").pqGrid("refreshDataAndView");
            $(".pq-grid-footer .ui-icon-refresh", $("#FinancialEntryGrid")).click();
            $("#FinancialEntryGrid").pqGrid("hideLoading");
            clearForm();
        }
    });
}

function LoadComissions(data) {
    $("#ComissionGrid").pqGrid("option", "dataModel.data", data);
    $("#ComissionGrid").pqGrid("refreshDataAndView");
    $(".pq-grid-footer .ui-icon-refresh", $("#ComissionGrid")).click();
    $("#ComissionGrid").pqGrid("hideLoading");
    clearForm();
}

function createUpdateFinancialEntry() {

    if ($("#ddlPartner").val() == 0) {
        alert("Partner is missing! Please enter Partner");
        return;
    }

    if ($("#PurchaseDate").val() == "") {
        alert("Date is missing! Please enter Date");
        return;
    }

    if ($("#Amount").val() == "") {
        alert("Amount is missing! Please enter Amount");
        return;
    }
    if (parseFloat($("#Amount").val()) === 0) {
        alert("Amount Shoudl be greater then 0");
        return;
    }
    var details = JSON.stringify({
        Purchase_Id: $("#PurchaseId").val() ? $("#PurchaseId").val() : 0,
        Partner_Id: $("#ddlPartner").val(),
        Amount: $("#Amount").val(),
        Purchase_Date: $('#PurchaseDate').val()
    });
    $.ajax({
        type: "POST", //HTTP POST Method
        traditional: true,
        contentType: 'application/json; charset=utf-8',
        url: '../DataEntry/CreateUpdateFinancialEntry',
        data: details,
        success: function (msg) {
            if (msg.success) {
                alert(msg.message);
                LoadALLEntries();
            }
            else {
                alert(msg.message);
            }
        },
        error: function (jqXhr, exception) {
            alert("Something went wrong! please Contact to Administrator");
        }


    });
}

function UpdateEntries(PurchaseId, PartnerId, Amount, StrPurchase_Date) {
    var Purchasedata = $("#FinancialEntryGrid").pqGrid("option", "dataModel.data")
    let filterPurchasedata = $.grep(Purchasedata, function (item) { return item.Purchase_Id == PurchaseId })[0]
    $("#PurchaseId").val(filterPurchasedata.Purchase_Id);
    $("#ddlPartner").val(filterPurchasedata.Partner_Id);
    $("#Amount").val(filterPurchasedata.Amount);
    $('#PurchaseDate').val(filterPurchasedata.StrPurchase_Date)
}

function DeleteEntries(purchasId) {

    $.ajax({
        type: "POST", //HTTP POST Method
        traditional: true,
        url: '../DataEntry/DeleteFinancialEntry',
        data: { PurchaseId: purchasId },
        success: function (msg) {
            if (msg.success) {
                alert(msg.message);
                LoadALLEntries();
            }
            else {
                alert(msg.message);
            }
        },
        error: function (jqXhr, exception) {
            alert("Something went wrong! please Contact to Administrator");
        }
    });
}

function clearForm() {
    $("#PurchaseId").val('');
    $('#ddlPartner').val(0);
    $("#Amount").val('');
    $('#PurchaseDate').val('')
}

function isNumeric() {
    //48 to 57 key code 0 to 9, 8 is backspace , 190 is dot , 46 is delete
    if ((event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105) && event.keyCode != 8 && event.keyCode != 190 && event.keyCode != 110 && event.keyCode != 46) {
        event.keyCode = 0;
        return false;
    }
}

//*************************** Buton Click Function ***************************//

$("#btnSave").click(function () {
    createUpdateFinancialEntry()
});

$("#btnReset").click(function () {
    clearForm()
});

$("#btnCalculation").click(function () {
    $("#ComissionGrid").pqGrid("showLoading");
    $.ajax({
        type: "GET",
        traditional: true,
        url: "../DataEntry/GetAllPartnersCommission",
        success: function (msg) {
            if (msg.success) {
                alert(msg.message);
                LoadComissions(msg.Comissions)
            }
            else {
                alert(msg.message);
            }
        },
        error: function (jqXhr, exception) {
            alert("Something went wrong! please Contact to Administrator");
        }
    });
});

