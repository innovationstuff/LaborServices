
//list
function ConfirmDelete(id) { $("#DeletedID").val(id); $("#deleteModel").modal(); }
function Success(data) { $("#deleteModel").modal('hide'); $($("#" + data).closest("tr")).slideUp(); }

//create
function success() {
    var allInpts = $("#getForm textarea,#getForm input[type=text] , #getForm input[type=file]");
    $("#divCreate").slideUp(1000);
    allInpts.val('');
    $("#getAllList").click();
}


$(document).ready(function () {
    // Start Global

    //End  Global

    // strat List View
    $("#btnDelete").click(function () {
        $("#deleteModel").modal('hide'); $($("#" + $("#DeletedID").val()).closest("tr")).slideUp();
    });
    // End  List View


    // Start Create View
    var allInpts = $("#getForm textarea,#getForm input[type=text] , #getForm input[type=file]");
    $("#btnSave").click(function (e) {
        //e.preventDefault();
        var bl = true;
        allInpts.removeClass("redBorder");
        allInpts.each(function () {
            if ($(this).val().trim() == "") { $(this).focus().addClass("redBorder"); bl = false; return false; }
        });
        return bl;
    });

    var fileUpload = $("#getForm input[name=ImgFile]");
    fileUpload.change(function () {
        if ($(this).val().trim() != "") {
            var frmData = new FormData();
            var ImgUrlField = $(".ImgUrltxt");
            frmData.append("ImgFile", $(this)[0].files[0]);
            frmData.append("OldImg", ImgUrlField.val());
            frmData.append("operation", "0");
            SaveImg(frmData, $("#getForm .ImgUrltxt"));
        }
    });
    //End Create View

    //Start Edit View
    $("#btnSaveEditing").click(function () {
        $("#closeModal").click();
        var trs = $("#tr" + $(".EditingIDClass").val().trim()), i = -1;
        var frmData = $("#Edit .form-control");
        $(trs).find('td').each(function (column, td) {
            i++;
            if (i > 0 && i < 5) $(td).text($(frmData[i - 1]).val());
        });
    });

    var EditImg = $("#EditImg");
    EditImg.change(function () {
        if ($(this).val().trim() != "") {
            var frmData = new FormData();
            frmData.append("ImgFile", $(this)[0].files[0]);
            frmData.append("OldImg", "");
            frmData.append("operation", "1");
            SaveImg(frmData, $("#divEditing .ImgUrltxt"));
        }
    });
    //End Edit View

    // functions
    function SaveImg(frmData, ImgValue){
        
        $.ajax({
            url: "SaveImage",
            method: "post",
            cache: false,
            contentType: false,
            processData: false,
            data: frmData,
            success: function (data) {
                ImgValue.attr("value", data);
            },
            error: function (e) {

            }
        });
    }
});