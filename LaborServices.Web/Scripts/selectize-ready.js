$(function () {
    $.each($(".autocomplete"),
           function (i, v) {
               if ($(this).is("select") && ($(this).attr("class").indexOf("selectized") == -1)) {
                   $(this).selectize({ selectOnTab: true });
               }
           });
});
