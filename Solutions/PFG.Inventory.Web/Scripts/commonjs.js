/* 原型繫結
* ============================================================ */
String.prototype.trim = function () {
    return this.replace(/(^\s*)|(\s*$)/g, "");
}

String.prototype.startWith = function (prefix) {
    return (this.substr(0, prefix.length) === prefix);
}

Array.prototype.last = function () { return this[this.length - 1]; }

//判斷字串是否包含指定的字
//回傳: bool
String.prototype.contains = function (txt) {
    return (this.indexOf(txt) >= 0);
}


//拓展方法
$.extend({
    Alert: function (msg, callback, title, isError) {
        var isError = isError || false;
        var classIcon = isError ? ' icon-ban-circle red' : 'icon-warning-sign yellow';
        var title_text = title ? title : "系統訊息";
        var title_template = '<h4 class="modal-title"><i class="' + classIcon + '"></i>' + title_text + '</h4>';
        var btn_title = $("<button><span aria-hidden='true'>&times;</span><span class='sr-only'>Close</span></button>").addClass("close").attr({ "data-dismiss": "modal" });
        var headerArea = $("<div class='modal-header'></div>").append(btn_title).append(title_template);
        var bodyArea = $("<div class='modal-body'></div>").html(msg);
        var footArea = $("<div class='modal-footer'></div>");
        var btn_OK = $("<button>確定</button>").addClass("btn btn-primary").attr({ "data-dismiss": "modal" }).appendTo(footArea);
        if (callback != undefined) {
            btn_OK.click(function () {
                callback();
            });
        }


        var contentArea = $("<div id='alertDialog'><div class='modal-dialog'><div class='modal-content'></div></div></div>")
            .find(".modal-content").append(headerArea).append(bodyArea).append(footArea).end()
            .attr({ "tabindex": "-1", "role": "dialog", "aria-labelledby": "myModalLabel", "aria-hidden": "true" }).addClass("modal fade");
        contentArea.appendTo("body").modal('show');
        contentArea.bind('hidden.bs.modal', function () {
            $(this).data('bs.modal', null);
            console.log('hidden');
            $(this).remove();
        });




    },
    Confirm: function (msg, callback, title) {

        var title_text = title ? title : "系統訊息";
        var title_template = '<h4 class="modal-title">' + title_text + '</h4>';
        var btn_title = $("<button><span aria-hidden='true'>&times;</span><span class='sr-only'>Close</span></button>").addClass("close").attr({ "data-dismiss": "modal" });
        var headerArea = $("<div class='modal-header'></div>").append(btn_title).append(title_template);
        var bodyArea = $("<div class='modal-body'></div>").html(msg);
        var footArea = $("<div class='modal-footer'></div>");
        var btn_OK = $("<button>確定</button>").addClass("btn btn-primary").attr({ "data-dismiss": "modal" }).appendTo(footArea);
        var btn_Cancel = $("<button>取消</button>").addClass("btn").attr({ "data-dismiss": "modal" }).appendTo(footArea);
        if (callback != undefined) {
            btn_OK.click(function () {
                callback();
            });
        }


        var contentArea = $("<div id='confirmDialog'><div class='modal-dialog'><div class='modal-content'></div></div></div>")
            .find(".modal-content").append(headerArea).append(bodyArea).append(footArea).end()
            .attr({ "tabindex": "-1", "role": "dialog", "aria-labelledby": "myModalLabel", "aria-hidden": "true" }).addClass("modal fade");

        contentArea.appendTo("body").modal('show');
        contentArea.on('hidden.bs.modal', function () {
            $(this).data('bs.modal', null);
            $(this).remove();
        });



    },
    PopDialog: function (msg, title, width) {
        var modalWidth = 650;
        if (width != undefined) modalWidth = width;

        var title_text = title ? title : "系統訊息";
        var title_template = '<h4 class="modal-title">' + title_text + '</h4>';
        var btn_title = $("<button><span aria-hidden='true'>&times;</span><span class='sr-only'>Close</span></button>").addClass("close").attr({ "data-dismiss": "modal" });
        var headerArea = $("<div class='modal-header'></div>").append(btn_title).append(title_template);
        var bodyArea = $("<div class='modal-body'></div>").html(msg);
        var footArea = $("<div class='modal-footer'></div>");
        var btn_OK = $("<button>確定</button>").addClass("btn btn-primary").attr({ "data-dismiss": "modal" }).appendTo(footArea);

        var contentArea = $("<div id='popDialog'><div class='modal-dialog' style='width:" + width + "px'><div class='modal-content'></div></div></div>")
            .find(".modal-content").append(headerArea).append(bodyArea).append(footArea).end()
            .attr({ "tabindex": "-1", "role": "dialog", "aria-labelledby": "myModalLabel", "aria-hidden": "true" }).addClass("modal fade");
        contentArea.appendTo("body").modal('show');
        contentArea.bind('hidden.bs.modal', function () {
            $(this).data('bs.modal', null);
            console.log('hidden');
            $(this).remove();
        });




    },
    ShowLoading: function () {
        $('#loading').fadeIn();
    },
    HideLoading: function () {
        $('#loading').fadeOut();
    },
    ShowGridLoading: function (divId) {
        var template = $("<div id='grid-loading' style='z-index: 2000;' class='ajax-loading-overlay'><i class='ajax-loading-icon fa fa-spin fa-spinner fa-2x blue'></i> <span class='blue'> 讀取中... </span></div>");
        template.appendTo('#' + divId).fadeIn();
    },
    HideGridLoading: function () {
        //$("#grid-loading").fadeOut();
    }
});

