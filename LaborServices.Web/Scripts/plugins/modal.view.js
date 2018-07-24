var searchForm;

function showModal(href) {
    $('#modalContent').load(href, function () {
        $("#modalDialog").removeClass("modal-lg");
        $('#modal').modal({
            keyboard: true
        }, 'show');
        bindForm(this);
        fakeLoaderFadeOut();
    });
}

function showLargeModal(href) {
    $('#modalContent').load(href, function () {
        $("#modalDialog").addClass("modal-lg");
        $('#modal').modal({
            keyboard: true
        }, 'show');
        bindForm(this);
    });
}

$(document).on("click", ".modal-link", function () {
    fakeLoaderFadeIn();
    showModal(this.href);
    searchForm = $($(this).parents("div[class='block full']")).find("form");
    return false;
});

$(document).on("click", ".modal-lg-link", function () {
    showLargeModal(this.href);
    searchForm = $($(this).parents("div[class='block full']")).find("form");
    return false;
});

function bindForm(dialog) {
    $('form', dialog).submit(function () {
        var formData = new FormData($(this)[0]);
        var $form = $(this);
        if (!$form.valid()) return false;

        $.ajax({
            url: this.action,
            type: this.method,
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            success: function (result) {
                if (result.success) {
                    $('#fakeLoader').fadeOut();

                    if (result.createAnother == true) {
                        $(':input', $form)
                          .not(':button, :submit, :reset, :hidden, #CreateAnother')
                          .val('')
                          .removeAttr('checked')
                          .removeAttr('selected');

                        var selectizes = $('select', $form);

                        $.each(selectizes, function (index, value) {
                            var selectize = $(value)[0].selectize;
                            selectize.clear();
                        });
                    } else {
                        if (result.displayMessage != undefined) {
                          
                            $("#page-content").find(".block:first").prepend(result.displayMessage);
                        }
                        $('#modal').modal('hide');

                        if (result.returnUrl != undefined) {
                            location.replace(result.returnUrl);
                        }
                        else {
                            if (searchForm.length > 0) searchForm.submit();
                            else location.reload(false);
                        }
                    }       
                } else {
                    $('#modalContent').html(result);
                    $('#fakeLoader').fadeOut();
                    bindForm();
                }

                $("#fakeLoader").replaceWith('<div id="fakeLoader"></div>');
            }
        });
        return false;
    });
}



// second bootsratp modal
// ======================

$(document).on("click", ".modal-link2", function () {
    showModal2(this.href);
    searchForm = $($(this).parents("div[class='block full']")).find("form");
    return false;
});


function showModal2(href) {
    $('#modalContent2').load(href, function () {
        $("#modalDialog2").removeClass("modal-lg");
        $('#modal2').modal({
            keyboard: true
        }, 'show');
        bindForm2(this);
    });
}

function bindForm2(dialog) {
    $('form', dialog).submit(function () {
        var formData = new FormData($(this)[0]);
        var $form = $(this);
        if (!$form.valid()) return false;

        $.ajax({
            url: this.action,
            type: this.method,
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            success: function (result) {
                if (result.success) {
                    $('#fakeLoader').fadeOut();

                    if (result.createAnother == true) {
                        $(':input', $form)
                          .not(':button, :submit, :reset, :hidden, #CreateAnother')
                          .val('')
                          .removeAttr('checked')
                          .removeAttr('selected');

                        var selectizes = $('select', $form);

                        $.each(selectizes, function (index, value) {
                            var selectize = $(value)[0].selectize;
                            selectize.clear();
                        });
                    } else {
                        if (result.displayMessage != undefined) {

                            $("#page-content").find(".block:first").prepend(result.displayMessage);
                        }
                        $('#modal2').modal('hide');

                        if (result.returnUrl != undefined) {
                            location.replace(result.returnUrl);
                        }
                        else {
                            if (searchForm.length > 0) searchForm.submit();
                            else location.reload(false);
                        }
                    }
                } else {
                    $('#modalContent2').html(result);
                    $('#fakeLoader').fadeOut();
                    bindForm();
                }

                $("#fakeLoader").replaceWith('<div id="fakeLoader"></div>');
            }
        });
        return false;
    });
}