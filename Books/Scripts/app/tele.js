function showAlertMessage(alert, message) {
    if (message) {
        var $alert = alert;
        $alert.find("span").replaceWith("<span>" + message + "</span>");
        $alert.show().delay(3000).hide(1000);
    }
}

function DisplayAlertErrors(formId, errors, alert) {
    var $alertError = alert;
    var $ul = $alertError.find('ul');
    $ul.empty();
    $('#' + formId + ' [data-valmsg-for').text("");

    if (errors != undefined && errors.length > 0) {
        for (var i = 0; i < errors.length; i++) {
            if (errors[i].Key === "") { //error model or error save 
                $ul.append("<li>" + errors[i].Value[0] + "</li>");
            } else { // error properties
                $('#' + formId + ' [data-valmsg-for="' + errors[i].Key + '"').text(errors[i].Value[0]);
            }
        }
    }

    if ($ul.children().length)
        $alertError.show();
}