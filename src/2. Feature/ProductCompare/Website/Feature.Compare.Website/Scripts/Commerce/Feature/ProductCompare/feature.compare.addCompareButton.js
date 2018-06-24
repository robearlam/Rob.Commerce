(function (root, factory) {
    'use strict';
    if (typeof define === 'function' && define.amd) {
        // use AMD define funtion to support AMD modules if in use
        define(['exports'], factory);

    } else if (typeof exports === 'object') {
        // to support CommonJS
        factory(exports);
    }
    var AddToCompareForm = {}
    root.AddToCompareForm = AddToCompareForm;
    factory(AddToCompareForm);

}(this, function (element) {

    AddToCompareForm.OnSuccess = function (data) {
        if (data.Success) {
            $(".add-product-to-compare").hide();
            $(".view-product-compare").show();
        }

        // Update Button State
        $('.add-to-compare-btn').button('reset');
    }

    AddToCompareForm.OnFailure = function (data) {
        if (data && !data.Success) {
        }

        // Update Button State
        $('.add-to-compare-btn').button('reset');
    }

    AddToCompareForm.Init = function (element) {
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

    AddToCompareForm.SetAddButton = function () {
        $(document).ready(function () {
            MessageContext.ClearAllMessages();
            $(".add-to-compare-btn").button('loading');
        });
    }
}));