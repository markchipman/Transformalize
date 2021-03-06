﻿$(document).ready(function () {

    $(document).ajaxStart(function () {
        $.blockUI({
            css: {
                message: null,
                border: 'none',
                padding: '15px',
                backgroundColor: '#000',
                '-webkit-border-radius': '10px',
                '-moz-border-radius': '10px',
                opacity: .5,
                color: '#fff',
                baseZ: 1021
            }
        });
    });

    $(document).ajaxComplete(function () {
        $.unblockUI();
    });

    $(document).ajaxError(function (event, request) {
        $.unblockUI();
        $().toastmessage('showErrorToast', request.responseText);
    });

});
