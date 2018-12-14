(function (root, factory) {
    'use strict';
    if (typeof define === 'function' && define.amd) {
        // use AMD define funtion to support AMD modules if in use
        define(['exports'], factory);

    } else if (typeof exports === 'object') {
        // to support CommonJS
        factory(exports);
    }
    var RemoveFromCompareForm = {}
    root.RemoveFromCompareForm = RemoveFromCompareForm;
    factory(RemoveFromCompareForm);

}(this, function (element) {

    RemoveFromCompareForm.OnSuccess = function (data) {
        if (data.Success) {
            $('#pc-'+data.RemovedSellableItemId).remove();
        }
    }

    RemoveFromCompareForm.OnFailure = function (data) {
        if (data && !data.Success) {
            alert('failure');
        }     
    }

    RemoveFromCompareForm.Init = function (element) {
        var form = new CXAForm(element);
        form.Init(AddToCompareForm);

        //Only enable form if we are not in experience editor mode
        if (CXAApplication.IsExperienceEditorMode() === false) {
            form.Enable();
        }
        else {
            form.EnableInDesignEditing();
        }
    }

    RemoveFromCompareForm.SetAddButton = function () {
        $(document).ready(function () {
            MessageContext.ClearAllMessages();
            $(".add-to-compare-btn").button('loading');
        });
    }
}));