$.fn.extend({
    fullScreen:function(){
        var winWidth=0;
        var winHeight=0;
        //获取窗口宽度
        if (window.innerWidth){
            winWidth = window.innerWidth;
        }else if ((document.body) && (document.body.clientWidth)){
            winWidth = document.body.clientWidth;
        }
        //获取窗口高度
        if (window.innerHeight){
            winHeight = window.innerHeight;
        }else if((document.body) && (document.body.clientHeight)){
            winHeight = document.body.clientHeight;
        }
        //通过深入Document内部对body进行检测，获取窗口高度
        if (document.documentElement && document.documentElement.clientHeight && document.documentElement.clientWidth){
            winWidth = document.documentElement.clientWidth;
            winHeight = document.documentElement.clientHeight;
        }

        $(this).css({ "height": parseInt(winHeight), "width": parseInt(winWidth), "padding-top": "20%" });
    }
});


//名稱空間
var commonjs = {};

/* 取得摘要式的錯誤 
 * ============================================================
 * Useage : commonjs.getValidationSummaryErrors(errors);
 * Param $form -> from selector
 * Return html string
 * ============================================================ */
commonjs.getValidationSummaryErrors = function ($form) {
    // We verify if we created it beforehand
    var errorSummary = $form.find('.validation-summary-errors, .validation-summary-valid');
    if (!errorSummary.length) {
        errorSummary = $('<div class="alert alert-danger alert-message">' +
        '<button type="button" class="close" data-dismiss="alert">' +
        '<i class="icon-remove"></i></button><strong><i class="icon-warning-sign red bigger-130"></i> 儲存失敗. 請修正以下的錯誤後，再試一次.</strong><ul></ul></div>')
                .prependTo($form);
    }

    return errorSummary;
};

/* Display Json Error
 * ============================================================
 * Useage : commonjs.displayJSONErrors(errors);
 * Return html string
 * ============================================================ */
commonjs.displayJSONErrors = function (errors) {
    var errorSummary = $("<div class='json-error alert alert-danger'><h4><i class='icon-warning-sign'></i> 發生錯誤</h4><ul></ul></div>")
    var items = $.map(errors, function (error) {
        return '<li>' + error + '</li>';
    }).join('');
    var ul = errorSummary
            .find('ul')
            .append(items);
    return errorSummary.wrap('<p/>').parent().html();
};

/* Display Json Error to form
 * ============================================================
 * Useage : commonjs.displayJSONErrors(errors);
 * Return append html string to form
 * ============================================================ */
commonjs.displayErrors = function (form, errors , isfade) {
    var errorSummary = commonjs.getValidationSummaryErrors(form)
            .removeClass('validation-summary-valid')
            .addClass('validation-summary-errors');

    var items = $.map(errors, function (error) {
        return '<li>' + error + '</li>';
    }).join('');

    var ul = errorSummary
            .find('ul')
            .empty()
            .append(items);

  

};


/* 將表單回到初始狀態
 * ============================================================
 * Usage : commonjs.resetForm(selector);
 * Return void
 * ============================================================ */
commonjs.resetForm = function ($form) {
    $form[0].reset();
    // We reset the form so we make sure unobtrusive errors get cleared out.
    commonjs.getValidationSummaryErrors($form)
            .removeClass('validation-summary-errors')
            .addClass('validation-summary-valid');
};

/* 表單提交 Handler
 * ============================================================
 * Useage : $("#add-form").submit(callback, commonjs.formSubmitHandler);
 *          OR $("#add-form").submit(commonjs.formSubmitHandler);
 * Return void
 * ============================================================ */
commonjs.formSubmitHandler = function (e) {
    var $form = $(this);
    var callback = null;
    if (arguments.length != 0) callback = arguments[0].data;
    
    if (!$form.valid || $form.valid()) {
        $.ajax({
            type: "POST",
            url: $form.attr('action'),
            data: $form.serializeArray(),
            contentType: "application/x-www-form-urlencoded;charset=utf-8"
        }).done(function (json) {
            json = json || {};
            // In case of success, we redirect to the provided URL or the same page.
            if (json.success) {
                if (callback != null) { callback.call(json); }
                else {
                    location = json.redirect || location.href;
                }
            } else if (json.errors) {
                commonjs.displayErrors($form, json.errors);
            }
        })
        .error(function () {
            commonjs.displayErrors($form, ['伺服器發生錯誤']);
        });
    }
    // Prevent the normal behavior since we opened the dialog
    e.preventDefault();
};


/* 表單成功通用動作 彈出訊息 觸發criteriaForm
 * ============================================================
 * Useage :
 * Return void
 * ============================================================ */
