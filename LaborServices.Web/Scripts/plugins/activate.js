function activate(url, checkbox, itemId) {
    $.ajax({
        type: "POST",
        url: url,
        dataType: "json",
        data: { isChecked: checkbox.checked, id: itemId }
    });
};