commonjs.success = function () {
    $.Alert('儲存成功', function () {
        commonjs.goBack();
        $("#criteria-form").submit();
    });
};


/* 刪除動作 彈出訊息 觸發criteriaForm
 * ============================================================
 * Useage :
 * Return void
 * ============================================================ */
commonjs.deleteHandler = function () {
    var btn = $(this);
    var url = btn.attr("href");
    $.Confirm("確定要刪除所選項目嗎!?", function () {
        $.get(url).done(function (json) {
            var json = json || {};
            if (json.success) {
                $.Alert("刪除成功!", function () {
                    $("#criteria-form").submit();
                });
            } else if (json.errors) {
                $.Alert(json.errors);
            }
        }).fail(function (fail) {
            
            $.Alert("系統發生錯誤");
        });
    });
};

commonjs.goToNextPage = function (link, url) {
    var separator = url.indexOf('?') >= 0 ? '&' : '?';
    $.get(url + separator)
            .done(function (content) {
                $("#item-index").hide();
                var detail = $("#item-detail");
                detail.html(content).filter('div').fadeIn(); // Filter for the div tag only, script tags could surface
                $.validator.unobtrusive.parse(detail.find('form'));
            });

}

commonjs.goBack = function () {
    $("#item-detail").hide().empty();
    $("#item-index").fadeIn();
}

/* Ajax 彈出視窗 使用 modal
 * ============================================================
 * Useage :
 * Return void
 * id => string ex: myDialog
 * link => jquery seletor ex: %(".class")
 * url => 直接給url
 * width => 指定寬度
 * ============================================================ */
commonjs.loadAndShowDialog = function (link, url, width) {
    var modalWidth = 650;
    if (width != undefined) modalWidth = width;

    $.get(url).done(function (content) {

        var contentArea = $("<div id='showDialog'><div class='modal-dialog' style='width:" + width + "px'><div class='modal-content'></div></div></div>")
         .find(".modal-content").append(content).end()
         .attr({ "tabindex": "-1", "role": "dialog", "aria-labelledby": "myModalLabel", "aria-hidden": "true" }).addClass("modal fade");
        contentArea.appendTo("body").modal('show');
        contentArea.bind('hidden.bs.modal', function () {
            $(this).data('bs.modal', null);
            $(this).remove();
        });

        $.validator.unobtrusive.parse(contentArea.find('form'));
    });
}

//commonjs.exportHandler = function (form, url) {
//    //        debugger;
//    $.post(url, form.serialize() + "&isExport=true")
//        .done(function (json) {
//            json = json || {};
//            if (json.success) {
//                $("body").append("<iframe src='" + json.url + "' style='display: none;' ></iframe>");
//            } else if (json.errors) {
//                $.Alert(json.errors, undefined, "伺服器錯誤", true);
//            }
//        })
//        .error(function () {
//            $.Alert("伺服器發生錯誤", undefined, "伺服器錯誤", true);
//        });
//}

commonjs.exportHandler = function (form, url, parameters) {
    //var para = new Array();
    var par = form.serialize();
    if (parameters) {
        $.each(parameters, function (idx, value) {
            par += "&" + value;
        });
    } else {
        par += "&isExport=true";
    }
    $("#item-detail2").show();
    $("#item-detail2").fullScreen();
    $("#item-detail2-text").show();
    $("#item-detail2-text").fullScreen();
    $.post(url, par)
    .done(function (json) {
        $("#item-detail2").hide();
        $("#item-detail2-text").hide();
        json = json || {};
        if (json.success) {
            $("body").append("<iframe src='" + json.url + "' style='display:none;'></iframe>");
        } else if (json.errors) {
            $.Alert(json.errors, undefined, "伺服器錯誤", true);
        }
    })
    .error(function () {
        $("#item-detail2").hide();
        $("#item-detail2-text").hide();
        $.Alert("伺服器發生錯誤", undefined, "伺服器錯誤", true);
    });
};

var Application = {};
Application.success = function () {
    $('#showDialog').modal('hide');
    $.Alert("存儲成功", function () {
        $("#criteria-form").submit();
    }, "提示");
};


Application.select2 = function (placeholder, width, url,over_ajax_data) {
    placeholder = placeholder || "請輸入資料";
    width = width || "200px";
    
    var options = {
        placeholder: placeholder,
        width: width,
        ajax: {
            url: url,
            dataType: 'json',
            quietMillis: 100,
            data: function (term, page) { // page is the one-based page number tracked by Select2
                return {
                    term: term, //search term
                    pageSize: 10, // page size
                    pageNo: page // page number
                };
            },
            results: function (data, page) {
                var more = (page * 10) < data.total;
                return { results: data.data, more: more };
            }
        },
        allowClear: true,
        formatSearching: function () { return "搜尋中，請稍後..."; },
        formatNoMatches: function () { return "沒有可查詢的資料"; },
        formatLoadMore: function () { return "更多..."; },
        minimunInputLength: 0,
    };
    
    if (over_ajax_data != undefined)
    {
        options.ajax.data = over_ajax_data;   
    }
    return options;
};



